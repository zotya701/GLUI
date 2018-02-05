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
        public string Text { get { return mText; } set { mText = value; Dirty = true; } }
        private string mText;
        public Font Font { get { return mFont; } set { mFont = value; Dirty = true; } }
        private Font mFont;
        public Color Color { get; }
        public bool Immediate { get { return mImmediate; } set { mImmediate = value; mCached = !value; } }
        private bool mImmediate;
        public bool Cached { get { return mCached; } set { mCached = value; mImmediate = !value; } }
        private bool mCached;

        public Label()
        {
            Cached = true;
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);
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
                Raster.Location = AbsoluteLocation;
                Font.RegenerateTextCache(Text);
            }

            base.OnUpdate();
        }
    }
}
