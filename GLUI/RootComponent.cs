using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GLUI
{
    public class RootComponent : Component
    {
        public void ListenTo(ref MouseHandler eventHandler)
        {
            eventHandler += MouseHandler;
        }
        public void ListenTo(ref KeyboardHandler eventHandler)
        {
            eventHandler += KeyboardHandler;
        }
        public new void MouseHandler(object sender, MouseState mouseState)
        {
            if (mouseState.IsPressed)
            {
                base.BringFront(mouseState);
            }
            base.MouseHandler(sender, mouseState);
        }
        public new void Render()
        {
            var wViewport = new int[4];
            GL.GetInteger(GetPName.Viewport, wViewport);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Viewport(wViewport[0], wViewport[1], wViewport[2], wViewport[3]);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Ortho(0, wViewport[2], wViewport[3], 0, 1, -1);

            base.Render();
        }
        public new void Update()
        {
            base.Update();
        }
        protected override void OnKeyboard(KeyboardState keyboardState) { }
        protected override void OnMouse(MouseState mouseState) { }
        protected override void OnRender() { }
        protected override void OnUpdate() { }
    }
}
