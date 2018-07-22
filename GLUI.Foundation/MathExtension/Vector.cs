using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.Foundation.MathExtension
{
    public class Vector
    {
        private static Dictionary<(int axis, int dimension), Vector> mUnitVectors = new Dictionary<(int axis, int dimension), Vector>();
        private static Dictionary<int, Vector> mZeroVectors = new Dictionary<int, Vector>();

        private static ulong mCounter = 0;
        private ulong mId = mCounter++;
        private int mDimension;
        private double[] mValues;

        public double Length => Math.Sqrt(mValues.Sum(value => value * value));

        public static Vector Unit(int axis, int dimension)
        {
            if(!mUnitVectors.ContainsKey((axis, dimension)))
            {
                var wUnit = new Vector(dimension);
                wUnit[axis] = 1;
                mUnitVectors[(axis, dimension)] = wUnit;
            }
            return mUnitVectors[(axis, dimension)];
        }

        public static Vector NewUnit(int axis, int dimension) => new Vector(Unit(axis, dimension));

        public static Vector Zero(int dimension)
        {
            if (!mZeroVectors.ContainsKey(dimension))
            {
                mZeroVectors[dimension] = new Vector(dimension).DoForAll(0, (l, r) => 0);
            }
            return mZeroVectors[dimension];
        }

        public static Vector NewZero(int dimension) => new Vector(Zero(dimension));

        public double this[int i]
        {
            get { return mValues[i]; }
            set { mValues[i] = value; }
        }

        public Vector(int dimension)
        {
            if (dimension < 2) throw new InvalidOperationException($"A vector cannot contain less than two elements! You tried to instantiate a vector with dimension: {dimension}.");
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

        public Vector Normalize() => this / Length;

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
            if (mValues.Length != vector.mValues.Length) throw new InvalidOperationException($"The two vector's dimension must be equal!");
            var wResult = new Vector(mDimension);
            for (int i = 0; i < mDimension; ++i)
            {
                wResult[i] = operation(mValues[i], vector[i]);
            }
            return wResult;
        }

        public static Vector operator +(Vector vector, Vector value) => vector.DoForAll(value, (l, r) => l + r);

        public static Vector operator +(Vector vector, double value) => vector.DoForAll(value, (l, r) => l + r);

        public static Vector operator +(double value, Vector vector) => vector.DoForAll(value, (l, r) => r + l);

        public static Vector operator -(Vector vector, Vector value) => vector.DoForAll(value, (l, r) => l - r);

        public static Vector operator -(Vector vector, double value)=> vector.DoForAll(value, (l, r) => l - r);

        public static Vector operator -(double value, Vector vector) => vector.DoForAll(value, (l, r) => r - l);

        public static Vector operator *(Vector vector, double value) => vector.DoForAll(value, (l, r) => l * r);

        public static Vector operator *(double value, Vector vector) => vector.DoForAll(value, (l, r) => r * l);

        public static Vector operator /(Vector vector, double value) => vector.DoForAll(value, (l, r) => l / r);

        public static Vector operator /(double value, Vector vector) => vector.DoForAll(value, (l, r) => r / l);

        public static double operator *(Vector left, Vector right) => left.DoForAll(right, (l, r) => l * r).mValues.Sum();

        public static Vector operator -(Vector vector) => vector * (-1);

        public static bool operator ==(Vector left, Vector right) => left.mDimension == right.mDimension && left.mValues.SequenceEqual(right.mValues);

        public static bool operator !=(Vector left, Vector right) => !(left == right);

        public override bool Equals(object obj) => this == (obj as Vector);

        public override int GetHashCode() => mId.GetHashCode();
    }
}
