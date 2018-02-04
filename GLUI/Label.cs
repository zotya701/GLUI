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
        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mTexCoordsId = 0;
        private int mIndicesCount = 0;

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
                Font.DrawText(Text);
            }
            else
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                GL.Disable(EnableCap.Lighting);
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, Font.TextureId);

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
        }

        protected override void OnUpdate()
        {
            var wSize = Font.MeasureText(Text);
            Size = new Size(Math.Max(Width, wSize.Width), Math.Max(Height, wSize.Width));

            if (Cached)
            {
                Raster.Location = AbsoluteLocation;

                var wVertices = new List<int>();
                var wIndices = new List<uint>();
                var wTexCoords = new List<float>();

                for (int i = 0; i < Text.Length; ++i)
                {
                    var wChar = Text[i];

                    wVertices.AddRange(Font.CalculateVertices(wChar).Select(wVertex => new int[] { wVertex.X, wVertex.Y }).SelectMany(wVertex => wVertex));
                    wTexCoords.AddRange(Font.CalculateTexCoords(wChar).Select(wVertex => new float[] { wVertex.X, wVertex.Y}).SelectMany(wVertex => wVertex));
                    wIndices.AddRange(new List<uint> { 0, 1, 2, 2, 3, 0 }.Select(wIndex => (uint)(wIndex + i * 4)));

                    Font.MoveRaster(wChar);
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
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(int) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
                GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.DynamicDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, mTexCoordsId);
                GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * wTexCoords.Count, wTexCoords.ToArray(), BufferUsageHint.DynamicDraw);
            }

            base.OnUpdate();
        }
    }
}
