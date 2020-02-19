using System;
using OpenTK.Graphics.OpenGL4;

namespace Blueshift.UI.Rendering
{
    public class Quad : IDisposable
    {
        private bool _isDisposed = false;
        private int _vbo;
        private int _vao;

        public Quad()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();

            var vertices = new float[]
            {
                -1.0f, -1.0f, 0.0f,
                -1.0f,  1.0f, 0.0f,
                 1.0f, -1.0f, 0.0f,
                 1.0f,  1.0f, 0.0f,
            };

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 3, 0);
            GL.EnableVertexAttribArray(0);
        }

        public void Bind()
        {
            GL.BindVertexArray(_vao);
        }

        public void Render()
        {
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        #region IDisposable Support

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) below.
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
                if (_vao != 0)
                {
                    GL.DeleteVertexArray(_vao);
                }

                if (_vbo != 0)
                {
                    GL.DeleteBuffer(_vbo);
                }
            }

            _isDisposed = true;
        }

        #endregion
    }
}
