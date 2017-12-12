using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Collections.ObjectModel;

namespace GLUI
{
    public class Window : Component
    {
        private Component mPanel;

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

            //GL.Color3(BackgroundColor.R,
            //          BackgroundColor.G,
            //          BackgroundColor.B);
            //GL.Begin(BeginMode.Quads);
            //GL.Vertex2(AbsoluteLocation.X, AbsoluteLocation.Y);
            //GL.Vertex2(AbsoluteLocation.X + Width, AbsoluteLocation.Y);
            //GL.Vertex2(AbsoluteLocation.X + Width, AbsoluteLocation.Y + Height);
            //GL.Vertex2(AbsoluteLocation.X, AbsoluteLocation.Y + Height);
            //GL.End();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            Dirty = false;
        }
    }
}
