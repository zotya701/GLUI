using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public class Texture : IDisposable
    {
        private bool mDisposed = false;

        public int Id { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Texture()
        {
            if (Id == 0) Id = GL.GenTexture();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                if (Id != 0) GL.DeleteTexture(Id);
            }

            mDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
    }
}
