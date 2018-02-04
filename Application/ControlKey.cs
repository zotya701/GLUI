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
        private List<Key> mKeys = new List<Key>();

        public ControlKey(params Key[] keys)
        {
            foreach (var wKey in keys)
            {
                mKeys.Add(wKey);
            }
        }

        public ControlKey(ControlKey left, params Key[] keys)
        {
            mKeys = left.mKeys;
            mKeys.AddRange(keys);
        }

        public override string ToString()
        {
            return string.Join(" + ", mKeys.Select(key => key.RealKey));
        }

        public static implicit operator ControlKey(Key key)
        {
            return new ControlKey(key);
        }

        public static ControlKey operator +(ControlKey left, Key right)
        {
            return new ControlKey(left, right);
        }

        public static bool operator ==(KeyboardState left, ControlKey right)
        {
            var wFullFilled = true;
            foreach (var wKey in right.mKeys)
            {
                wFullFilled = wFullFilled && left.KeyDown[wKey.RealKey];
            }
            return wFullFilled;
        }

        public static bool operator ==(ControlKey left, KeyboardState right)
        {
            return right == left;
        }

        public static bool operator !=(KeyboardState left, ControlKey right)
        {
            return !(left == right);
        }

        public static bool operator !=(ControlKey left, KeyboardState right)
        {
            return right != left;
        }
    }
}
