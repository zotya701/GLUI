using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GLUI.GLUI
{
    public class Font : IDisposable
    {
        private class FontTextureData
        {
            public Vector2 Location { get; set; }
            public Vector2 Size { get; set; }
        }

        private bool mDisposed = false;

        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mTexCoordsId = 0;
        private int mIndicesCount = 0;

        private Texture mTexture;
        private Color mColor;
        private System.Drawing.Font mFont;
        private int mFontHeight;
        private Dictionary<char, FontTextureData> mLookUpTable;
        private Vector2 mOriginalRasterLocation;
        private static string mCharSet;

        /// <summary>
        /// Specifies the font's color.
        /// </summary>
        public Color Color
        {
            get
            {
                return mColor;
            }
            set
            {
                if (Color == value) return;

                mColor = value;
                OnColorChanged();
            }
        }

        /// <summary>
        /// Gets the font's family name, i.e. "Arial". 
        /// </summary>
        public string FamilyName { get { return mFont.FontFamily.Name; } }

        /// <summary>
        /// Gets the font's size.
        /// </summary>
        public int Size { get { return mFontHeight; } }

        /// <summary>
        /// The available letters.
        /// </summary>
        public static string CharSet
        {
            get
            {
                if (string.IsNullOrEmpty(mCharSet))
                {
                    mCharSet = "";
                    foreach (var wCharCode in Enumerable.Range(0, Byte.MaxValue + 1))
                    {
                        mCharSet = mCharSet + Convert.ToChar(wCharCode);
                    }
                }
                return mCharSet;
            }
        }

        /// <summary>
        /// Creates a new font.
        /// </summary>
        /// <param name="familyName">The font's family name, i.e. "Arial".</param>
        /// <param name="size">The font's size.</param>
        /// <param name="color">The font's color.</param>
        public Font(string familyName, float size, Color color)
        {
            size = size + 3;
            mFont = new System.Drawing.Font(familyName, size, GraphicsUnit.Pixel);
            Color = color;
            GenerateFontMap();
        }

        /// <summary>
        /// Draws the cached text.
        /// </summary>
        public void DrawCachedText()
        {
            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.VertexPointer(2, VertexPointerType.Int, 0, 0);

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

        /// <summary>
        /// Regenerates the cache according to the given text.
        /// </summary>
        /// <param name="text"></param>
        public void RegenerateTextCache(string text)
        {
            mOriginalRasterLocation = Raster.Location;

            var wVertices = new List<int>();
            var wIndices = new List<uint>();
            var wTexCoords = new List<float>();

            for (int i = 0; i < text.Length; ++i)
            {
                var wChar = text[i];

                wVertices.AddRange(CalculateVertices(wChar).Select(wVertex => new int[] { wVertex.X, wVertex.Y }).SelectMany(wVertex => wVertex));
                wTexCoords.AddRange(CalculateTexCoords(wChar).Select(wVertex => new float[] { wVertex.X, wVertex.Y }).SelectMany(wVertex => wVertex));
                wIndices.AddRange(new List<uint> { 0, 1, 2, 2, 3, 0 }.Select(wIndex => (uint)(wIndex + i * 4)));

                MoveRaster(wChar);
            }

            if (mVerticesId == 0) mVerticesId = GL.GenBuffer();
            if (mIndicesId == 0) mIndicesId = GL.GenBuffer();
            if (mTexCoordsId == 0) mTexCoordsId = GL.GenBuffer();
            mIndicesCount = wIndices.Count;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(int) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTexCoordsId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * wTexCoords.Count, wTexCoords.ToArray(), BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Calculates the texture coordinates of a given letter.
        /// </summary>
        /// <param name="c">The letter which we want it's texture coordinates.</param>
        /// <returns>The texture coordinates of the bounding rectangle of the letter.</returns>
        private PointF[] CalculateTexCoords(char c)
        {
            var wX = (float)mLookUpTable[c].Location.X / mTexture.Width;
            var wY = (float)mLookUpTable[c].Location.Y / mTexture.Height;
            var wWidth = (float)mLookUpTable[c].Size.X / mTexture.Width;
            var wHeight = (float)mLookUpTable[c].Size.Y / mTexture.Height;

            var wTP1 = new PointF(wX, wY);
            var wTP2 = new PointF(wTP1.X + wWidth, wTP1.Y);
            var wTP3 = new PointF(wTP2.X, wTP1.Y + wHeight);
            var wTP4 = new PointF(wTP1.X, wTP3.Y);

            return new PointF[] { wTP1, wTP2, wTP3, wTP4 };
        }

        /// <summary>
        /// Calculates the vertices of a given letter.
        /// </summary>
        /// <param name="c">The letter which we want it's vertices.</param>
        /// <returns>The vertices of the bounding rectangle of the letter.</returns>
        private Point[] CalculateVertices(char c)
        {
            var wX = (int)mLookUpTable[c].Location.X;
            var wY = (int)mLookUpTable[c].Location.Y;
            var wWidth = (int)mLookUpTable[c].Size.X;
            var wHeight = (int)mLookUpTable[c].Size.Y;

            var wP1 = new Point((int)Raster.Location.X, (int)Raster.Location.Y);
            var wP2 = new Point(wP1.X + wWidth, wP1.Y);
            var wP3 = new Point(wP2.X, wP1.Y + wHeight);
            var wP4 = new Point(wP1.X, wP3.Y);

            return new Point[] { wP1, wP2, wP3, wP4 };
        }

        /// <summary>
        /// Moves the raster according to the given letter.
        /// </summary>
        /// <param name="c"></param>
        private void MoveRaster(char c)
        {
            if (c == ' ')
            {
                Raster.Move(mLookUpTable[c].Size.X, 0);
            }

            switch (c)
            {
                case '\r': Raster.Move(mOriginalRasterLocation.X - Raster.Location.X, 0); break;
                case '\n': Raster.Move(0, mFontHeight); break;
                default: Raster.Move(mLookUpTable[c].Size.X - (mFontHeight / 4), 0); break;
            }
        }

        /// <summary>
        /// Measures the size of the given text.
        /// </summary>
        /// <param name="text">The text to be measured.</param>
        /// <returns>The width and height of the given text in pixels.</returns>
        public Vector2 MeasureText(string text)
        {
            mOriginalRasterLocation = Raster.Location;
            var wBottomRight = new Vector2(0, 0);

            foreach (var wChar in text)
            {
                MoveRaster(wChar);
                wBottomRight.X = Math.Max(wBottomRight.X, (int)Raster.Location.X + mFontHeight / 4);
                wBottomRight.Y = Math.Max(wBottomRight.Y, (int)Raster.Location.Y + mFontHeight);
            }

            var wSize = wBottomRight - mOriginalRasterLocation;

            Raster.Location = mOriginalRasterLocation;

            return wSize;
        }

        /// <summary>
        /// Saves the texture atlas of a font as a .bmp.
        /// </summary>
        public void SaveCharacterSet()
        {
            // Create bitmap from the texture
            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
            var wCharacterSetImage = new Bitmap(mTexture.Width, mTexture.Height);
            var wData = wCharacterSetImage.LockBits(new Rectangle(0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wCharacterSetImage.UnlockBits(wData);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            using (var wPen = new Pen(Color.FromArgb(Color.R, Color.G, Color.B)))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                foreach (var wTuple in mLookUpTable)
                {
                    wGraphics.DrawRectangle(wPen, (int)wTuple.Value.Location.X, (int)wTuple.Value.Location.Y, (int)wTuple.Value.Size.X, (int)wTuple.Value.Size.Y);
                }
                wGraphics.Flush();
            }

            // Save the image
            var wFileName = $"{FamilyName}_{Size}_{Color.ToString()}_";
            var wFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{wFileName}*").ToList();
            if (wFiles.Any())
            {
                var wId = wFiles.Select(wPath => Path.GetFileNameWithoutExtension(wPath)).Select(wPath => int.Parse(wPath.Substring(wPath.LastIndexOf('_') + 1))).Max() + 1;
                wFileName = $"{wFileName}{wId}.bmp";
            }
            else
            {
                wFileName = $"{wFileName}0.bmp";
            }
            wCharacterSetImage.Save(wFileName);
        }

        /// <summary>
        /// Regenerates the texture atlas when the color changes.
        /// </summary>
        private void OnColorChanged()
        {
            if (mTexture == null) return;

            // Generate the bitmap containing the texture atlas
            var wCharacterSetImage = new Bitmap(mTexture.Width, mTexture.Height);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                foreach (var wTuple in mLookUpTable)
                {
                    wGraphics.DrawString($"{wTuple.Key}", mFont, new SolidBrush(Color.FromArgb(Color.R, Color.G, Color.B)), (int)wTuple.Value.Location.X, (int)wTuple.Value.Location.Y);
                }
                wGraphics.Flush();
            }

            // Update the texture
            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
            var wData = wCharacterSetImage.LockBits(new Rectangle(0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, wCharacterSetImage.Width, wCharacterSetImage.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wCharacterSetImage.UnlockBits(wData);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Generates the texture atlas.
        /// </summary>
        private void GenerateFontMap()
        {
            mLookUpTable = new Dictionary<char, FontTextureData>();
            var wTable = new List<List<Tuple<char, Vector2>>> { new List<Tuple<char, Vector2>>() };
            mFontHeight = 0;

            // Order the characters to fit into a square like shape
            using (var wGraphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                var wTotalWidth = 0;
                for (int i = 0; i < CharSet.Length; ++i)
                {
                    wTotalWidth = wTotalWidth + wGraphics.MeasureString($"{CharSet[i]}", mFont, int.MaxValue, StringFormat.GenericDefault).ToSize().Width;
                }
                var wLimit = wTotalWidth / Math.Ceiling(Math.Sqrt(CharSet.Length));

                var wWidth = 0;
                var wIndex = 0;
                foreach (var wChar in CharSet)
                {
                    var wSize = wGraphics.MeasureString($"{wChar}", mFont, int.MaxValue, StringFormat.GenericDefault).ToSize();
                    mFontHeight = Math.Max(mFontHeight, wSize.Height);
                    wWidth = wWidth + wSize.Width;
                    if (wWidth > wLimit)
                    {
                        wWidth = 0;
                        wTable.Add(new List<Tuple<char, Vector2>> { });
                        wIndex++;
                    }
                    wTable[wIndex].Add(Tuple.Create(wChar, new Vector2(wSize.Width, wSize.Height)));
                }
            }

            // Set the location and size information for every character in the texture atlas
            var wLocation = new Vector2(0, 0);
            foreach (var wRow in wTable)
            {
                foreach (var wColumn in wRow)
                {
                    mLookUpTable[wColumn.Item1] = new FontTextureData { Location = wLocation, Size = wColumn.Item2 };
                    wLocation.X = wLocation.X + wColumn.Item2.X;
                }
                wLocation.X = 0;
                wLocation.Y = wLocation.Y + mFontHeight;
            }

            // Generate the bitmap containing the texture atlas
            var wMWidth = (int)wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.X));
            var wMHeight = wTable.Count * mFontHeight;
            var wCharacterSetImage = new Bitmap(wMWidth, wMHeight);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                foreach (var wTuple in mLookUpTable)
                {
                    wGraphics.DrawString($"{wTuple.Key}", mFont, new SolidBrush(Color.FromArgb(Color.R, Color.G, Color.B)), (int)wTuple.Value.Location.X, (int)wTuple.Value.Location.Y);
                }
                wGraphics.Flush();
            }

            // Create the texture from the bitmap
            mTexture?.Dispose();
            mTexture = new Texture
            {
                Width = wMWidth,
                Height = wMHeight
            };
            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
            var wData = wCharacterSetImage.LockBits(new Rectangle(0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, wCharacterSetImage.Width, wCharacterSetImage.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wCharacterSetImage.UnlockBits(wData);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Releases the OpenGL resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                mTexture?.Dispose();
                if (mVerticesId != 0) GL.DeleteBuffer(mVerticesId);
                if (mIndicesId != 0) GL.DeleteBuffer(mIndicesId);
                if (mTexCoordsId != 0) GL.DeleteBuffer(mTexCoordsId);
            }

            mDisposed = true;
        }

        /// <summary>
        /// Releases the OpenGL resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
