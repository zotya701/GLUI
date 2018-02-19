using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Drawing2D;

namespace GLUI
{
    public class Label : Component
    {
        private bool mDisposed = false;

        private string mText;
        private Font mFont;
        private bool mImmediate;
        private bool mCached;
        private Alignment mAlignment;

        public string Text { get { return mText; } set { mText = value; Dirty = true; } }
        public Font Font { get { return mFont; } set { mFont = value; Dirty = true; } }
        public Color Color { get; }
        public bool Immediate { get { return mImmediate; } set { mImmediate = value; mCached = !value; } }
        public bool Cached { get { return mCached; } set { mCached = value; mImmediate = !value; } }
        public Alignment Alignment { get { return mAlignment; } set { mAlignment = value; Dirty = true; } }

        public Label()
        {
            Cached = true;
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            Text = string.Empty;
            Font = new Font("Arial", 12, Color.Black);
            Alignment = new Alignment
            {
                Vertical = Vertical.Top,
                Horizontal = Horizontal.Left
            };
        }

        protected override void OnRender()
        {
            base.OnRender();
            if (Immediate)
            {
                Raster.Location = AbsoluteLocation;
                Font.DrawText(Text);
            }
            else if (Cached)
            {
                Font.DrawCachedText();
            }
        }

        protected override void OnUpdate()
        {
            var wSize = Font.MeasureText(Text);
            Size = new Size(Math.Max(Width, wSize.Width), Math.Max(Height, wSize.Height));

            if (Cached)
            {
                var wX = 0;
                var wY = 0;
                switch (Alignment.Horizontal)
                {
                    case Horizontal.Left: wX = AbsoluteLocation.X;
                        break;
                    case Horizontal.Center: wX = (AbsoluteLocation.X + Size.Width / 2) - wSize.Width / 2;
                        break;
                    case Horizontal.Right: wX = AbsoluteLocation.X + Size.Width - wSize.Width;
                        break;
                }
                switch (Alignment.Vertical)
                {
                    case Vertical.Top: wY = AbsoluteLocation.Y;
                        break;
                    case Vertical.Center: wY = (AbsoluteLocation.Y + Size.Height / 2) - wSize.Height / 2;
                        break;
                    case Vertical.Bottom: wY = AbsoluteLocation.Y + Size.Height - wSize.Height;
                        break;
                }
                Raster.Location = new Point(wX, wY);
                Font.RegenerateTextCache(Text);
            }

            base.OnUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                Font?.Dispose();
            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
