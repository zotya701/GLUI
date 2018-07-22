using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GLUI.Foundation.MathExtension;
using System.Collections.Generic;

namespace GLUI.Foundation.Test.MathExtension
{
    [TestClass]
    public class TestVector2
    {
        [TestMethod]
        public void TestDefaultConstructor()
        {
            var wVector = new Vector2();

            Assert.AreEqual(0, wVector.X);
            Assert.AreEqual(0, wVector.Y);

            Assert.AreEqual(0, wVector[0]);
            Assert.AreEqual(0, wVector[1]);

            Assert.IsTrue(wVector == Vector2.Zero);
            Assert.IsFalse(wVector != Vector2.Zero);
            Assert.AreEqual(Vector2.Zero, wVector);
            Assert.AreNotSame(Vector2.Zero, wVector);

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[2]; });

            wVector.X = 1;
            wVector.Y = 2;

            Assert.AreEqual(1, wVector.X);
            Assert.AreEqual(2, wVector.Y);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);

            Assert.IsFalse(wVector == Vector2.Zero);
            Assert.IsTrue(wVector != Vector2.Zero);
            Assert.AreNotEqual(Vector2.Zero, wVector);
            Assert.AreNotSame(Vector2.Zero, wVector);
        }

        [TestMethod]
        public void TestConstructor()
        {
            foreach(var wVector in new List<Vector2> { new Vector2(1,2), (1, 2) })
            {
                Assert.AreEqual(1, wVector.X);
                Assert.AreEqual(2, wVector.Y);

                Assert.AreEqual(1, wVector[0]);
                Assert.AreEqual(2, wVector[1]);

                Assert.IsFalse(wVector == Vector2.Zero);
                Assert.IsTrue(wVector != Vector2.Zero);
                Assert.AreNotEqual(Vector2.Zero, wVector);
                Assert.AreNotSame(Vector2.Zero, wVector);

                Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[-1]; });
                Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[2]; });

                wVector.X = 0;
                wVector.Y = 0;

                Assert.AreEqual(0, wVector.X);
                Assert.AreEqual(0, wVector.Y);

                Assert.AreEqual(0, wVector[0]);
                Assert.AreEqual(0, wVector[1]);

                Assert.IsTrue(wVector == Vector2.Zero);
                Assert.IsFalse(wVector != Vector2.Zero);
                Assert.AreEqual(Vector2.Zero, wVector);
                Assert.AreNotSame(Vector2.Zero, wVector);
            }
        }

        [TestMethod]
        public void TestDeconstructor()
        {
            var wVector = new Vector2(1, 2);
            var (X, Y) = wVector;

            Assert.AreEqual(wVector.X, X);
            Assert.AreEqual(wVector.Y, Y);
        }

        [TestMethod]
        public void TestUnitVectors()
        {
            Assert.AreSame(Vector2.UnitX, Vector2.UnitX);
            Assert.AreSame(Vector2.UnitY, Vector2.UnitY);
            Assert.AreSame(Vector2.Zero, Vector2.Zero);

            Assert.AreEqual(1, Vector2.UnitX.X);
            Assert.AreEqual(0, Vector2.UnitX.Y);
            Assert.AreEqual(1, Vector2.UnitX.Length);

            Assert.AreEqual(0, Vector2.UnitY.X);
            Assert.AreEqual(1, Vector2.UnitY.Y);
            Assert.AreEqual(1, Vector2.UnitY.Length);

            Assert.AreEqual(0, Vector2.Zero.X);
            Assert.AreEqual(0, Vector2.Zero.Y);
            Assert.AreEqual(0, Vector2.Zero.Length);

            Assert.AreNotEqual(Vector2.UnitX, Vector2.UnitY);
            Assert.AreNotEqual(Vector2.UnitX, Vector2.Zero);

            Assert.AreNotEqual(Vector2.UnitY, Vector2.Zero);

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector2.UnitX[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector2.UnitX[2]; });

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector2.UnitY[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector2.UnitY[2]; });

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector2.Zero[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector2.Zero[2]; });
        }

        [TestMethod]
        public void TestNormalize()
        {
            var wVector = new Vector2(0, 0);

            var wResult = wVector.Normalize();

            Assert.AreEqual(double.NaN, wResult[0]);
            Assert.AreEqual(double.NaN, wResult[1]);
            Assert.AreEqual(double.NaN, wResult.Length);

            wVector = new Vector2(3, 4);

            wResult = wVector.Normalize();

            Assert.AreNotEqual(1, wResult[0]);
            Assert.AreNotEqual(2, wResult[1]);

            Assert.AreNotEqual(1, wVector.Length);
            Assert.AreEqual(1, wResult.Length);

            Assert.AreEqual(Math.Sign(wResult[0]), Math.Sign(wVector[0]));
            Assert.AreEqual(Math.Sign(wResult[1]), Math.Sign(wVector[1]));
        }

        [TestMethod]
        public void TestAddition()
        {
            var wVector1 = new Vector2(0, 0);
            var wVector2 = new Vector2(1, 2);

            var wResult = wVector1 + 1;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(1, wResult[1]);

            wResult = 1 + wVector1;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(1, wResult[1]);

            wResult = wVector1 + wVector2;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(2, wResult[1]);

            wResult = wResult + 1;

            Assert.AreEqual(2, wResult[0]);
            Assert.AreEqual(3, wResult[1]);

            wResult = 1 + wResult;
            Assert.AreEqual(3, wResult[0]);
            Assert.AreEqual(4, wResult[1]);

            wResult = wResult + wVector2;
            Assert.AreEqual(4, wResult[0]);
            Assert.AreEqual(6, wResult[1]);

            Assert.AreEqual(0, wVector1[0]);
            Assert.AreEqual(0, wVector1[1]);

            Assert.AreEqual(1, wVector2[0]);
            Assert.AreEqual(2, wVector2[1]);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            var wVector1 = new Vector2(0, 0);
            var wVector2 = new Vector2(1, 2);

            var wResult = wVector1 - 1;

            Assert.AreEqual(-1, wResult[0]);
            Assert.AreEqual(-1, wResult[1]);

            wResult = 1 - wVector1;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(1, wResult[1]);

            wResult = wVector1 - wVector2;

            Assert.AreEqual(-1, wResult[0]);
            Assert.AreEqual(-2, wResult[1]);

            wResult = wResult - 1;

            Assert.AreEqual(-2, wResult[0]);
            Assert.AreEqual(-3, wResult[1]);

            wResult = 1 - wResult;
            Assert.AreEqual(3, wResult[0]);
            Assert.AreEqual(4, wResult[1]);

            wResult = wResult - wVector2;
            Assert.AreEqual(2, wResult[0]);
            Assert.AreEqual(2, wResult[1]);

            Assert.AreEqual(0, wVector1[0]);
            Assert.AreEqual(0, wVector1[1]);

            Assert.AreEqual(1, wVector2[0]);
            Assert.AreEqual(2, wVector2[1]);
        }

        [TestMethod]
        public void TestNegation()
        {
            var wVector = new Vector2(0, 0);
            var wNegated = new Vector2(0, 0);

            Assert.AreEqual(Vector2.Zero, wVector);
            Assert.AreEqual(Vector2.Zero, wNegated);
            Assert.AreEqual(-wVector, -wVector);
            Assert.AreEqual(wNegated, -wVector);
            Assert.AreEqual(-wNegated, wVector);
            Assert.AreEqual(wVector, -(-wVector));
            Assert.AreEqual(Vector2.Zero, wVector);
            Assert.AreEqual(Vector2.Zero, wNegated);

            wVector = new Vector2(1, 2);
            wNegated = new Vector2(-1, -2);

            Assert.AreEqual(-wVector, -wVector);
            Assert.AreEqual(-wNegated, -wNegated);
            Assert.AreEqual(wNegated, -wVector);
            Assert.AreEqual(-wNegated, wVector);
            Assert.AreEqual(wNegated, -(-wNegated));
            Assert.AreEqual(wNegated, -(-wNegated));

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);

            Assert.AreEqual(-1, wNegated[0]);
            Assert.AreEqual(-2, wNegated[1]);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            var wVector = new Vector2(1, 2);

            var wResult = wVector * 1;

            Assert.AreEqual(1 * 1, wResult[0]);
            Assert.AreEqual(2 * 1, wResult[1]);

            wResult = 1 * wVector;

            Assert.AreEqual(1 * 1, wResult[0]);
            Assert.AreEqual(2 * 1, wResult[1]);

            wResult = wVector * 2;

            Assert.AreEqual(1 * 2, wResult[0]);
            Assert.AreEqual(2 * 2, wResult[1]);

            wResult = 2 * wVector;

            Assert.AreEqual(2 * 1, wResult[0]);
            Assert.AreEqual(2 * 2, wResult[1]);

            wResult = wVector * 0.5;

            Assert.AreEqual(1 * 0.5, wResult[0]);
            Assert.AreEqual(2 * 0.5, wResult[1]);

            wResult = 0.5 * wVector;

            Assert.AreEqual(0.5 * 1, wResult[0]);
            Assert.AreEqual(0.5 * 2, wResult[1]);

            wResult = wResult * 2;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(2, wResult[1]);

            wResult = 2 * wResult;

            Assert.AreEqual(2, wResult[0]);
            Assert.AreEqual(4, wResult[1]);

            wResult = wResult * 0.5;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(2, wResult[1]);

            wResult = 0.5 * wResult;

            Assert.AreEqual(0.5, wResult[0]);
            Assert.AreEqual(1, wResult[1]);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);
        }

        [TestMethod]
        public void TestDivision()
        {
            var wVector = new Vector2(1, 2);

            var wResult = wVector / 1;

            Assert.AreEqual(1 / 1d, wResult[0]);
            Assert.AreEqual(2 / 1d, wResult[1]);

            wResult = 1 / wVector;

            Assert.AreEqual(1d / 1, wResult[0]);
            Assert.AreEqual(1d / 2, wResult[1]);

            wResult = wVector / 2;

            Assert.AreEqual(1 / 2d, wResult[0]);
            Assert.AreEqual(2 / 2d, wResult[1]);

            wResult = 2 / wVector;

            Assert.AreEqual(2d / 1, wResult[0]);
            Assert.AreEqual(2d / 2, wResult[1]);

            wResult = wVector / 0.5;

            Assert.AreEqual(1 / 0.5d, wResult[0]);
            Assert.AreEqual(2 / 0.5d, wResult[1]);

            wResult = 0.5 / wVector;

            Assert.AreEqual(0.5d / 1, wResult[0]);
            Assert.AreEqual(0.5d / 2, wResult[1]);

            wResult = wResult / 2;

            Assert.AreEqual(0.5d / 2, wResult[0]);
            Assert.AreEqual(0.5d / 4, wResult[1]);

            wResult = 2 / wResult;

            Assert.AreEqual(2 / (0.5d / 2), wResult[0]);
            Assert.AreEqual(2 / (0.5d / 4), wResult[1]);

            wResult = wResult / 0.5;

            Assert.AreEqual(4 / (0.5d / 2), wResult[0]);
            Assert.AreEqual(4 / (0.5d / 4), wResult[1]);

            wResult = 0.5 / wResult;

            Assert.AreEqual(0.5d / (4 / (0.5d / 2)), wResult[0]);
            Assert.AreEqual(0.5d / (4 / (0.5d / 4)), wResult[1]);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);
        }

        [TestMethod]
        public void TestRotate()
        {
            var wVector = new Vector2(1, 0);

            var wResult = wVector.Rotate(Math.PI / 2);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(1, wResult[1], 1.0E-10);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(0, wVector[1]);

            wResult = wVector.Rotate(-Math.PI / 2);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(-1, wResult[1], 1.0E-10);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(0, wVector[1]);

            wResult = wResult.Rotate(Math.PI);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(1, wResult[1], 1.0E-10);

            wResult = wResult.Rotate(-Math.PI);
            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(-1, wResult[1], 1.0E-10);
        }
    }
}
