using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation.MathExtension
{
    public class Vector2 : Vector
    {
        public static Vector2 UnitX { get; } = Unit(0, 2) as Vector2;
        public static Vector2 UnitY { get; } = Unit(1, 2) as Vector2;

        public double X
        {
            get { return this[0]; }
            set { this[0] = value; }
        }
        public double Y
        {
            get { return this[1]; }
            set { this[1] = value; }
        }

        public Vector2(double X = 0, double Y = 0) : base(2)
        {
            this.X = X;
            this.Y = Y;
        }

        public Vector2 Rotate(double angle)
        {
            return new Vector2(X = X * Math.Cos(angle) - Y * Math.Sin(angle),
                               Y = X * Math.Sin(angle) + Y * Math.Cos(angle));
        }
    }
}
