using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation
{
    public class Command
    {
        public string Name;
        public List<Key> Keys { get; } = new List<Key>();

        public event EventHandler<EventArgs> Activated;

        public Command(string name, params Key[] keys)
        {
            Name = name;
            foreach (var wKey in keys) { Keys.Add(wKey); }
        }

        public Command Executes(Action action)
        {
            Activated += (o, e) => { action?.Invoke(); };
            return this;
        }

        public void Check(KeyboardState keyboardState)
        {
            if (Keys.All(wKey => keyboardState.KeyDown[wKey]))
            {
                if(Keys.Reverse<Key>().Zip(keyboardState.Last10Keys.Reverse<Key>(), (l, r) => l == r).All(value => value == true))
                {
                    Activated?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public override string ToString() => $"{Name} -> {string.Join(" + ", Keys)}";
    }
}
