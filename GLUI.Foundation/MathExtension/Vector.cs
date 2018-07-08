using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation.MathExtension
{
    public class Vector
    {
        private int mDimension;
        private double[] mValues;

        public static Vector Unit(int axis, int dimension)
        {
            var wUnit = new Vector(dimension);
            wUnit[axis] = 1;
            return wUnit;
        }

        public double this[int i]
        {
            get { return mValues[i]; }
            set { mValues[i] = value; }
        }

        public Vector(int dimension)
        {
            if (dimension < 2) throw new IndexOutOfRangeException($"A vector cannot contain less than two elements! You tried to instantiate a vector with dimension: {dimension}.");
            mDimension = dimension;
            mValues = new double[dimension];
        }

        public Vector(Vector vector)
        {
            mDimension = vector.mDimension;
            mValues = new double[mDimension];
            for (int i = 0; i < mDimension; ++i)
            {
                mValues[i] = vector[i];
            }
        }

        public double Length()
        {
            return Math.Sqrt(mValues.Sum(value => value * value));
        }

        public Vector Normalize()
        {
            return this / Length();
        }

        private Vector DoForAll(double value, Func<double, double, double> operation)
        {
            var wResult = new Vector(mDimension);
            for (int i = 0; i < mDimension; ++i)
            {
                wResult[i] = operation(mValues[i], value);
            }
            return wResult;
        }

        private Vector DoForAll(Vector vector, Func<double, double, double> operation)
        {
            if (mValues.Length != vector.mValues.Length)
            {
                throw new InvalidOperationException($"The two vector's dimension must be equal!");
            }
            var wResult = new Vector(mDimension);
            for(int i = 0; i < mDimension; ++i)
            {
                wResult[i] = operation(mValues[i], vector[i]);
            }
            return wResult;
        }

        public static Vector operator +(Vector vector, Vector value)
        {
            return vector.DoForAll(value, (l, r) => l + r);
        }

        public static Vector operator +(Vector vector, double value)
        {
            return vector.DoForAll(value, (l, r) => l + r);
        }

        public static Vector operator +(double value, Vector vector)
        {
            return vector * value;
        }

        public static Vector operator -(Vector vector, Vector value)
        {
            return vector.DoForAll(value, (l, r) => l - r);
        }

        public static Vector operator -(Vector vector, double value)
        {
            return vector.DoForAll(value, (l, r) => l - r);
        }

        public static Vector operator -(double value, Vector vector)
        {
            return vector * value;
        }

        public static Vector operator *(Vector vector, double value)
        {
            return vector.DoForAll(value, (l, r) => l * r);
        }

        public static Vector operator *(double value, Vector vector)
        {
            return vector * value;
        }

        public static Vector operator /(Vector vector, double value)
        {
            return vector.DoForAll(value, (x, y) => x / y);
        }

        public static Vector operator /(double value, Vector vector)
        {
            return vector / value;
        }

        public static double operator *(Vector left, Vector right)
        {
            return left.DoForAll(right, (l, r) => l * r).mValues.Sum();
        }
    }
}
