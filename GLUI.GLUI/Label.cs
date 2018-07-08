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
using GLUI.Foundation;

namespace GLUI.GLUI
{
    public class Label : Component
    {
        public new class Default : Component.Default
        {
            public static string Text { get; set; } = "Label";
            public static HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
            public static VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
            public static string FontFamily { get; set; } = "Arial";
            public static float FontSize { get; set; } = 12.0f;
            public static Color FontColor { get; set; } = Color.LightGray;
            public static Color DisabledFontColor { get; set; } = Color.FromArgb(140, 140, 140);
            public new static Color BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
            public new static float BorderWidth { get; set; } = 0.0f;
        }

        private bool mDisposed = false;
        private bool mFontChanged = false;
        private bool mFontColorChanged = false;

        private string mText;
        private HorizontalAlignment mHorizontalAlignment;
        private VerticalAlignment mVerticalAlignment;
        private string mFontFamily;
        private float mFontSize;
        private Color mFontColor;
        private Color mDisabledFontColor;
        private Color mOriginalFontColor;
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
                if (string.Equals(mText, value)) return;

                mText = value;
                Dirty = true;
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                return mHorizontalAlignment;
            }
            set
            {
                if (mHorizontalAlignment == value) return;

                mHorizontalAlignment = value;
                Dirty = true;
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                return mVerticalAlignment;
            }
            set
            {
                if (mVerticalAlignment == value) return;

                mVerticalAlignment = value;
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
                if (mFontFamily == value) return;

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
                if (mFontSize == value) return;

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
                if (mFontColor == value) return;

                mFontColor = value;
                Dirty = true;
                mFontColorChanged = true;
            }
        }

        public Color DisabledFontColor
        {
            get
            {
                return mDisabledFontColor;
            }
            set
            {
                if (mDisabledFontColor == value) return;

                mDisabledFontColor = value;
                Dirty = true;
            }
        }

        public Label() : this(Default.Text)
        {

        }

        public Label(string text)
        {
            BackgroundColor = Default.BackgroundColor;
            BorderWidth = Default.BorderWidth;

            Text = text;
            HorizontalAlignment = Default.HorizontalAlignment;
            VerticalAlignment = Default.VerticalAlignment;
            FontFamily = Default.FontFamily;
            FontSize = Default.FontSize;
            FontColor = Default.FontColor;
            DisabledFontColor = Default.DisabledFontColor;
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

        protected override void GreyOut()
        {
            //base.GreyOut();
            mOriginalFontColor = FontColor;
            FontColor = DisabledFontColor;
        }

        protected override void Highlight()
        {
            //base.Highlight();
        }

        protected override void ResetColors()
        {
            //base.ResetColors();
            FontColor = mOriginalFontColor;
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
            //var wCenter = Size * 0.5f;
            //GL.PushMatrix();
            //GL.Translate(-wCenter.X * GLScale, -wCenter.Y * GLScale, 0);
            //GL.Scale(GLScale, GLScale, 1);
            //GL.Translate(wCenter.X / GLScale, wCenter.Y / GLScale, 0);
            if (string.IsNullOrEmpty(Text) == false)
            {
                mFont.DrawCachedText();
            }
            //GL.PopMatrix();
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
            switch (HorizontalAlignment)
            {
                case HorizontalAlignment.Left: wX = 0; break;
                case HorizontalAlignment.Center: wX = Size.X / 2 - wSize.X / 2; break;
                case HorizontalAlignment.Right: wX = Size.X - wSize.X; break;
            }
            switch (VerticalAlignment)
            {
                case VerticalAlignment.Top: wY = 0; break;
                case VerticalAlignment.Center: wY = Size.Y / 2 - wSize.Y / 2; break;
                case VerticalAlignment.Bottom: wY = Size.Y - wSize.Y; break;
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
