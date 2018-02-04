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
            //GL.Enable(EnableCap.Texture2D);
            //GL.BindTexture(TextureTarget.Texture2D, mFontTexture);
            //GL.Begin(BeginMode.Quads);
            //GL.TexCoord2(0, 0);
            //GL.Vertex2(0, 0);
            //GL.TexCoord2(1, 0);
            //GL.Vertex2(200, 0);
            //GL.TexCoord2(1, 1);
            //GL.Vertex2(200, 200);
            //GL.TexCoord2(0, 1);
            //GL.Vertex2(0, 200);
            //GL.End();
        }

        protected override void OnUpdate()
        {
            if (Dirty)
            {
                base.OnUpdate();
            }
        }
    }
}
