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
        private bool mFontChanged = false;

        private string mText;
        private Alignment mAlignment;
        private string mFontFamily;
        private int mFontSize;
        private Color mFontColor;
        private Font mFont;

        public string Text
        {
            get
            {
                return mText;
            }
            set
            {
                mText = value;
                Dirty = true;
            }
        }

        public Alignment Alignment
        {
            get
            {
                return mAlignment;
            }
            set
            {
                mAlignment = value;
                Dirty = true;
            }
        }

        public string FontFamily
        {
            get
            {
                return mFontFamily;
            }
            set
            {
                mFontFamily = value;
                Dirty = true;
                mFontChanged = true;
            }
        }

        public int FontSize
        {
            get
            {
                return mFontSize;
            }
            set
            {
                mFontSize = value;
                Dirty = true;
                mFontChanged = true;
            }
        }

        public Color FontColor
        {
            get
            {
                return mFontColor;
            }
            set
            {
                mFontColor = value;
                Dirty = true;
                mFontChanged = true;
            }
        }

        public Label()
        {
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);

            Text = string.Empty;
            Alignment = new Alignment
            {
                Vertical = Vertical.Top,
                Horizontal = Horizontal.Left
            };
            FontFamily = "Arial";
            FontSize = 12;
            FontColor = Color.Black;

        }

        protected override void OnRender()
        {
            base.OnRender();
            mFont.DrawCachedText();
        }

        protected override void OnUpdate()
        {
            if (mFontChanged)
            {
                mFontChanged = false;
                mFont?.Dispose();
                mFont = Font.Create(FontFamily, FontSize, FontColor);
            }

            var wSize = mFont.MeasureText(Text);
            Size = new Size(Math.Max(Width, wSize.Width), Math.Max(Height, wSize.Height));

            var wX = 0;
            var wY = 0;
            switch (Alignment.Horizontal)
            {
                case Horizontal.Left:
                    wX = AbsoluteLocation.X;
                    break;
                case Horizontal.Center:
                    wX = (AbsoluteLocation.X + Size.Width / 2) - wSize.Width / 2;
                    break;
                case Horizontal.Right:
                    wX = AbsoluteLocation.X + Size.Width - wSize.Width;
                    break;
            }
            switch (Alignment.Vertical)
            {
                case Vertical.Top:
                    wY = AbsoluteLocation.Y;
                    break;
                case Vertical.Center:
                    wY = (AbsoluteLocation.Y + Size.Height / 2) - wSize.Height / 2;
                    break;
                case Vertical.Bottom:
                    wY = AbsoluteLocation.Y + Size.Height - wSize.Height;
                    break;
            }
            Raster.Location = new Point(wX, wY);
            mFont.RegenerateTextCache(Text);

            base.OnUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                mFont?.Dispose();
            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
