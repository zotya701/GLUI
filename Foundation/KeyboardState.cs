using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation
{
    public delegate void KeyboardHandler(object sender, KeyboardState keyboardState);
    public class KeyboardState : EventArgs
    {
        public bool Alt { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public char KeyChar { get; set; }
        public bool IsPressed { get; set; }
        public Dictionary<OpenTK.Input.Key, bool> KeyDown { get; } = Enum.GetValues(typeof(OpenTK.Input.Key)).Cast<OpenTK.Input.Key>().Distinct().ToDictionary(key => (OpenTK.Input.Key)key, key => false);
    }
}
