using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public class Texture
    {
        private static object mObj = new object();
        private static List<int> mDeletedTextures = new List<int>();

        public int Id { get; private set; }
        public Texture()
        {
            Id = GL.GenTexture();
            lock (mObj)
            {
                if (mDeletedTextures.Any())
                {
                    foreach(var wId in mDeletedTextures)
                    {
                        GL.DeleteTexture(wId);
                    }
                    mDeletedTextures.Clear();
                }
            }
        }

        ~Texture()
        {
            lock (mObj)
            {
                mDeletedTextures.Add(Id);
            }
        }
    }
}
