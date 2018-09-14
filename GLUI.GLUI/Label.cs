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
            public static System.Drawing.Font Font { get; set; } = new System.Drawing.Font("Courier New", 12, FontStyle.Regular, GraphicsUnit.Point);
            public static Color ForegroundColor { get; set; } = Color.LightGray;
            public static Color DisabledForegroundColor { get; set; } = Color.FromArgb(140, 140, 140);
            public new static Color BackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
            public new static Color DisabledBackgroundColor { get; set; } = Color.FromArgb(0, 0, 0, 0);
            public new static float BorderWidth { get; set; } = 0.0f;
        }

        private bool mDisposed = false;

        private Bitmap mImage;
        private Graphics mGraphics;
        private Texture mTexture;
        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mTexCoordsId = 0;
        private int mIndicesCount = 0;

        private string mText;
        private HorizontalAlignment mHorizontalAlignment;
        private VerticalAlignment mVerticalAlignment;
        private System.Drawing.Font mFont;
        private Color mForegroundColor;
        private Color mDisabledForegroundColor;

        public bool IsClicked { get; protected set; } = false;

        internal event EventHandler mLabelPressed;
        internal event EventHandler mLabelReleased;

        public event EventHandler Clicked;

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

        public System.Drawing.Font Font
        {
            get
            {
                return mFont;
            }
            set
            {
                if (mFont == value) return;

                mFont = value;
                Dirty = true;
            }
        }

        public Color ForegroundColor
        {
            get
            {
                return mForegroundColor;
            }
            set
            {
                if (mForegroundColor == value) return;

                mForegroundColor = value;
                Dirty = true;
            }
        }

        public Color DisabledForegroundColor
        {
            get
            {
                return mDisabledForegroundColor;
            }
            set
            {
                if (mDisabledForegroundColor == value) return;

                mDisabledForegroundColor = value;
                Dirty = true;
            }
        }

        public Label() : this(Default.Text)
        {

        }

        public Label(string text)
        {
            Text = text;
            HorizontalAlignment = Default.HorizontalAlignment;
            VerticalAlignment = Default.VerticalAlignment;
            Font = Default.Font.Clone() as System.Drawing.Font;

            ForegroundColor = Default.ForegroundColor;
            DisabledForegroundColor = Default.DisabledForegroundColor;
            BackgroundColor = Default.BackgroundColor;
            DisabledBackgroundColor = Default.DisabledBackgroundColor;

            BorderWidth = Default.BorderWidth;

            mImage = new Bitmap(1, 1);
            mGraphics = Graphics.FromImage(mImage);
            mGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            mGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            mGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            mGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public (int Width, int Height) MeasureText(string text)
        {
            var wSize = mGraphics.MeasureString(text, mFont);
            return ((int)Math.Ceiling(wSize.Width), (int)Math.Ceiling(wSize.Height));
        }

        public static (int Width, int Height) MeasureText(string text, System.Drawing.Font font)
        {
            using (var wImage = new Bitmap(1, 1))
            using (var wGraphics = Graphics.FromImage(wImage))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                wGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var wSize = wGraphics.MeasureString(text, font);
                return ((int)Math.Ceiling(wSize.Width), (int)Math.Ceiling(wSize.Height));
            }
        }

        protected virtual internal void OnPressed(object s, EventArgs e)
        {
            mLabelPressed?.Invoke(s, e);
            Clicked?.Invoke(s, e);
        }

        protected virtual internal void OnReleased(object s, EventArgs e)
        {
            mLabelReleased?.Invoke(s, e);
        }

        protected virtual internal void OnClick(object s, EventArgs e)
        {
            Clicked?.Invoke(s, e);
        }

        protected override void GreyOut()
        {
            //base.GreyOut();
            //mOriginalFontColor = FontColor;
            //FontColor = DisabledFontColor;
        }

        protected override void Highlight()
        {
            //base.Highlight();
        }

        protected override void ResetColors()
        {
            //base.ResetColors();
            //FontColor = mOriginalFontColor;
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if (mouseState.IsOverDirectly && mouseState.Button == OpenTK.Input.MouseButton.Left && mouseState.IsPressed)
            {
                OnPressed(this, EventArgs.Empty);
                IsClicked = true;
            }
            if (IsClicked && mouseState.ButtonDown[OpenTK.Input.MouseButton.Left] == false)
            {
                OnReleased(this, EventArgs.Empty);
                IsClicked = false;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();

            if (string.IsNullOrEmpty(mText)) return;

            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.VertexPointer(2, VertexPointerType.Float, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);

            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTexCoordsId);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, 0);

            GL.DrawElements(BeginMode.Triangles, mIndicesCount, DrawElementsType.UnsignedInt, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected override void OnUpdate()
        {
            if (string.IsNullOrEmpty(mText)) return;
            var (wWidth, wHeight) = MeasureText(mText);
            using (var wImage = new Bitmap(wWidth, wHeight))
            using (var wGraphics = Graphics.FromImage(wImage))
            {
                // Render the text to the bitmap
                wGraphics.Clear(Color.Transparent);

                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                wGraphics.DrawString(mText, mFont, new SolidBrush(mForegroundColor), 0, 0);
                wGraphics.Flush();

                // Create the texture from the bitmap
                mTexture?.Dispose();
                mTexture = new Texture
                {
                    Width = wWidth,
                    Height = wHeight
                };
                GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
                var wData = wImage.LockBits(new Rectangle(0, 0, wWidth, wHeight), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, wWidth, wHeight, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
                wImage.UnlockBits(wData);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.BindTexture(TextureTarget.Texture2D, 0);





                //GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 0f);
                //GL.TexCoord2(1f, 0f); GL.Vertex2(mTexture.Width, 0f);
                //GL.TexCoord2(1f, 1f); GL.Vertex2(mTexture.Width, mTexture.Height);

                //GL.TexCoord2(1f, 1f); GL.Vertex2(mTexture.Width, mTexture.Height);
                //GL.TexCoord2(0f, 1f); GL.Vertex2(0f, mTexture.Height);
                //GL.TexCoord2(0f, 0f); GL.Vertex2(0f, 0f);

                // Create the vertices to render the texture
                var wVertices = new List<float>
                {                       // INDICES
                    0, 0,               // 0, Upper left
                    wWidth, 0,          // 1, Upper right
                    wWidth, wHeight,    // 2, Bottom right
                    0, wHeight          // 3, Bottom left
                };
                var wIndices = new List<uint>
                {
                    0, 1, 2,  2, 3, 0
                };
                var wTexCoords = new List<float>
                {
                    0, 0,
                    1, 0,
                    1, 1,
                    0, 1
                };
                if (mVerticesId == 0) mVerticesId = GL.GenBuffer();
                if (mIndicesId == 0) mIndicesId = GL.GenBuffer();
                if (mTexCoordsId == 0) mTexCoordsId = GL.GenBuffer();
                mIndicesCount = wIndices.Count;

                GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, mTexCoordsId);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * wTexCoords.Count, wTexCoords.ToArray(), BufferUsageHint.StaticDraw);
            }

            Size = new Vector2(Math.Max(Width, wWidth),
                               Math.Max(Height, wHeight));

            base.OnUpdate();


            //if (mFontChanged)
            //{
            //    mFontChanged = false;
            //    mFontColorChanged = false;
            //    mFont?.Dispose();
            //    mFont = new Font(FontFamily, FontSize, FontColor);
            //}
            //if (mFontColorChanged)
            //{
            //    mFontColorChanged = false;
            //    if (mFont != null)
            //    {
            //        mFont.Color = FontColor;
            //    }
            //}

            //var wSize = mFont.MeasureText(Text);
            //Size = new Vector2(Math.Max(Width, wSize.X),
            //                   Math.Max(Height, wSize.Y));

            //float wX = 0;
            //float wY = 0;
            //switch (HorizontalAlignment)
            //{
            //    case HorizontalAlignment.Left: wX = 0; break;
            //    case HorizontalAlignment.Center: wX = Size.X / 2 - wSize.X / 2; break;
            //    case HorizontalAlignment.Right: wX = Size.X - wSize.X; break;
            //}
            //switch (VerticalAlignment)
            //{
            //    case VerticalAlignment.Top: wY = 0; break;
            //    case VerticalAlignment.Center: wY = Size.Y / 2 - wSize.Y / 2; break;
            //    case VerticalAlignment.Bottom: wY = Size.Y - wSize.Y; break;
            //}
            //Raster.Location = new Vector2((float)Math.Round(wX), (float)Math.Round(wY));
            //mFont.RegenerateTextCache(Text);

            //base.OnUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                mFont?.Dispose();
                mGraphics.Dispose();
                mImage.Dispose();
            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
