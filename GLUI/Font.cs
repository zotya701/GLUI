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
        public int TextureId { get { return mTexture.Id; } }
        private Texture mTexture;
        private System.Drawing.Font mFont;
        static private string mCharSet;
        private Dictionary<char, CharTextureData> mLookUpTable;
        private int mFontHeight;
        private Point mOriginalRasterLocation;

        public Color Color { get; }
        public string FamilyName { get { return mFont.FontFamily.Name; } }
        public float Size { get { return mFont.Size; } }
        static public string CharSet
        {
            get
            {
                if (string.IsNullOrEmpty(mCharSet))
                {
                    //mCharSet = "";
                    //foreach (var wCharCode in Enumerable.Range(0, Byte.MaxValue + 1))
                    //{
                    //    if ((32 <= wCharCode && wCharCode <= 126) || (160 <= wCharCode && wCharCode <= Byte.MaxValue))
                    //    {
                    //        mCharSet = mCharSet + Convert.ToChar(wCharCode);
                    //    }
                    //    else
                    //    {
                    //        mCharSet = mCharSet + " ";
                    //    }
                    //}
                    mCharSet = "";
                    foreach (var wCharCode in Enumerable.Range(0, Byte.MaxValue + 1))
                    {
                        //if ((32 <= wCharCode && wCharCode <= 126) || (160 <= wCharCode && wCharCode <= Byte.MaxValue))
                        //{
                        //    mCharSet = mCharSet + Convert.ToChar(wCharCode);
                        //}
                        //else
                        //{
                        //    mCharSet = mCharSet + " ";
                        //}
                        mCharSet = mCharSet + Convert.ToChar(wCharCode);
                    }
                }
                return mCharSet;
            }
        }

        public Font(string familyName, float size, Color color)
        {
            mFont = new System.Drawing.Font(familyName, size, GraphicsUnit.Pixel);
            Color = color;
            GenerateTextureAtlas();
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

            MoveRaster(c);
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

            if (c == '\r')
            {
                Raster.Move(mOriginalRasterLocation.X - Raster.Location.X, 0);
            }
            else
            if (c == '\n')
            {
                Raster.Move(0, mFontHeight);
            }
            else
            {
                Raster.Move(mLookUpTable[c].Size.Width - (mFont.Height / 4), 0);
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

        private void GenerateTextureAtlas()
        {
            var wOffset = new Size(0, 0);
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
                    //var wSize = TextRenderer.MeasureText(wGraphics, $"{wChar}", mFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPrefix) + wOffset;
                    //var wSize = TextRenderer.MeasureText($"{wChar}", mFont, new Size(), TextFormatFlags.NoPrefix) + wOffset;
                    var wSize = wGraphics.MeasureString($"{wChar}", mFont, int.MaxValue, StringFormat.GenericDefault).ToSize() + wOffset;
                    mFontHeight = Math.Max(mFontHeight, wSize.Height);
                    //var wHeight = wTable.Count * mFont.Height;
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
            var wLocation = new Point(0, 0) + wOffset;
            foreach (var wRow in wTable)
            {
                foreach (var wColumn in wRow)
                {
                    mLookUpTable[wColumn.Item1] = new CharTextureData { Location = wLocation, Size = wColumn.Item2 - wOffset };
                    wLocation.X = wLocation.X + wColumn.Item2.Width;
                }
                wLocation.X = wOffset.Width;
                //wLocation.Y = wLocation.Y + mFont.Height + wOffset.Height;
                wLocation.Y = wLocation.Y + mFontHeight + wOffset.Height;
            }

            // Generate the bitmap containing the texture atlas
            var wMWidth = wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.Width)) + wOffset.Width;
            //var wMHeight = wTable.Count * (mFont.Height + wOffset.Height) + wOffset.Height;
            var wMHeight = wTable.Count * (mFontHeight + wOffset.Height) + wOffset.Height;
            var wCharacterSetImage = new Bitmap(wMWidth, wMHeight);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            {
                wGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                foreach (var wTuple in mLookUpTable)
                {
                    //TextRenderer.DrawText(wGraphics, $"{wTuple.Key}", mFont, wTuple.Value.Location, Color.FromArgb(Color.R, Color.G, Color.B), TextFormatFlags.NoPrefix);
                    wGraphics.DrawString($"{wTuple.Key}", mFont, new SolidBrush(Color.FromArgb(Color.R, Color.G, Color.B)), wTuple.Value.Location);
                }
                wGraphics.Flush();
            }
            wCharacterSetImage.Save("ASD.bmp");

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
