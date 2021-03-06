﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation
{
    public partial class Key
    {
        public OpenTK.Input.Key RealKey { get; }

        private Key(OpenTK.Input.Key key) { RealKey = key; }

        public override string ToString() => RealKey.ToString();

        public static implicit operator Key(OpenTK.Input.Key key) => new Key(key);

        public static bool operator ==(Key left, Key right) => left.RealKey == right.RealKey;

        public static bool operator !=(Key left, Key right) => left.RealKey != right.RealKey;

        public override bool Equals(object obj) => RealKey == (obj as Key).RealKey;

        public override int GetHashCode() => RealKey.GetHashCode();
    }
}
