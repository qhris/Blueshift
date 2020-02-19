using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Blueshift.System.Chip8;
using Blueshift.UI.Rendering;
using Blueshift.UI.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace Blueshift.UI
{
    public class EmulatorWindow : GameWindow
    {
        private static readonly TimeSpan s_titleUpdateInterval = TimeSpan.FromSeconds(1);

        private MovingFrameRate _frameRate;
        private DateTime _nextFrameRate;
        private string _baseTitle;
        private string _romDirectory;

        private bool _isDisposed;

        private Chip8System _system;

        private DynamicTexture _gameTexture;
        private Shader _gameShader;
        private Quad _quad;

        public EmulatorWindow(EmulatorWindowParameters parameters) : base(
            parameters.Width,
            parameters.Height,
            GraphicsMode.Default,
            parameters.Title,
            GameWindowFlags.Default,
            DisplayDevice.Default,
            parameters.MajorGLVersion,
            parameters.MinorGLVersion,
            GraphicsContextFlags.ForwardCompatible)
        {
            _baseTitle = parameters.Title;

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo romDirectory = DirectoryFinder.Find(baseDirectory, "Roms")
                .FirstOrDefault();
            if (romDirectory is null)
            {
                throw new DirectoryNotFoundException("Could not find the ROM directory");
            }

            _romDirectory = romDirectory.FullName;


            UpdateTitle();
        }

        private void UpdateTitle()
        {
            Title = string.Format("{0} (OpenGL {1}) FPS: {2:0.00}, Time: {3:0.00}ms, Frame: {4}",
                _baseTitle,
                GL.GetString(StringName.Version),
                _frameRate?.FrameRate ?? 0,
                _frameRate?.FrameTime.TotalMilliseconds ?? 0,
                _frameRate?.Frame ?? 0);
        }

        protected override void Dispose(bool manual)
        {
            base.Dispose(manual);

            if (_isDisposed)
            {
                return;
            }

            if (manual)
            {
                _gameTexture?.Dispose();
                _gameShader?.Dispose();
                _quad?.Dispose();
            }

            _isDisposed = true;
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            _system?.KeyDown(e.Key);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            _system?.KeyUp(e.Key);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _frameRate = new MovingFrameRate(capacity: 32);
            _nextFrameRate = DateTime.Now + s_titleUpdateInterval;

            _system = new Chip8System();
            //_system.Memory.WriteBlock(ReadROM(@"G:\Projects\NESCore\Blueshift\Roms\Chip8\Chip8 Picture.ch8"));
            //_system.Memory.WriteBlock(ReadROM("Tetris [Fran Dachille, 1991].ch8"));
            //_system.Memory.WriteBlock(ReadROM(@"G:\Projects\NESCore\Blueshift\Roms\Chip8\IBM Logo.ch8"));
            //_system.Memory.WriteBlock(ReadROM("Pong (alt).ch8"));
            _system.Memory.WriteBlock(ReadROM("Space Invaders [David Winter].ch8"));

            int gameWidth = _system.PPU.ResolutionX;
            int gameHeight = _system.PPU.ResolutionY;
            _gameTexture = new DynamicTexture(gameWidth, gameHeight);
            _gameShader = new Shader("basic");
            _gameShader.SetColor("foregroundColor", Color.ForestGreen);
            _quad = new Quad();

            _system.Power();

            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.Texture2D);
        }

        private byte[] ReadROM(string path)
        {
            path = Path.Combine(_romDirectory, "Chip8", path);

            using (var stream = new FileStream(path, FileMode.Open))
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // Update title or a regular interval.
            DateTime currentTime = DateTime.Now;
            if (currentTime >= _nextFrameRate)
            {
                _nextFrameRate = currentTime + s_titleUpdateInterval;
                UpdateTitle();
            }

            if (_system == null)
            {
                return;
            }

            for (int i = 0; i < 2; ++i)
            {
                _system.CPU?.Execute();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            _frameRate.Update();

            _system.PPU.FillFramebuffer(_gameTexture.Buffer);
            _gameTexture.Update();

            GL.Clear(ClearBufferMask.ColorBufferBit);
            _gameTexture.Bind();
            _gameShader.Bind();
            _quad.Bind();
            _quad.Render();

            SwapBuffers();
        }
    }
}
