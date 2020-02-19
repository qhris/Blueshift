using System;
using OpenTK.Graphics.OpenGL4;

namespace Blueshift.UI.Rendering
{
    public class DynamicTexture : IDisposable
    {
        private bool _isDisposed = false;
        private uint _textureHandle;

        public DynamicTexture(int width, int height)
        {
            Width = width;
            Height = height;
            Buffer = new byte[width * height * 3];
            
            _textureHandle = CreateTextureResource();
        }

        public int Width { get; }

        public int Height { get; }

        public byte[] Buffer { get; }


        #region IDisposable Interface

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (_textureHandle != 0)
                {
                    GL.DeleteTexture(_textureHandle);
                }
            }

            _isDisposed = true;
        }

        #endregion

        private uint CreateTextureResource()
        {
            GL.CreateTextures(TextureTarget.Texture2D, 1, out uint texture);

            // Requires OpenGL 4.5+
            GL.TextureParameter(texture, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TextureParameter(texture, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TextureStorage2D(texture, 1, SizedInternalFormat.Rgba32f, Width, Height);

            byte[] pixels = Buffer;
            for (int i = 0; i < pixels.Length; i += 3)
            {
                byte c = (byte)((i % 2) == 0 ? 255 : 0);
                pixels[i + 0] = c;
                pixels[i + 1] = c;
                pixels[i + 2] = c;
            }

            // GL.TexSubImage2D()
            // GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);
            GL.TextureSubImage2D(texture, 0, 0, 0, Width, Height, PixelFormat.Rgb, PixelType.UnsignedByte, pixels);

            return texture;
        }

        public void Update()
        {
            GL.TextureSubImage2D(_textureHandle, 0, 0, 0, Width, Height, PixelFormat.Rgb, PixelType.UnsignedByte, Buffer);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureHandle);
        }
    }
}
