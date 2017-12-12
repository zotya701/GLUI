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
            keys.ToList().ForEach(arg => mKeys.Add(arg));
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
            right.mKeys.ForEach(key => wFullFilled = wFullFilled && left.KeyDown[key.RealKey]);
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
