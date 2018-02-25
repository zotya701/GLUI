using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation
{
    public class Key
    {
        public OpenTK.Input.Key RealKey { get; private set; }

        public static Key Unknown { get { return new Key(OpenTK.Input.Key.Unknown); } }
        public static Key ShiftLeft { get { return new Key(OpenTK.Input.Key.ShiftLeft); } }
        public static Key LShift { get { return new Key(OpenTK.Input.Key.LShift); } }
        public static Key ShiftRight { get { return new Key(OpenTK.Input.Key.ShiftRight); } }
        public static Key RShift { get { return new Key(OpenTK.Input.Key.RShift); } }
        public static Key ControlLeft { get { return new Key(OpenTK.Input.Key.ControlLeft); } }
        public static Key LControl { get { return new Key(OpenTK.Input.Key.LControl); } }
        public static Key ControlRight { get { return new Key(OpenTK.Input.Key.ControlRight); } }
        public static Key RControl { get { return new Key(OpenTK.Input.Key.RControl); } }
        public static Key AltLeft { get { return new Key(OpenTK.Input.Key.AltLeft); } }
        public static Key LAlt { get { return new Key(OpenTK.Input.Key.LAlt); } }
        public static Key AltRight { get { return new Key(OpenTK.Input.Key.AltRight); } }
        public static Key RAlt { get { return new Key(OpenTK.Input.Key.RAlt); } }
        public static Key WinLeft { get { return new Key(OpenTK.Input.Key.WinLeft); } }
        public static Key LWin { get { return new Key(OpenTK.Input.Key.LWin); } }
        public static Key WinRight { get { return new Key(OpenTK.Input.Key.WinRight); } }
        public static Key RWin { get { return new Key(OpenTK.Input.Key.RWin); } }
        public static Key Menu { get { return new Key(OpenTK.Input.Key.Menu); } }
        public static Key F1 { get { return new Key(OpenTK.Input.Key.F1); } }
        public static Key F2 { get { return new Key(OpenTK.Input.Key.F2); } }
        public static Key F3 { get { return new Key(OpenTK.Input.Key.F3); } }
        public static Key F4 { get { return new Key(OpenTK.Input.Key.F4); } }
        public static Key F5 { get { return new Key(OpenTK.Input.Key.F5); } }
        public static Key F6 { get { return new Key(OpenTK.Input.Key.F6); } }
        public static Key F7 { get { return new Key(OpenTK.Input.Key.F7); } }
        public static Key F8 { get { return new Key(OpenTK.Input.Key.F8); } }
        public static Key F9 { get { return new Key(OpenTK.Input.Key.F9); } }
        public static Key F10 { get { return new Key(OpenTK.Input.Key.F10); } }
        public static Key F11 { get { return new Key(OpenTK.Input.Key.F11); } }
        public static Key F12 { get { return new Key(OpenTK.Input.Key.F12); } }
        public static Key F13 { get { return new Key(OpenTK.Input.Key.F13); } }
        public static Key F14 { get { return new Key(OpenTK.Input.Key.F14); } }
        public static Key F15 { get { return new Key(OpenTK.Input.Key.F15); } }
        public static Key F16 { get { return new Key(OpenTK.Input.Key.F16); } }
        public static Key F17 { get { return new Key(OpenTK.Input.Key.F17); } }
        public static Key F18 { get { return new Key(OpenTK.Input.Key.F18); } }
        public static Key F19 { get { return new Key(OpenTK.Input.Key.F19); } }
        public static Key F20 { get { return new Key(OpenTK.Input.Key.F20); } }
        public static Key F21 { get { return new Key(OpenTK.Input.Key.F21); } }
        public static Key F22 { get { return new Key(OpenTK.Input.Key.F22); } }
        public static Key F23 { get { return new Key(OpenTK.Input.Key.F23); } }
        public static Key F24 { get { return new Key(OpenTK.Input.Key.F24); } }
        public static Key F25 { get { return new Key(OpenTK.Input.Key.F25); } }
        public static Key F26 { get { return new Key(OpenTK.Input.Key.F26); } }
        public static Key F27 { get { return new Key(OpenTK.Input.Key.F27); } }
        public static Key F28 { get { return new Key(OpenTK.Input.Key.F28); } }
        public static Key F29 { get { return new Key(OpenTK.Input.Key.F29); } }
        public static Key F30 { get { return new Key(OpenTK.Input.Key.F30); } }
        public static Key F31 { get { return new Key(OpenTK.Input.Key.F31); } }
        public static Key F32 { get { return new Key(OpenTK.Input.Key.F32); } }
        public static Key F33 { get { return new Key(OpenTK.Input.Key.F33); } }
        public static Key F34 { get { return new Key(OpenTK.Input.Key.F34); } }
        public static Key F35 { get { return new Key(OpenTK.Input.Key.F35); } }
        public static Key Up { get { return new Key(OpenTK.Input.Key.Up); } }
        public static Key Down { get { return new Key(OpenTK.Input.Key.Down); } }
        public static Key Left { get { return new Key(OpenTK.Input.Key.Left); } }
        public static Key Right { get { return new Key(OpenTK.Input.Key.Right); } }
        public static Key Enter { get { return new Key(OpenTK.Input.Key.Enter); } }
        public static Key Escape { get { return new Key(OpenTK.Input.Key.Escape); } }
        public static Key Space { get { return new Key(OpenTK.Input.Key.Space); } }
        public static Key Tab { get { return new Key(OpenTK.Input.Key.Tab); } }
        public static Key BackSpace { get { return new Key(OpenTK.Input.Key.BackSpace); } }
        public static Key Back { get { return new Key(OpenTK.Input.Key.Back); } }
        public static Key Insert { get { return new Key(OpenTK.Input.Key.Insert); } }
        public static Key Delete { get { return new Key(OpenTK.Input.Key.Delete); } }
        public static Key PageUp { get { return new Key(OpenTK.Input.Key.PageUp); } }
        public static Key PageDown { get { return new Key(OpenTK.Input.Key.PageDown); } }
        public static Key Home { get { return new Key(OpenTK.Input.Key.Home); } }
        public static Key End { get { return new Key(OpenTK.Input.Key.End); } }
        public static Key CapsLock { get { return new Key(OpenTK.Input.Key.CapsLock); } }
        public static Key ScrollLock { get { return new Key(OpenTK.Input.Key.ScrollLock); } }
        public static Key PrintScreen { get { return new Key(OpenTK.Input.Key.PrintScreen); } }
        public static Key Pause { get { return new Key(OpenTK.Input.Key.Pause); } }
        public static Key NumLock { get { return new Key(OpenTK.Input.Key.NumLock); } }
        public static Key Clear { get { return new Key(OpenTK.Input.Key.Clear); } }
        public static Key Sleep { get { return new Key(OpenTK.Input.Key.Sleep); } }
        public static Key Keypad0 { get { return new Key(OpenTK.Input.Key.Keypad0); } }
        public static Key Keypad1 { get { return new Key(OpenTK.Input.Key.Keypad1); } }
        public static Key Keypad2 { get { return new Key(OpenTK.Input.Key.Keypad2); } }
        public static Key Keypad3 { get { return new Key(OpenTK.Input.Key.Keypad3); } }
        public static Key Keypad4 { get { return new Key(OpenTK.Input.Key.Keypad4); } }
        public static Key Keypad5 { get { return new Key(OpenTK.Input.Key.Keypad5); } }
        public static Key Keypad6 { get { return new Key(OpenTK.Input.Key.Keypad6); } }
        public static Key Keypad7 { get { return new Key(OpenTK.Input.Key.Keypad7); } }
        public static Key Keypad8 { get { return new Key(OpenTK.Input.Key.Keypad8); } }
        public static Key Keypad9 { get { return new Key(OpenTK.Input.Key.Keypad9); } }
        public static Key KeypadDivide { get { return new Key(OpenTK.Input.Key.KeypadDivide); } }
        public static Key KeypadMultiply { get { return new Key(OpenTK.Input.Key.KeypadMultiply); } }
        public static Key KeypadSubtract { get { return new Key(OpenTK.Input.Key.KeypadSubtract); } }
        public static Key KeypadMinus { get { return new Key(OpenTK.Input.Key.KeypadMinus); } }
        public static Key KeypadAdd { get { return new Key(OpenTK.Input.Key.KeypadAdd); } }
        public static Key KeypadPlus { get { return new Key(OpenTK.Input.Key.KeypadPlus); } }
        public static Key KeypadDecimal { get { return new Key(OpenTK.Input.Key.KeypadDecimal); } }
        public static Key KeypadPeriod { get { return new Key(OpenTK.Input.Key.KeypadPeriod); } }
        public static Key KeypadEnter { get { return new Key(OpenTK.Input.Key.KeypadEnter); } }
        public static Key A { get { return new Key(OpenTK.Input.Key.A); } }
        public static Key B { get { return new Key(OpenTK.Input.Key.B); } }
        public static Key C { get { return new Key(OpenTK.Input.Key.C); } }
        public static Key D { get { return new Key(OpenTK.Input.Key.D); } }
        public static Key E { get { return new Key(OpenTK.Input.Key.E); } }
        public static Key F { get { return new Key(OpenTK.Input.Key.F); } }
        public static Key G { get { return new Key(OpenTK.Input.Key.G); } }
        public static Key H { get { return new Key(OpenTK.Input.Key.H); } }
        public static Key I { get { return new Key(OpenTK.Input.Key.I); } }
        public static Key J { get { return new Key(OpenTK.Input.Key.J); } }
        public static Key K { get { return new Key(OpenTK.Input.Key.K); } }
        public static Key L { get { return new Key(OpenTK.Input.Key.L); } }
        public static Key M { get { return new Key(OpenTK.Input.Key.M); } }
        public static Key N { get { return new Key(OpenTK.Input.Key.N); } }
        public static Key O { get { return new Key(OpenTK.Input.Key.O); } }
        public static Key P { get { return new Key(OpenTK.Input.Key.P); } }
        public static Key Q { get { return new Key(OpenTK.Input.Key.Q); } }
        public static Key R { get { return new Key(OpenTK.Input.Key.R); } }
        public static Key S { get { return new Key(OpenTK.Input.Key.S); } }
        public static Key T { get { return new Key(OpenTK.Input.Key.T); } }
        public static Key U { get { return new Key(OpenTK.Input.Key.U); } }
        public static Key V { get { return new Key(OpenTK.Input.Key.V); } }
        public static Key W { get { return new Key(OpenTK.Input.Key.W); } }
        public static Key X { get { return new Key(OpenTK.Input.Key.X); } }
        public static Key Y { get { return new Key(OpenTK.Input.Key.Y); } }
        public static Key Z { get { return new Key(OpenTK.Input.Key.Z); } }
        public static Key Number0 { get { return new Key(OpenTK.Input.Key.Number0); } }
        public static Key Number1 { get { return new Key(OpenTK.Input.Key.Number1); } }
        public static Key Number2 { get { return new Key(OpenTK.Input.Key.Number2); } }
        public static Key Number3 { get { return new Key(OpenTK.Input.Key.Number3); } }
        public static Key Number4 { get { return new Key(OpenTK.Input.Key.Number4); } }
        public static Key Number5 { get { return new Key(OpenTK.Input.Key.Number5); } }
        public static Key Number6 { get { return new Key(OpenTK.Input.Key.Number6); } }
        public static Key Number7 { get { return new Key(OpenTK.Input.Key.Number7); } }
        public static Key Number8 { get { return new Key(OpenTK.Input.Key.Number8); } }
        public static Key Number9 { get { return new Key(OpenTK.Input.Key.Number9); } }
        public static Key Tilde { get { return new Key(OpenTK.Input.Key.Tilde); } }
        public static Key Grave { get { return new Key(OpenTK.Input.Key.Grave); } }
        public static Key Minus { get { return new Key(OpenTK.Input.Key.Minus); } }
        public static Key Plus { get { return new Key(OpenTK.Input.Key.Plus); } }
        public static Key BracketLeft { get { return new Key(OpenTK.Input.Key.BracketLeft); } }
        public static Key LBracket { get { return new Key(OpenTK.Input.Key.LBracket); } }
        public static Key BracketRight { get { return new Key(OpenTK.Input.Key.BracketRight); } }
        public static Key RBracket { get { return new Key(OpenTK.Input.Key.RBracket); } }
        public static Key Semicolon { get { return new Key(OpenTK.Input.Key.Semicolon); } }
        public static Key Quote { get { return new Key(OpenTK.Input.Key.Quote); } }
        public static Key Comma { get { return new Key(OpenTK.Input.Key.Comma); } }
        public static Key Period { get { return new Key(OpenTK.Input.Key.Period); } }
        public static Key Slash { get { return new Key(OpenTK.Input.Key.Slash); } }
        public static Key BackSlash { get { return new Key(OpenTK.Input.Key.BackSlash); } }
        public static Key NonUSBackSlash { get { return new Key(OpenTK.Input.Key.NonUSBackSlash); } }
        public static Key LastKey { get { return new Key(OpenTK.Input.Key.LastKey); } }


        private Key(OpenTK.Input.Key key)
        {
            RealKey = key;
        }

        public static Command operator +(Key left, Key right)
        {
            return new Command(left, right);
        }

        public static bool operator ==(OpenTK.Input.Key left, Key right)
        {
            return left == right.RealKey;
        }

        public static bool operator !=(OpenTK.Input.Key left, Key right)
        {
            return left != right.RealKey;
        }

        public static bool operator ==(Key left, OpenTK.Input.Key right)
        {
            return left.RealKey == right;
        }

        public static bool operator !=(Key left, OpenTK.Input.Key right)
        {
            return left.RealKey != right;
        }
    }
}
