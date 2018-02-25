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
        public List<OpenTK.Input.Key> Last10Keys = new List<OpenTK.Input.Key>();

        public bool Alt { get; set; }
        public bool Control { get; set; }
        public bool Shift { get; set; }
        public char KeyChar { get; set; }
        public bool IsPressed { get; set; }
        public Dictionary<OpenTK.Input.Key, bool> KeyDown { get; } = Enum.GetValues(typeof(OpenTK.Input.Key)).Cast<OpenTK.Input.Key>().Distinct().ToDictionary(key => (OpenTK.Input.Key)key, key => false);
        public OpenTK.Input.Key LastKey
        {
            get
            {
                return Last10Keys.LastOrDefault();
            }
            set
            {
                Last10Keys.Add(value);
                if(Last10Keys.Count > 10)
                {
                    Last10Keys.RemoveAt(0);
                }
            }
        }
    }
}
