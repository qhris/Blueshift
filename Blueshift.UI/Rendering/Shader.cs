using System;
using System.Drawing;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Blueshift.UI.Rendering
{
    public class Shader : IDisposable
    {
        private bool _isDisposed = false;
        private int _program;

        public Shader(string shaderName) : this(
            File.ReadAllText(Path.ChangeExtension(Path.Combine("Shaders", shaderName), ".vs.glsl")),
            File.ReadAllText(Path.ChangeExtension(Path.Combine("Shaders", shaderName), ".fs.glsl")))
        {
        }

        public Shader(string vertex, string fragment)
        {
            int vs = CompileShader(vertex, ShaderType.VertexShader);
            int fs = CompileShader(fragment, ShaderType.FragmentShader);

            _program = CompileProgram(vs, fs);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
        }

        private int CompileShader(string content, ShaderType shaderType)
        {
            int shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, content);
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);

            if (success != (int)All.True)
            {
                string error = GL.GetShaderInfoLog(shader);
                throw new Exception("Failed to compile shader:\n" + error);
            }

            return shader;
        }

        private int CompileProgram(params int[] shaders)
        {
            int program = GL.CreateProgram();

            foreach (var shader in shaders)
            {
                GL.AttachShader(program, shader);
            }

            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);

            if (success != (int)All.True)
            {
                string error = GL.GetProgramInfoLog(program);
                throw new Exception("Failed to link program:\n" + error);
            }

            return program;
        }

        public void Bind()
        {
            GL.UseProgram(_program);
        }

        public void SetColor(string name, Color color)
        {
            int location = GL.GetUniformLocation(_program, name);
            if (location != -1)
            {
                Vector3 data = new Vector3
                {
                    X = color.R / 255.0f,
                    Y = color.G / 255.0f,
                    Z = color.B / 255.0f,
                };

                GL.ProgramUniform3(_program, location, data);
            }
        }

        #region IDisposable Support

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
                if (_program != 0)
                {
                    GL.DeleteTexture(_program);
                }
            }

            _isDisposed = true;
        }

        #endregion
    }
}
