using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation
{
    public class Command
    {
        public List<Key> Keys { get; } = new List<Key>();

        public event EventHandler<Command> Activated;

        public Command(params Key[] keys)
        {
            foreach (var wKey in keys)
            {
                Keys.Add(wKey);
            }
        }

        public Command(Command left, params Key[] keys)
        {
            Keys = left.Keys;
            Keys.AddRange(keys);
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
            return string.Join(" + ", Keys.Select(key => key.RealKey));
        }

        public static implicit operator Command(Key key)
        {
            return new Command(key);
        }

        public static Command operator +(Command left, Key right)
        {
            return new Command(left, right);
        }

        //public static bool operator ==(KeyboardState left, ControlKey right)
        //{
        //    var wFullFilled = true;
        //    foreach (var wKey in right.Keys)
        //    {
        //        wFullFilled = wFullFilled && left.KeyDown[wKey.RealKey];
        //    }
        //    return wFullFilled;
        //}

        //public static bool operator ==(ControlKey left, KeyboardState right)
        //{
        //    return right == left;
        //}

        //public static bool operator !=(KeyboardState left, ControlKey right)
        //{
        //    return !(left == right);
        //}

        //public static bool operator !=(ControlKey left, KeyboardState right)
        //{
        //    return right != left;
        //}
    }
}
