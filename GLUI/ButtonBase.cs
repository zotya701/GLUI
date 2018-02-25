using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public abstract class ButtonBase : Component
    {
        private bool mDisposed = false;

        private Label mLabel = null;

        public bool Pressed { get; protected set; } = false;

        protected event EventHandler mButtonPressed;
        protected event EventHandler mButtonReleased;

        public event EventHandler ButtonClicked;

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
                mLabel?.Dispose();
                mLabel = value;
                Children.Add(mLabel);
            }
        }

        public string Text { get { return Label.Text; } set { if (Text == value) return; Label.Text = value; } }

        public ButtonBase()
        {
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
                Text = string.Empty
            };
        }

        protected virtual void OnPressed(object s, EventArgs e)
        {
            mButtonPressed?.Invoke(s, e);
        }

        protected virtual void OnReleased(object s, EventArgs e)
        {
            mButtonReleased?.Invoke(s, e);
        }

        protected virtual void OnClick(object s, EventArgs e)
        {
            ButtonClicked?.Invoke(s, e);
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if(mouseState.IsOverDirectly && mouseState.Button == OpenTK.Input.MouseButton.Left && mouseState.IsPressed)
            {
                OnPressed(this, new EventArgs());
                Pressed = true;
            }
            if (Pressed && mouseState.ButtonDown[OpenTK.Input.MouseButton.Left] == false)
            {
                OnReleased(this, new EventArgs());
                Pressed = false;
            }
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
                Label?.Dispose();
            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
