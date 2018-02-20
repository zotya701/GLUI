using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI
{
    public class Button : Component
    {
        private bool mDisposed = false;
        private Label mLabel = null;

        public Label Label
        {
            get
            {
                return mLabel;
            }
            set
            {
                if (mLabel == value) return;

                Children.Remove(mLabel);
                mLabel = value;
                Children.Add(mLabel);
            }
        }

        public string Text { get { return Label.Text; } set { if (Text == value) return; Label.Text = value; } }

        public Button(string text)
        {
            Highlightable = true;
            Label = new Label()
            {
                Alignment = new Alignment
                {
                    Vertical = Vertical.Center,
                    Horizontal = Horizontal.Center
                },
                FontFamily = "Arial",
                FontSize = 14,
                FontColor = Color.Black,
                Text = text
            };
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnUpdate()
        {
            Label.Size = Size;
            base.OnUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                //mFont?.Dispose();
            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
