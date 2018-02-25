using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace GLUI
{
    public class Scissor
    {
        private Vector2 mLocation;
        private Vector2 mSize;

        public Scissor(float x, float y, float width, float height)
        {
            mLocation = new Vector2(x, y);
            mSize = new Vector2(width, height);
        }

        public Scissor(Vector2 location, Vector2 size)
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
            var wX = Convert.ToInt32(mLocation.X);
            var wY = Convert.ToInt32(wViewport[3] - mLocation.Y - mSize.Y);
            var wWidth = Convert.ToInt32(mSize.X);
            var wHeight = Convert.ToInt32(mSize.Y);
            GL.Scissor(wX, wY, wWidth, wHeight);
        }

        public Scissor Merge(Scissor scissor)
        {
            if (scissor == null) return this;

            var wX = Math.Max(mLocation.X, scissor.mLocation.X);
            var wY = Math.Max(mLocation.Y, scissor.mLocation.Y);
            var wTopLeft = new Vector2(wX, wY);

            var wThisBottomRight = mLocation + mSize;
            var wBottomRight = scissor.mLocation + scissor.mSize;
            wX = Math.Min(wThisBottomRight.X, wBottomRight.X);
            wY = Math.Min(wThisBottomRight.Y, wBottomRight.Y);
            wBottomRight = new Vector2(wX, wY);

            var wWidth = wBottomRight.X - wTopLeft.X;
            var wHeigth = wBottomRight.Y - wTopLeft.Y;
            var wSize = new Vector2(wWidth, wHeigth);

            return new Scissor(wTopLeft, wSize);
        }
    }
}
