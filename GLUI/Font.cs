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
        private class CharTextureData
        {
            public Point Location { get; set; }
            public Size Size { get; set; }
        }

        private int mTexId;
        private System.Drawing.Font mFont;
        static private string mCharSet;
        private Dictionary<char, CharTextureData> mLookUpTable;

        public Color Color { get; }
        public string FamilyName { get { return mFont.FontFamily.Name; } }
        public float Size { get { return mFont.Size; } }
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

        public Font(string familyName, float size, Color color)
        {
            mFont = new System.Drawing.Font(familyName, size, GraphicsUnit.Pixel);
            Color = color;
            mTexId = 0;
            GenerateTextureAtlas();
        }

        public void SaveCharacterSet()
        {
            // Create bitmap from the texture
            if (mTexId == 0)
            {
                return;
            }
            int wWidth;
            int wHeight;
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out wWidth);
            GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out wHeight);
            var wCharacterSetImage = new Bitmap(wWidth, wHeight);
            var wData = wCharacterSetImage.LockBits(new Rectangle(0, 0, wCharacterSetImage.Width, wCharacterSetImage.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wCharacterSetImage.UnlockBits(wData);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            using (var wPen = new Pen(Color.FromArgb(Color.R, Color.G, Color.B)))
            {
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                mLookUpTable.ToList().ForEach(wTuple => wGraphics.DrawRectangle(wPen, new Rectangle(wTuple.Value.Location, wTuple.Value.Size)));
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
            var wOffset = new Size(5, 5);
            mLookUpTable = new Dictionary<char, CharTextureData>();
            var wTable = new List<List<Tuple<char, Size>>> { new List<Tuple<char, Size>>() };

            // Order the characters to fit into the possible smallest square like shape
            foreach (var wChar in CharSet.Distinct())
            {
                var wSize = TextRenderer.MeasureText($"{wChar}", mFont, new Size(), TextFormatFlags.NoPrefix) + wOffset;
                var wHeight = wTable.Count * mFont.Height;
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
                wLocation.Y = wLocation.Y + mFont.Height + wOffset.Height;
            }

            // Generate the bitmap containing the texture atlas
            var wMWidth = wTable.Max(wRow => wRow.Sum(wColumn => wColumn.Item2.Width)) + wOffset.Width;
            var wMHeight = wTable.Count * (mFont.Height + wOffset.Height) + wOffset.Height;
            var wCharacterSetImage = new Bitmap(wMWidth, wMHeight);
            using (var wGraphics = Graphics.FromImage(wCharacterSetImage))
            {
                wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                mLookUpTable.ToList().ForEach(wTuple => TextRenderer.DrawText(wGraphics, $"{wTuple.Key}", mFont, wTuple.Value.Location, Color.FromArgb(Color.R, Color.G, Color.B), TextFormatFlags.NoPrefix));
                wGraphics.Flush();
            }

            // Create the texture from the bitmap
            if (mTexId != 0)
            {
                GL.DeleteTexture(mTexId);
            }
            mTexId = GL.GenTexture();
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            GL.BindTexture(TextureTarget.Texture2D, mTexId);
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
