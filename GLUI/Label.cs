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
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);
        }

        protected override void OnRender()
        {
            base.OnRender();
            if (Immediate)
            {
                Raster.Location = AbsoluteLocation;
                foreach(var wChar in Text)
                {
                    Font.DrawChar(wChar);
                }
            }
            else
            {

            }
        }

        protected override void OnUpdate()
        {
            if (Dirty)
            {
                var wSize = Font.MeasueText(Text);
                Size = new Size(Math.Max(Width, wSize.Width), Math.Max(Height, wSize.Width));
                base.OnUpdate();
            }
        }
    }
}
