using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public class Font
    {
        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mTexCoordsId = 0;
        private int mIndicesCount = 0;
        private Texture mTexture;
        private System.Drawing.Font mFont;
        private int mFontHeight;
        private Dictionary<char, CharTextureData> mLookUpTable;
        private Point mOriginalRasterLocation;
        private static string mCharSet;

        public Color Color { get; }
        public string FamilyName { get { return mFont.FontFamily.Name; } }
        public int Size { get { return mFontHeight; } }
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

        public Font(string familyName, int size, Color color)
        {
            mFont = new System.Drawing.Font(familyName, size, GraphicsUnit.Pixel);
            Color = color;
            GenerateFontMap();
        }

        public void DrawText(string text)
        {
            mOriginalRasterLocation = Raster.Location;
            foreach (var wChar in text)
            {
                DrawChar(wChar);
            }
        }

        public void DrawChar(char c)
        {
            if (c != '\r' && c != '\n')
            {
                var wVertices = CalculateVertices(c);
                var wTexCoords = CalculateTexCoords(c);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Disable(EnableCap.Lighting);
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(wTexCoords[0].X, wTexCoords[0].Y); GL.Vertex2(wVertices[0].X, wVertices[0].Y);
                GL.TexCoord2(wTexCoords[1].X, wTexCoords[1].Y); GL.Vertex2(wVertices[1].X, wVertices[1].Y);
                GL.TexCoord2(wTexCoords[2].X, wTexCoords[2].Y); GL.Vertex2(wVertices[2].X, wVertices[2].Y);
                GL.TexCoord2(wTexCoords[3].X, wTexCoords[3].Y); GL.Vertex2(wVertices[3].X, wVertices[3].Y);
                GL.End();
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            MoveRaster(c);
        }

        public void DrawCachedText()
        {
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.Texture2D);
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

        public void RegenerateTextCache(string text)
        {
            mOriginalRasterLocation = Raster.Location;

            var wVertices = new List<int>();
            var wIndices = new List<uint>();
            var wTexCoords = new List<float>();

            for (int i = 0; i < text.Length; ++i)
            {
                var wChar = text[i];

                if (wChar == 's')
                {
                    var asd = 0;
                }

                //if (wChar != '\r' && wChar != '\n')
                //{
                wVertices.AddRange(CalculateVertices(wChar).Select(wVertex => new int[] { wVertex.X, wVertex.Y }).SelectMany(wVertex => wVertex));
                wTexCoords.AddRange(CalculateTexCoords(wChar).Select(wVertex => new float[] { wVertex.X, wVertex.Y }).SelectMany(wVertex => wVertex));
                wIndices.AddRange(new List<uint> { 0, 1, 2, 2, 3, 0 }.Select(wIndex => (uint)(wIndex + i * 4)));
                //}

                MoveRaster(wChar);
            }


            if (mVerticesId != 0)
            {
                GL.DeleteBuffer(mVerticesId);
            }
            if (mIndicesId != 0)
            {
                GL.DeleteBuffer(mIndicesId);
            }
            if (mTexCoordsId != 0)
            {
                GL.DeleteBuffer(mTexCoordsId);
            }

            mVerticesId = GL.GenBuffer();
            mIndicesId = GL.GenBuffer();
            mTexCoordsId = GL.GenBuffer();
            mIndicesCount = wIndices.Count;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(int) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTexCoordsId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * wTexCoords.Count, wTexCoords.ToArray(), BufferUsageHint.StaticDraw);
        }

        public PointF[] CalculateTexCoords(char c)
        {
            var wX = (float)mLookUpTable[c].Location.X / mTexture.Width;
            var wY = (float)mLookUpTable[c].Location.Y / mTexture.Height;
            var wWidth = (float)mLookUpTable[c].Size.Width / mTexture.Width;
            var wHeight = (float)mLookUpTable[c].Size.Height / mTexture.Height;

            var wTP1 = new PointF(wX, wY);
            var wTP2 = new PointF(wTP1.X + wWidth, wTP1.Y);
            var wTP3 = new PointF(wTP2.X, wTP1.Y + wHeight);
            var wTP4 = new PointF(wTP1.X, wTP3.Y);

            return new PointF[] { wTP1, wTP2, wTP3, wTP4 };
        }

        public Point[] CalculateVertices(char c)
        {
            var wX = mLookUpTable[c].Location.X;
            var wY = mLookUpTable[c].Location.Y;
            var wWidth = mLookUpTable[c].Size.Width;
            var wHeight = mLookUpTable[c].Size.Height;

            var wP1 = new Point(Raster.Location.X, Raster.Location.Y);
            var wP2 = new Point(wP1.X + wWidth, wP1.Y);
            var wP3 = new Point(wP2.X, wP1.Y + wHeight);
            var wP4 = new Point(wP1.X, wP3.Y);

            return new Point[] { wP1, wP2, wP3, wP4 };
        }

        public void MoveRaster(char c)
        {
            if (c == ' ')
            {
                Raster.Move(mLookUpTable[c].Size.Width, 0);
            }

            switch (c)
            {
                case '\r': Raster.Move(mOriginalRasterLocation.X - Raster.Location.X, 0); break;
                case '\n': Raster.Move(0, mFontHeight); break;
                default: Raster.Move(mLookUpTable[c].Size.Width - (mFont.Height / 4), 0); break;
            }
        }

        public Size MeasureText(string text)
        {
            var wRasterLocation = Raster.Location;
            var wBottomRight = new Point(0, 0);

            foreach (var wChar in text)
            {
                MoveRaster(wChar);
                wBottomRight.X = Math.Max(wBottomRight.X, Raster.Location.X + mFontHeight / 4);
                wBottomRight.Y = Math.Max(wBottomRight.Y, Raster.Location.Y + mFontHeight);
            }

            var wSize = new Size(wBottomRight.X - wRasterLocation.X, wBottomRight.Y - wRasterLocation.Y);

            Raster.Location = wRasterLocation;

            return wSize;
        }

        public void SaveCharacterSet()
        {
            // Create bitmap from the texture
            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
            var wCharacterSetImage = new Bitmap(mTexture.Width, mTexture.Height);
            var wData = wCharacterSetImage.LockBits(new Rectangle(0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wCharacterSetImage.UnlockBits(wData);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            using (var wPen = new Pen(Color.FromArgb(Color.R, Color.G, Color.B)))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                foreach (var wTuple in mLookUpTable)
                {
                    wGraphics.DrawRectangle(wPen, new Rectangle(wTuple.Value.Location, wTuple.Value.Size));
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

        private void GenerateFontMap()
        {
            mLookUpTable = new Dictionary<char, CharTextureData>();
            var wTable = new List<List<Tuple<char, Size>>> { new List<Tuple<char, Size>>() };
            mFontHeight = 0;

            // Order the characters to fit into the possible smallest square like shape
            using (var wGraphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                foreach (var wChar in CharSet.Distinct())
                {
                    var wSize = wGraphics.MeasureString($"{wChar}", mFont, int.MaxValue, StringFormat.GenericDefault).ToSize();
                    mFontHeight = Math.Max(mFontHeight, wSize.Height);
                    var wHeight = wTable.Count * wSize.Height;
                    var wMaxWidth = wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.Width));
                    var wMinWidth = wTable.Min(wRow => wRow.Sum(wColumn => wColumn.Item2.Width));
                    if (wMinWidth + wSize.Width < wHeight)
                    {
                        (wTable.FirstOrDefault(wRow => wRow.Sum(wColumn => wColumn.Item2.Width) == wMinWidth) ?? wTable.First()).Add(Tuple.Create(wChar, wSize));
                    }
                    else
                    {
                        wTable.Add(new List<Tuple<char, Size>> { Tuple.Create(wChar, wSize) });
                    }
                }
            }

            // Set the location and size information for every character in the texture atlas
            var wLocation = new Point(0, 0);
            foreach (var wRow in wTable)
            {
                foreach (var wColumn in wRow)
                {
                    mLookUpTable[wColumn.Item1] = new CharTextureData { Location = wLocation, Size = wColumn.Item2 };
                    wLocation.X = wLocation.X + wColumn.Item2.Width;
                }
                wLocation.X = 0;
                wLocation.Y = wLocation.Y + mFontHeight;
            }

            // Generate the bitmap containing the texture atlas
            var wMWidth = wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.Width));
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
                    wGraphics.DrawString($"{wTuple.Key}", mFont, new SolidBrush(Color.FromArgb(Color.R, Color.G, Color.B)), wTuple.Value.Location);
                }
                wGraphics.Flush();
            }

            // Create the texture from the bitmap
            mTexture = new Texture
            {
                Width = wMWidth,
                Height = wMHeight
            };
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.BindTexture(TextureTarget.Texture2D, mTexture.Id);
            var wData = wCharacterSetImage.LockBits(new Rectangle(0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, wCharacterSetImage.Width, wCharacterSetImage.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wCharacterSetImage.UnlockBits(wData);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        }
    }
}
