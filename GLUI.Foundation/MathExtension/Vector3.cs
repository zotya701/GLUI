using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation.MathExtension
{
    public class Vector3 : Vector
    {
        public static Vector3 UnitX { get; } = Unit(0, 3) as Vector3;
        public static Vector3 UnitY { get; } = Unit(1, 3) as Vector3;
        public static Vector3 UnitZ { get; } = Unit(2, 3) as Vector3;

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

        public double Z
        {
            get { return this[2]; }
            set { this[2] = value; }
        }

        public Vector3(double X = 0, double Y = 0, double Z = 0) : base(3)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public new Vector3 Normalize()
        {
            return this.Normalize();
        }

        public static Vector3 operator +(Vector3 vector, Vector3 value)
        {
            return vector + value;
        }

        public static Vector3 operator +(Vector3 vector, double value)
        {
            return vector + value;
        }

        public static Vector3 operator +(double value, Vector3 vector)
        {
            return vector + value;
        }

        public static Vector3 operator -(Vector3 vector, Vector3 value)
        {
            return vector - value;
        }

        public static Vector3 operator -(Vector3 vector, double value)
        {
            return vector - value;
        }

        public static Vector3 operator -(double value, Vector3 vector)
        {
            return vector - value;
        }

        public static Vector3 operator *(Vector3 vector, double value)
        {
            return vector * value;
        }

        public static Vector3 operator *(double value, Vector3 vector)
        {
            return vector * value;
        }

        public static Vector3 operator /(Vector3 vector, double value)
        {
            return vector / value;
        }

        public static Vector3 operator /(double value, Vector3 vector)
        {
            return vector / value;
        }

        public Vector3 RotateX(double angle)
        {
            return new Vector3(X = X,
                               Y = Y * Math.Cos(angle) - Z * Math.Sin(angle),
                               Z = Y * Math.Sin(angle) + Z * Math.Cos(angle));
        }

        public Vector3 RotateY(double angle)
        {
            return new Vector3(X = X * Math.Cos(angle) + Z * Math.Sin(angle),
                               Y = Y,
                               Z = -X * Math.Sin(angle) + Z * Math.Cos(angle));
        }

        public Vector3 RotateZ(double angle)
        {
            return new Vector3(X = X * Math.Cos(angle) - Y * Math.Sin(angle),
                               Y = X * Math.Sin(angle) + Y * Math.Cos(angle),
                               Z = Z);
        }

        public Vector3 Rotate(Vector3 axis, double angle)
        {
            var v = this;
            var k = axis.Normalize();
            return v * Math.Cos(angle) + (k % v) * Math.Sin(angle) + k * (k * v) * (1 - Math.Cos(angle));
        }

        public static Vector3 operator %(Vector3 left, Vector3 right)
        {
            return new Vector3(left.Y * right.Z - left.Z * right.Y,
                               left.Z * right.X - left.X * right.Z,
                               left.X * right.Y - left.Y * right.X);
        }
    }
}
