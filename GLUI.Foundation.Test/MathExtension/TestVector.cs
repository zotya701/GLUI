using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GLUI.Foundation.MathExtension;
using System.Linq;

namespace GLUI.Foundation.Test.MathExtension
{
    [TestClass]
    public class TestVector
    {
        [TestMethod]
        public void TestConstructor1()
        {
            for (int dimension = -10; dimension < 10; ++dimension)
            {
                if (dimension < 2)
                {
                    Assert.ThrowsException<InvalidOperationException>(() => { new Vector(dimension); });
                }
                else
                {
                    var wVector = new Vector(dimension);

                    for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0, wVector[i]); }
                    Assert.IsTrue(wVector == Vector.Zero(dimension));
                    Assert.IsFalse(wVector != Vector.Zero(dimension));
                    Assert.AreEqual(Vector.Zero(dimension), wVector);
                    Assert.AreNotSame(Vector.Zero(dimension), wVector);

                    for (int i = 0; i < dimension; ++i) { wVector[i] = i + 1; }

                    for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector[i]); }
                    Assert.IsFalse(wVector == Vector.Zero(dimension));
                    Assert.IsTrue(wVector != Vector.Zero(dimension));
                    Assert.AreNotEqual(Vector.Zero(dimension), wVector);
                    Assert.AreNotSame(Vector.Zero(dimension), wVector);

                    Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[-1]; });
                    Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[dimension]; });
                }
            }
        }

        [TestMethod]
        public void TestConstructor2()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector[i] = i + 1; }
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector[i]); }

                var wResult = new Vector(wVector);
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i], wResult[i]); }
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wResult[i]); }

                for (int i = 0; i < dimension; ++i) { wResult[i] = 0; }
                Assert.AreEqual(Vector.Zero(dimension), wResult);

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector[i]); }
            }
        }

        [TestMethod]
        public void TestUnitVectors()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                for (int axis = 0; axis < dimension; ++axis)
                {
                    var wUnit = Vector.Unit(axis, dimension);
                    for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i == axis ? 1 : 0, wUnit[i]); }
                    Assert.AreEqual(1, wUnit.Length);

                    Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wUnit[-1]; });
                    Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wUnit[dimension]; });

                    Assert.AreEqual(Vector.Unit(axis, dimension), wUnit);
                    Assert.AreEqual(Vector.Unit(axis, dimension).GetHashCode(), wUnit.GetHashCode());
                    Assert.AreSame(Vector.Unit(axis, dimension), wUnit);

                    var wNewUnit = Vector.NewUnit(axis, dimension);
                    Assert.AreEqual(wNewUnit, wUnit);
                    Assert.AreNotEqual(wNewUnit.GetHashCode(), wUnit.GetHashCode());
                    Assert.AreNotSame(wNewUnit, wUnit);
                }

                var wZero = Vector.Zero(dimension);
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0, wZero[i]); }
                Assert.AreEqual(0, wZero.Length);

                Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wZero[-1]; });
                Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wZero[dimension]; });

                Assert.AreEqual(Vector.Zero(dimension), wZero);
                Assert.AreEqual(Vector.Zero(dimension).GetHashCode(), wZero.GetHashCode());
                Assert.AreSame(Vector.Zero(dimension), wZero);

                var wNewZero = Vector.NewZero(dimension);
                Assert.AreEqual(wNewZero, wZero);
                Assert.AreNotEqual(wNewZero.GetHashCode(), wZero.GetHashCode());
                Assert.AreNotSame(wNewZero, wZero);
            }
        }

        [TestMethod]
        public void TestNormalize()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector[i] = 0; }

                var wResult = wVector.Normalize();
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(double.NaN, wResult[i]); }
                Assert.AreEqual(double.NaN, wResult.Length);
                Assert.AreEqual(0, wVector.Length, 1E-10);

                wVector = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector[i] = i + 1; }

                wResult = wVector.Normalize();
                for (int i = 0; i < dimension; ++i)
                {
                    Assert.AreNotEqual(i + 1, wResult[i]);
                    Assert.AreEqual(Math.Sign(wVector[i]), Math.Sign(wResult[i]));
                    Assert.AreEqual(wVector[i] / wVector.Length, wResult[i], 1E-10);
                }
                Assert.AreEqual(1, wResult.Length, 1E-10);
                Assert.AreNotEqual(1, wVector.Length, 1E-10);
            }
        }

        [TestMethod]
        public void TestAddition()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector1 = new Vector(dimension);
                var wVector2 = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector1[i] = 0; wVector2[i] = i + 1; }

                var wResult = wVector1 + 1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(1, wResult[i]); }

                wResult = 1 + wVector1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(1, wResult[i]); }

                wResult = wVector1 + wVector2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector1[i] + wVector2[i], wResult[i]); }

                wResult = wResult + 1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector1[i] + wVector2[i] + 1, wResult[i]); }

                wResult = 1 + wResult;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector1[i] + wVector2[i] + 2, wResult[i]); }

                wResult = wResult + wVector2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector1[i] + 2 * wVector2[i] + 2, wResult[i]); }

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0, wVector1[i]); Assert.AreEqual(i + 1, wVector2[i]); }

                Assert.ThrowsException<InvalidOperationException>(() => { var wInvalid = Vector2.Zero + Vector3.Zero; });
            }
        }

        [TestMethod]
        public void TestSubtraction()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector1 = new Vector(dimension);
                var wVector2 = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector1[i] = 0; wVector2[i] = i + 1; }

                var wResult = wVector1 - 1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(-1, wResult[i]); }

                wResult = 1 - wVector1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(1, wResult[i]); }

                wResult = wVector1 - wVector2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector1[i] - wVector2[i], wResult[i]); }

                wResult = wResult - 1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector1[i] - wVector2[i] - 1, wResult[i]); }

                wResult = 1 - wResult;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(1 - (wVector1[i] - wVector2[i] - 1), wResult[i]); }

                wResult = wResult - wVector2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(1 - (wVector1[i] - wVector2[i] - 1) - wVector2[i], wResult[i]); }

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0, wVector1[i]); Assert.AreEqual(i + 1, wVector2[i]); }

                Assert.ThrowsException<InvalidOperationException>(() => { var wInvalid = Vector2.Zero - Vector3.Zero; });
            }
        }

        [TestMethod]
        public void TestNegation()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector = new Vector(dimension);
                var wNegated = new Vector(dimension);

                Assert.AreEqual(Vector.Zero(dimension), wVector);
                Assert.AreEqual(Vector.Zero(dimension), wNegated);
                Assert.AreEqual(-wVector, -wVector);
                Assert.AreEqual(wNegated, -wVector);
                Assert.AreEqual(-wNegated, wVector);
                Assert.AreEqual(wVector, -(-wVector));
                Assert.AreEqual(Vector.Zero(dimension), wVector);
                Assert.AreEqual(Vector.Zero(dimension), wNegated);

                wVector = new Vector(dimension);
                wNegated = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector[i] = i + 1; wNegated[i] = -(i + 1); }

                Assert.AreEqual(-wVector, -wVector);
                Assert.AreEqual(-wNegated, -wNegated);
                Assert.AreEqual(wNegated, -wVector);
                Assert.AreEqual(-wNegated, wVector);
                Assert.AreEqual(wNegated, -(-wNegated));
                Assert.AreEqual(wNegated, -(-wNegated));

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector[i]); Assert.AreEqual(-(i + 1), wNegated[i]); }
            }
        }

        [TestMethod]
        public void TestMultiplication()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector[i] = i + 1; }

                var wResult = wVector * 1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 1, wResult[i]); }

                wResult = 1 * wVector;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 1, wResult[i]); }

                wResult = wVector * 2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 2, wResult[i]); }

                wResult = 2 * wVector;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 2, wResult[i]); }

                wResult = wVector * 0.5;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 0.5, wResult[i]); }

                wResult = 0.5 * wVector;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 0.5, wResult[i]); }

                wResult = wResult * 2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i], wResult[i]); }

                wResult = 2 * wResult;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 2, wResult[i]); }

                wResult = wResult * 0.5;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i], wResult[i]); }

                wResult = 0.5 * wResult;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] * 0.5, wResult[i]); }

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector[i]); }
            }
        }

        [TestMethod]
        public void TestDivision()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector = new Vector(dimension);
                for (int i = 0; i < dimension; ++i) { wVector[i] = i + 1; }

                var wResult = wVector / 1;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] / 1, wResult[i]); }

                wResult = 1 / wVector;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(1 / wVector[i], wResult[i]); }

                wResult = wVector / 2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] / 2, wResult[i]); }

                wResult = 2 / wVector;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(2 / wVector[i], wResult[i]); }

                wResult = wVector / 0.5;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(wVector[i] / 0.5, wResult[i]); }

                wResult = 0.5 / wVector;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0.5 / wVector[i], wResult[i]); }

                wResult = wResult / 2;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0.5 / wVector[i] / 2, wResult[i]); }

                wResult = 2 / wResult;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(2 / (0.5 / wVector[i] / 2), wResult[i]); }

                wResult = wResult / 0.5;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(2 / (0.5 / wVector[i] / 2) / 0.5, wResult[i]); }

                wResult = 0.5 / wResult;
                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0.5 / (2 / (0.5 / wVector[i] / 2) / 0.5), wResult[i]); }

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector[i]); }
            }
        }

        [TestMethod]
        public void TestDotProduct()
        {
            for (int dimension = 2; dimension < 10; ++dimension)
            {
                var wVector1 = Vector.NewZero(dimension);
                var wVector2 = Vector.NewZero(dimension);

                Assert.AreEqual(0, wVector1 * wVector2);

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(0, wVector1[i]); Assert.AreEqual(0, wVector2[i]); }

                for (int i = 0; i < dimension; ++i) { wVector1[i] = i + 1; wVector2[i] = i + 1 + dimension; }

                Assert.AreEqual(Enumerable.Range(1, dimension).Zip(Enumerable.Range(1 + dimension, dimension), (l, r) => l * r).Sum(), wVector1 * wVector2);

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector1[i]); Assert.AreEqual(i + 1 + dimension, wVector2[i]); }

                Assert.AreEqual(wVector1.Length * wVector1.Length, wVector1 * wVector1, 1.0E-10);
                Assert.AreEqual(wVector2.Length * wVector2.Length, wVector2 * wVector2, 1.0E-10);

                for (int i = 0; i < dimension; ++i) { Assert.AreEqual(i + 1, wVector1[i]); Assert.AreEqual(i + 1 + dimension, wVector2[i]); }

                Assert.ThrowsException<InvalidOperationException>(() => { var wInvalid = Vector2.Zero * Vector3.Zero; });
            }
        }
    }
}
