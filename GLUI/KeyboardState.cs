using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI
{
    public delegate void KeyboardHandler(object sender, GLUI.KeyboardState keyboardState);
    public class KeyboardState : EventArgs
    {
        public bool Alt { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public char KeyChar { get; set; }
        public bool IsPressed { get; set; }
        public Dictionary<Key, bool> KeyDown { get; } = Enum.GetValues(typeof(Key)).Cast<Key>().Distinct().ToDictionary(key => (Key)key, key => false);
    }
}
