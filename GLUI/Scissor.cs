using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public class Scissor
    {
        private Point mLocation;
        private Size mSize;

        public Scissor(int x, int y, int width, int height)
        {
            mLocation = new Point(x, y);
            mSize = new Size(width, height);
        }

        public Scissor(Point location, Size size)
        {
            mLocation = location;
            mSize = size;
        }

        public Scissor Backup()
        {
            var wViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, wViewport);
            var wScissor = new int[4];
            GL.GetInteger(GetPName.ScissorBox, wScissor);
            return new Scissor(wScissor[0], wViewport[1] - wScissor[1], wScissor[2], wScissor[3]);
        }

        public void Apply()
        {
            var wViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, wViewport);
            GL.Scissor(mLocation.X, wViewport[1] - mLocation.Y, mSize.Width, mSize.Height);
        }
    }
}
