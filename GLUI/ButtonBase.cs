using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public abstract class ButtonBase : Component
    {
        private bool mDisposed = false;

        protected bool mPressed = false;

        public event EventHandler Pressed;
        public event EventHandler Released;

        public ButtonBase()
        {

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
                Console.WriteLine("Pressed");
                Pressed?.Invoke(this, new EventArgs());
                mPressed = true;
            }
            if (mPressed && mouseState.ButtonDown[OpenTK.Input.MouseButton.Left] == false)
            {
                Console.WriteLine("Released");
                Released?.Invoke(this, new EventArgs());
                mPressed = false;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                
            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
