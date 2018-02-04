using GLUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class ControlKey
    {
        public string Name { get; }
        public List<Key> Keys { get; } = new List<Key>();

        public event EventHandler<ControlKey> Activated;

        public ControlKey(params Key[] keys)
        {
            foreach (var wKey in keys)
            {
                Keys.Add(wKey);
            }
        }

        public ControlKey(ControlKey left, params Key[] keys)
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
                Activated?.Invoke(this, this);
            }
        }

        public override string ToString()
        {
            return string.Join(" + ", Keys.Select(key => key.RealKey));
        }

        public static implicit operator ControlKey(Key key)
        {
            return new ControlKey(key);
        }

        public static ControlKey operator +(ControlKey left, Key right)
        {
            return new ControlKey(left, right);
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
