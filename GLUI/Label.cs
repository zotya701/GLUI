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
        private class CharTextureData
        {
            public Point Location { get; set; }
            public Size Size { get; set; }
        }

        private int mFrameBuffer;
        private int mCachedTexture;
        private int mFontTexture;
        public string Text { get { return mText; } set { mText = value; Dirty = true; } }
        private string mText;
        public Font Font { get { return mFont; } set { mFont = value; GenerateFontTexture(); } }
        private Font mFont;
        public Color Color { get; set; }
        public bool Immediate { get { return mImmediate; } set { mImmediate = value; mCached = !value; } }
        private bool mImmediate;
        public bool Cached { get { return mCached; } set { mCached = value; mImmediate = !value; } }
        private bool mCached;
        static public string CharSet
        {
            get
            {
                if (string.IsNullOrEmpty(mCharSet))
                {
                    mCharSet = "";
                    foreach (var wCharCode in Enumerable.Range(0, Byte.MaxValue + 1))
                    {
                        if ((32 <= wCharCode && wCharCode <= 126) || (160 <= wCharCode && wCharCode <= Byte.MaxValue))
                        {
                            mCharSet = mCharSet + Convert.ToChar(wCharCode);
                        }
                        else
                        {
                            mCharSet = mCharSet + " ";
                        }
                    }
                }
                return mCharSet;
            }
        }
        static private string mCharSet;
        private Dictionary<char, CharTextureData> mLookUpTable;
        private Bitmap mCharSetImage;

        public Label()
        {
            mFrameBuffer = GL.GenFramebuffer();
            mCachedTexture = GL.GenTexture();
            mFontTexture = GL.GenTexture();
        }

        protected override void OnRender()
        {
            base.OnRender();
            if (Immediate)
            {

            }
            else
            {

            }
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, mFontTexture);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex2(200, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex2(200, 200);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, 200);
            GL.End();
        }

        protected override void OnUpdate()
        {
            if (Dirty)
            {
                base.OnUpdate();
            }
        }

        public void SaveCharSetImage()
        {
            var wImage = new Bitmap(mCharSetImage);
            using (var wGraphics = Graphics.FromImage(wImage))
            using (var wPen = new Pen(Color.FromArgb(Color.R, Color.G, Color.B)))
            {
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                mLookUpTable.ToList().ForEach(wTuple => wGraphics.DrawRectangle(wPen, new Rectangle(wTuple.Value.Location, wTuple.Value.Size)));
                wGraphics.Flush();
            }
            wImage.Save("TEST.bmp");
        }

        private void GenerateFontTexture()
        {
            var wOffset = new Size(5, 5);
            mLookUpTable = new Dictionary<char, CharTextureData>();
            var wTable = new List<List<Tuple<char, Size>>> { new List<Tuple<char, Size>>() };
            foreach (var wChar in CharSet.Distinct())
            {
                var wSize = TextRenderer.MeasureText($"{wChar}", Font, new Size(), TextFormatFlags.NoPrefix) + wOffset;
                var wHeight = wTable.Count * Font.Height;
                var wMaxWidth = wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.Width));
                var wMinWidth = wTable.Min(wRow => wRow.Sum(wColumn => wColumn.Item2.Width));
                if (wMinWidth + wSize.Width < wHeight)
                {
                    (wTable.FirstOrDefault(wRow => wRow.Sum(wColumn => wColumn.Item2.Width) == wMinWidth) ?? wTable.First()).Add(new Tuple<char, Size>(wChar, wSize));
                }
                else
                {
                    wTable.Add(new List<Tuple<char, Size>> { new Tuple<char, Size>(wChar, wSize) });
                }
            }

            var wLocation = new Point(0, 0) + wOffset;
            foreach (var wRow in wTable)
            {
                foreach (var wColumn in wRow)
                {
                    mLookUpTable[wColumn.Item1] = new CharTextureData { Location = wLocation, Size = wColumn.Item2 - wOffset };
                    wLocation.X = wLocation.X + wColumn.Item2.Width;
                }
                wLocation.X = wOffset.Width;
                wLocation.Y = wLocation.Y + Font.Height + wOffset.Height;
            }


            var wMWidth = wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.Width)) + wOffset.Width;
            var wMHeight = wTable.Count * (Font.Height + wOffset.Height) + wOffset.Height;
            mCharSetImage = new Bitmap(wMWidth, wMHeight);
            using (var wGraphics = Graphics.FromImage(mCharSetImage))
            using (var wPen = new Pen(Color.FromArgb(Color.R, Color.G, Color.B)))
            {
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                mLookUpTable.ToList().ForEach(wTuple => TextRenderer.DrawText(wGraphics, $"{wTuple.Key}", Font, wTuple.Value.Location, Color.FromArgb(Color.R, Color.G, Color.B), TextFormatFlags.NoPrefix));
                //mLookUpTable.ToList().ForEach(wTuple => wGraphics.DrawRectangle(wPen, new Rectangle(wTuple.Value.Location, wTuple.Value.Size)));
                wGraphics.Flush();
            }
            //mCharSetImage.Save("TEST.bmp");

            SaveCharSetImage();






            //var wSize = TextRenderer.MeasureText(Text, Font);
            //var wImage = new Bitmap(wSize.Width, wSize.Height);
            //using (var wGraphics = Graphics.FromImage(wImage))
            //{
            //    wGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            //    wGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //    wGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            //    wGraphics.DrawString(Text, Font, new SolidBrush(Color), new PointF(0, 0));
            //    wGraphics.Flush();
            //}

            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //GL.BindTexture(TextureTarget.Texture2D, mFontTexture);
            //var wData = wImage.LockBits(new Rectangle(0, 0, wImage.Width, wImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, wImage.Width, wImage.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            //wImage.UnlockBits(wData);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }
    }
}
