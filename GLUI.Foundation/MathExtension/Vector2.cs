using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation.MathExtension
{
    public class Vector2 : Vector
    {
        public static Vector2 UnitX { get; } = new Vector2(Unit(0, 2));
        public static Vector2 UnitY { get; } = new Vector2(Unit(1, 2));
        public new static Vector2 Zero { get; } = new Vector2(Zero(2));

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

        public Vector2(Vector vector) : base(vector) { }

        public static implicit operator Vector2((double X, double Y) tuple) => new Vector2(tuple.X, tuple.Y);

        public void Deconstruct(out double X, out double Y) { X = this.X; Y = this.Y; }

        public new Vector2 Normalize() => new Vector2(base.Normalize());

        public static Vector2 operator +(Vector2 vector, Vector2 value) => new Vector2((vector as Vector) + (value as Vector));

        public static Vector2 operator +(Vector2 vector, double value) => new Vector2((vector as Vector) + value);

        public static Vector2 operator +(double value, Vector2 vector) => new Vector2(value + (vector as Vector));

        public static Vector2 operator -(Vector2 vector, Vector2 value) => new Vector2((vector as Vector) - (value as Vector));

        public static Vector2 operator -(Vector2 vector, double value) => new Vector2((vector as Vector) - value);

        public static Vector2 operator -(double value, Vector2 vector) => new Vector2(value - (vector as Vector));

        public static Vector2 operator *(Vector2 vector, double value) => new Vector2((vector as Vector) * value);

        public static Vector2 operator *(double value, Vector2 vector) => new Vector2(value * (vector as Vector));

        public static Vector2 operator /(Vector2 vector, double value) => new Vector2((vector as Vector) / value);

        public static Vector2 operator /(double value, Vector2 vector) => new Vector2(value / (vector as Vector));

        public static Vector2 operator -(Vector2 vector) => new Vector2(-(vector as Vector));

        public Vector2 Rotate(double angle) => new Vector2(X * Math.Cos(angle) - Y * Math.Sin(angle), X * Math.Sin(angle) + Y * Math.Cos(angle));
    }
}
