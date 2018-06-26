using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation
{
    public class Command
    {
        public string Name;
        public List<Key> Keys { get; } = new List<Key>();

        public event EventHandler<Command> Activated;

        public Command(string name, params Key[] keys)
        {
            Name = name;
            foreach (var wKey in keys)
            {
                Keys.Add(wKey);
            }
        }

        public Command Executes(Action action)
        {
            Activated += (o, e) =>
            {
                action?.Invoke();
            };
            return this;
        }

        public void Check(KeyboardState keyboardState)
        {
            var wFullFilled = true;
            foreach (var wKey in Keys)
            {
                wFullFilled = wFullFilled && keyboardState.KeyDown[wKey.RealKey];
            }
            if (wFullFilled)
            {
                var wInOrder = true;
                var wKeys = Keys.ToList();
                var wLast10Keys = keyboardState.Last10Keys.ToList();
                wKeys.Reverse();
                wLast10Keys.Reverse();
                for (int i = 0; i < Math.Min(wKeys.Count, wLast10Keys.Count); ++i)
                {
                    wInOrder = wInOrder && wKeys[i] == wLast10Keys[i];
                }
                if(wInOrder == wKeys.Count >= Math.Min(wKeys.Count, wLast10Keys.Count))
                {
                    Activated?.Invoke(this, this);
                }
            }
        }

        public override string ToString()
        {
            return $"{Name} -> {string.Join(" + ", Keys)}";
        }
    }
}
