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

        public static Scissor Backup()
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
            GL.Scissor(mLocation.X, wViewport[3] - mLocation.Y - mSize.Height, mSize.Width, mSize.Height);
        }

        public Scissor Merge(Scissor scissor)
        {
            if (scissor == null) return this;

            var wX = Math.Max(mLocation.X, scissor.mLocation.X);
            var wY = Math.Max(mLocation.Y, scissor.mLocation.Y);
            var wTopLeft = new Point(wX, wY);

            var wThisBottomRight = mLocation + mSize;
            var wBottomRight = scissor.mLocation + scissor.mSize;
            wX = Math.Min(wThisBottomRight.X, wBottomRight.X);
            wY = Math.Min(wThisBottomRight.Y, wBottomRight.Y);
            wBottomRight = new Point(wX, wY);

            var wWidth = wBottomRight.X - wTopLeft.X;
            var wHeigth = wBottomRight.Y - wTopLeft.Y;
            var wSize = new Size(wWidth, wHeigth);

            return new Scissor(wTopLeft, wSize);
        }
    }
}
