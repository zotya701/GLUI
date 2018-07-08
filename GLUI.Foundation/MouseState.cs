using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation
{
    public delegate void MouseHandler(object sender, MouseState mouseState);
    public class MouseState : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsOver { get; set; }
        public bool IsOverDirectly { get; set; }
        public int XDelta { get; set; }
        public int YDelta { get; set; }
        public bool IsMoved { get; set; }
        public MouseButton Button { get; set; }
        public bool IsPressed { get; set; }
        public int Delta { get; set; }
        public bool IsScrolled { get; set; }
        public Dictionary<MouseButton, bool> ButtonDown { get; } = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>().Distinct().ToDictionary(button => (MouseButton)button, button => false);
    }
}
