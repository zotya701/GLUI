using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Drawing2D;
using OpenTK;
using Foundation;

namespace GLUI
{
    public class Label : Component
    {
        private bool mDisposed = false;
        private bool mFontChanged = false;
        private bool mFontColorChanged = false;

        private string mText;
        private Alignment mAlignment;
        private string mFontFamily;
        private float mFontSize;
        private Color mFontColor;
        private Font mFont;

        public float GLScale { get; set; } = 1.0f;

        public bool Clicked { get; protected set; } = false;

        internal event EventHandler mLabelPressed;
        internal event EventHandler mLabelReleased;

        public event EventHandler LabelClicked;

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

        public float FontSize
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
                mFontColorChanged = true;
            }
        }

        public Label()
        {
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            BorderWidth = 0;

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

        protected virtual internal void OnPressed(object s, EventArgs e)
        {
            mLabelPressed?.Invoke(s, e);
            LabelClicked?.Invoke(s, e);
        }

        protected virtual internal void OnReleased(object s, EventArgs e)
        {
            mLabelReleased?.Invoke(s, e);
        }

        protected virtual internal void OnClick(object s, EventArgs e)
        {
            LabelClicked?.Invoke(s, e);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if (mouseState.IsOverDirectly && mouseState.Button == OpenTK.Input.MouseButton.Left && mouseState.IsPressed)
            {
                OnPressed(this, new EventArgs());
                Clicked = true;
            }
            if (Clicked && mouseState.ButtonDown[OpenTK.Input.MouseButton.Left] == false)
            {
                OnReleased(this, new EventArgs());
                Clicked = false;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
            var wCenter = Size * 0.5f;
            GL.PushMatrix();
            GL.Translate(-wCenter.X * GLScale, -wCenter.Y * GLScale, 0);
            GL.Scale(GLScale, GLScale, 1);
            GL.Translate(wCenter.X / GLScale, wCenter.Y / GLScale, 0);
            mFont.DrawCachedText();
            GL.PopMatrix();
        }

        protected override void OnUpdate()
        {
            if (mFontChanged)
            {
                mFontChanged = false;
                mFontColorChanged = false;
                mFont?.Dispose();
                mFont = new Font(FontFamily, FontSize, FontColor);
            }
            if (mFontColorChanged)
            {
                mFontColorChanged = false;
                if (mFont != null)
                {
                    mFont.Color = FontColor;
                }
            }

            var wSize = mFont.MeasureText(Text);
            Size = new Vector2(Math.Max(Width, wSize.X),
                               Math.Max(Height, wSize.Y));

            float wX = 0;
            float wY = 0;
            switch (Alignment.Horizontal)
            {
                case Horizontal.Left:
                    wX = 0;
                    break;
                case Horizontal.Center:
                    wX = Size.X / 2 - wSize.X / 2;
                    break;
                case Horizontal.Right:
                    wX = Size.X - wSize.X;
                    break;
            }
            switch (Alignment.Vertical)
            {
                case Vertical.Top:
                    wY = 0;
                    break;
                case Vertical.Center:
                    wY = Size.Y / 2 - wSize.Y / 2;
                    break;
                case Vertical.Bottom:
                    wY = Size.Y - wSize.Y;
                    break;
            }
            Raster.Location = new Vector2((float)Math.Round(wX), (float)Math.Round(wY));
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
