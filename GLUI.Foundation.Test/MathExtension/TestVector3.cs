using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GLUI.Foundation.MathExtension;
using System.Collections.Generic;

namespace GLUI.Foundation.Test.MathExtension
{
    [TestClass]
    public class TestVector3
    {
        [TestMethod]
        public void TestDefaultConstructor()
        {
            var wVector = new Vector3();

            Assert.AreEqual(0, wVector.X);
            Assert.AreEqual(0, wVector.Y);
            Assert.AreEqual(0, wVector.Z);

            Assert.AreEqual(0, wVector[0]);
            Assert.AreEqual(0, wVector[1]);
            Assert.AreEqual(0, wVector[2]);

            Assert.IsTrue(wVector == Vector3.Zero);
            Assert.IsFalse(wVector != Vector3.Zero);
            Assert.AreEqual(Vector3.Zero, wVector);
            Assert.AreNotSame(Vector3.Zero, wVector);

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[3]; });

            wVector.X = 1;
            wVector.Y = 2;
            wVector.Z = 3;

            Assert.AreEqual(1, wVector.X);
            Assert.AreEqual(2, wVector.Y);
            Assert.AreEqual(3, wVector.Z);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);
            Assert.AreEqual(3, wVector[2]);

            Assert.IsFalse(wVector == Vector3.Zero);
            Assert.IsTrue(wVector != Vector3.Zero);
            Assert.AreNotEqual(Vector3.Zero, wVector);
            Assert.AreNotSame(Vector3.Zero, wVector);
        }

        [TestMethod]
        public void TestConstructor()
        {
            foreach (var wVector in new List<Vector3> { new Vector3(1, 2, 3), (1, 2, 3) })
            {
                Assert.AreEqual(1, wVector.X);
                Assert.AreEqual(2, wVector.Y);
                Assert.AreEqual(3, wVector.Z);

                Assert.AreEqual(1, wVector[0]);
                Assert.AreEqual(2, wVector[1]);
                Assert.AreEqual(3, wVector[2]);

                Assert.IsFalse(wVector == Vector3.Zero);
                Assert.IsTrue(wVector != Vector3.Zero);
                Assert.AreNotEqual(Vector3.Zero, wVector);
                Assert.AreNotSame(Vector3.Zero, wVector);

                Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[-1]; });
                Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = wVector[3]; });

                wVector.X = 0;
                wVector.Y = 0;
                wVector.Z = 0;

                Assert.AreEqual(0, wVector.X);
                Assert.AreEqual(0, wVector.Y);
                Assert.AreEqual(0, wVector.Z);

                Assert.AreEqual(0, wVector[0]);
                Assert.AreEqual(0, wVector[1]);
                Assert.AreEqual(0, wVector[2]);

                Assert.IsTrue(wVector == Vector3.Zero);
                Assert.IsFalse(wVector != Vector3.Zero);
                Assert.AreEqual(Vector3.Zero, wVector);
                Assert.AreNotSame(Vector3.Zero, wVector);
            }
        }

        [TestMethod]
        public void TestDeconstructor()
        {
            var wVector = new Vector3(1, 2, 3);
            var (X, Y, Z) = wVector;

            Assert.AreEqual(wVector.X, X);
            Assert.AreEqual(wVector.Y, Y);
            Assert.AreEqual(wVector.Z, Z);
        }

        [TestMethod]
        public void TestUnitVectors()
        {
            Assert.AreSame(Vector3.UnitX, Vector3.UnitX);
            Assert.AreSame(Vector3.UnitY, Vector3.UnitY);
            Assert.AreSame(Vector3.UnitZ, Vector3.UnitZ);
            Assert.AreSame(Vector3.Zero, Vector3.Zero);

            Assert.AreEqual(1, Vector3.UnitX.X);
            Assert.AreEqual(0, Vector3.UnitX.Y);
            Assert.AreEqual(0, Vector3.UnitX.Z);
            Assert.AreEqual(1, Vector3.UnitX.Length);

            Assert.AreEqual(0, Vector3.UnitY.X);
            Assert.AreEqual(1, Vector3.UnitY.Y);
            Assert.AreEqual(0, Vector3.UnitY.Z);
            Assert.AreEqual(1, Vector3.UnitY.Length);

            Assert.AreEqual(0, Vector3.UnitZ.X);
            Assert.AreEqual(0, Vector3.UnitZ.Y);
            Assert.AreEqual(1, Vector3.UnitZ.Z);
            Assert.AreEqual(1, Vector3.UnitZ.Length);

            Assert.AreEqual(0, Vector3.Zero.X);
            Assert.AreEqual(0, Vector3.Zero.Y);
            Assert.AreEqual(0, Vector3.Zero.Z);
            Assert.AreEqual(0, Vector3.Zero.Length);

            Assert.AreNotEqual(Vector3.UnitX, Vector3.UnitY);
            Assert.AreNotEqual(Vector3.UnitX, Vector3.UnitZ);
            Assert.AreNotEqual(Vector3.UnitX, Vector3.Zero);

            Assert.AreNotEqual(Vector3.UnitY, Vector3.UnitZ);
            Assert.AreNotEqual(Vector3.UnitY, Vector3.Zero);

            Assert.AreNotEqual(Vector3.UnitZ, Vector3.Zero);

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.UnitX[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.UnitX[3]; });

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.UnitY[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.UnitY[3]; });

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.UnitZ[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.UnitZ[3]; });

            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.Zero[-1]; });
            Assert.ThrowsException<IndexOutOfRangeException>(() => { var wValue = Vector3.Zero[3]; });
        }

        [TestMethod]
        public void TestNormalize()
        {
            var wVector = new Vector3(0, 0, 0);

            var wResult = wVector.Normalize();

            Assert.AreEqual(double.NaN, wResult[0]);
            Assert.AreEqual(double.NaN, wResult[1]);
            Assert.AreEqual(double.NaN, wResult[2]);
            Assert.AreEqual(double.NaN, wResult.Length);

            wVector = new Vector3(1, 2, 3);

            wResult = wVector.Normalize();

            Assert.AreNotEqual(1, wResult[0]);
            Assert.AreNotEqual(2, wResult[1]);
            Assert.AreNotEqual(3, wResult[2]);

            Assert.AreNotEqual(1, wVector.Length);
            Assert.AreEqual(1, wResult.Length);

            Assert.AreEqual(Math.Sign(wResult[0]), Math.Sign(wVector[0]));
            Assert.AreEqual(Math.Sign(wResult[1]), Math.Sign(wVector[1]));
            Assert.AreEqual(Math.Sign(wResult[2]), Math.Sign(wVector[2]));
        }

        [TestMethod]
        public void TestAddition()
        {
            var wVector1 = new Vector3(0, 0, 0);
            var wVector2 = new Vector3(1, 2, 3);

            var wResult = wVector1 + 1;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(1, wResult[1]);
            Assert.AreEqual(1, wResult[2]);

            wResult = 1 + wVector1;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(1, wResult[1]);
            Assert.AreEqual(1, wResult[2]);

            wResult = wVector1 + wVector2;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(2, wResult[1]);
            Assert.AreEqual(3, wResult[2]);

            wResult = wResult + 1;

            Assert.AreEqual(2, wResult[0]);
            Assert.AreEqual(3, wResult[1]);
            Assert.AreEqual(4, wResult[2]);

            wResult = 1 + wResult;
            Assert.AreEqual(3, wResult[0]);
            Assert.AreEqual(4, wResult[1]);
            Assert.AreEqual(5, wResult[2]);

            wResult = wResult + wVector2;
            Assert.AreEqual(4, wResult[0]);
            Assert.AreEqual(6, wResult[1]);
            Assert.AreEqual(8, wResult[2]);

            Assert.AreEqual(0, wVector1[0]);
            Assert.AreEqual(0, wVector1[1]);
            Assert.AreEqual(0, wVector1[2]);

            Assert.AreEqual(1, wVector2[0]);
            Assert.AreEqual(2, wVector2[1]);
            Assert.AreEqual(3, wVector2[2]);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            var wVector1 = new Vector3(0, 0, 0);
            var wVector2 = new Vector3(1, 2, 3);

            var wResult = wVector1 - 1;

            Assert.AreEqual(-1, wResult[0]);
            Assert.AreEqual(-1, wResult[1]);
            Assert.AreEqual(-1, wResult[2]);

            wResult = 1 - wVector1;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(1, wResult[1]);
            Assert.AreEqual(1, wResult[2]);

            wResult = wVector1 - wVector2;

            Assert.AreEqual(-1, wResult[0]);
            Assert.AreEqual(-2, wResult[1]);
            Assert.AreEqual(-3, wResult[2]);

            wResult = wResult - 1;

            Assert.AreEqual(-2, wResult[0]);
            Assert.AreEqual(-3, wResult[1]);
            Assert.AreEqual(-4, wResult[2]);

            wResult = 1 - wResult;
            Assert.AreEqual(3, wResult[0]);
            Assert.AreEqual(4, wResult[1]);
            Assert.AreEqual(5, wResult[2]);

            wResult = wResult - wVector2;
            Assert.AreEqual(2, wResult[0]);
            Assert.AreEqual(2, wResult[1]);
            Assert.AreEqual(2, wResult[2]);

            Assert.AreEqual(0, wVector1[0]);
            Assert.AreEqual(0, wVector1[1]);
            Assert.AreEqual(0, wVector1[2]);

            Assert.AreEqual(1, wVector2[0]);
            Assert.AreEqual(2, wVector2[1]);
            Assert.AreEqual(3, wVector2[2]);
        }

        [TestMethod]
        public void TestNegation()
        {
            var wVector = new Vector3(0, 0, 0);
            var wNegated = new Vector3(0, 0, 0);

            Assert.AreEqual(Vector3.Zero, wVector);
            Assert.AreEqual(Vector3.Zero, wNegated);
            Assert.AreEqual(-wVector, -wVector);
            Assert.AreEqual(wNegated, -wVector);
            Assert.AreEqual(-wNegated, wVector);
            Assert.AreEqual(wVector, -(-wVector));
            Assert.AreEqual(Vector3.Zero, wVector);
            Assert.AreEqual(Vector3.Zero, wNegated);

            wVector = new Vector3(1, 2, 3);
            wNegated = new Vector3(-1, -2, -3);

            Assert.AreEqual(-wVector, -wVector);
            Assert.AreEqual(-wNegated, -wNegated);
            Assert.AreEqual(wNegated, -wVector);
            Assert.AreEqual(-wNegated, wVector);
            Assert.AreEqual(wNegated, -(-wNegated));
            Assert.AreEqual(wNegated, -(-wNegated));

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);
            Assert.AreEqual(3, wVector[2]);

            Assert.AreEqual(-1, wNegated[0]);
            Assert.AreEqual(-2, wNegated[1]);
            Assert.AreEqual(-3, wNegated[2]);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            var wVector = new Vector3(1, 2, 3);

            var wResult = wVector * 1;

            Assert.AreEqual(1 * 1, wResult[0]);
            Assert.AreEqual(2 * 1, wResult[1]);
            Assert.AreEqual(3 * 1, wResult[2]);

            wResult = 1 * wVector;

            Assert.AreEqual(1 * 1, wResult[0]);
            Assert.AreEqual(2 * 1, wResult[1]);
            Assert.AreEqual(3 * 1, wResult[2]);

            wResult = wVector * 2;

            Assert.AreEqual(1 * 2, wResult[0]);
            Assert.AreEqual(2 * 2, wResult[1]);
            Assert.AreEqual(3 * 2, wResult[2]);

            wResult = 2 * wVector;

            Assert.AreEqual(2 * 1, wResult[0]);
            Assert.AreEqual(2 * 2, wResult[1]);
            Assert.AreEqual(2 * 3, wResult[2]);

            wResult = wVector * 0.5;

            Assert.AreEqual(1 * 0.5, wResult[0]);
            Assert.AreEqual(2 * 0.5, wResult[1]);
            Assert.AreEqual(3 * 0.5, wResult[2]);

            wResult = 0.5 * wVector;

            Assert.AreEqual(0.5 * 1, wResult[0]);
            Assert.AreEqual(0.5 * 2, wResult[1]);
            Assert.AreEqual(0.5 * 3, wResult[2]);

            wResult = wResult * 2;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(2, wResult[1]);
            Assert.AreEqual(3, wResult[2]);

            wResult = 2 * wResult;

            Assert.AreEqual(2, wResult[0]);
            Assert.AreEqual(4, wResult[1]);
            Assert.AreEqual(6, wResult[2]);

            wResult = wResult * 0.5;

            Assert.AreEqual(1, wResult[0]);
            Assert.AreEqual(2, wResult[1]);
            Assert.AreEqual(3, wResult[2]);

            wResult = 0.5 * wResult;

            Assert.AreEqual(0.5, wResult[0]);
            Assert.AreEqual(1, wResult[1]);
            Assert.AreEqual(1.5, wResult[2]);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);
            Assert.AreEqual(3, wVector[2]);
        }

        [TestMethod]
        public void TestDivision()
        {
            var wVector = new Vector3(1, 2, 3);

            var wResult = wVector / 1;

            Assert.AreEqual(1 / 1d, wResult[0]);
            Assert.AreEqual(2 / 1d, wResult[1]);
            Assert.AreEqual(3 / 1d, wResult[2]);

            wResult = 1 / wVector;

            Assert.AreEqual(1d / 1, wResult[0]);
            Assert.AreEqual(1d / 2, wResult[1]);
            Assert.AreEqual(1d / 3, wResult[2]);

            wResult = wVector / 2;

            Assert.AreEqual(1 / 2d, wResult[0]);
            Assert.AreEqual(2 / 2d, wResult[1]);
            Assert.AreEqual(3 / 2d, wResult[2]);

            wResult = 2 / wVector;

            Assert.AreEqual(2d / 1, wResult[0]);
            Assert.AreEqual(2d / 2, wResult[1]);
            Assert.AreEqual(2d / 3, wResult[2]);

            wResult = wVector / 0.5;

            Assert.AreEqual(1 / 0.5d, wResult[0]);
            Assert.AreEqual(2 / 0.5d, wResult[1]);
            Assert.AreEqual(3 / 0.5d, wResult[2]);

            wResult = 0.5 / wVector;

            Assert.AreEqual(0.5d / 1, wResult[0]);
            Assert.AreEqual(0.5d / 2, wResult[1]);
            Assert.AreEqual(0.5d / 3, wResult[2]);

            wResult = wResult / 2;

            Assert.AreEqual(0.5d / 2, wResult[0]);
            Assert.AreEqual(0.5d / 4, wResult[1]);
            Assert.AreEqual(0.5d / 6, wResult[2]);

            wResult = 2 / wResult;

            Assert.AreEqual(2 / (0.5d / 2), wResult[0]);
            Assert.AreEqual(2 / (0.5d / 4), wResult[1]);
            Assert.AreEqual(2 / (0.5d / 6), wResult[2]);

            wResult = wResult / 0.5;

            Assert.AreEqual(4 / (0.5d / 2), wResult[0]);
            Assert.AreEqual(4 / (0.5d / 4), wResult[1]);
            Assert.AreEqual(4 / (0.5d / 6), wResult[2]);

            wResult = 0.5 / wResult;

            Assert.AreEqual(0.5d / (4 / (0.5d / 2)), wResult[0]);
            Assert.AreEqual(0.5d / (4 / (0.5d / 4)), wResult[1]);
            Assert.AreEqual(0.5d / (4 / (0.5d / 6)), wResult[2]);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(2, wVector[1]);
            Assert.AreEqual(3, wVector[2]);
        }

        [TestMethod]
        public void TestDotProduct()
        {
            var wVector1 = new Vector3(0, 0, 0);
            var wVector2 = new Vector3(0, 0, 0);

            Assert.AreEqual(0, wVector1 * wVector2);

            Assert.AreEqual(0, wVector1[0]);
            Assert.AreEqual(0, wVector1[1]);
            Assert.AreEqual(0, wVector1[2]);

            Assert.AreEqual(0, wVector2[0]);
            Assert.AreEqual(0, wVector2[1]);
            Assert.AreEqual(0, wVector2[2]);

            wVector1 = new Vector3(1, 2, 3);
            wVector2 = new Vector3(4, 5, 6);

            Assert.AreEqual(1 * 4 + 2 * 5 + 3 * 6, wVector1 * wVector2);

            Assert.AreEqual(1, wVector1[0]);
            Assert.AreEqual(2, wVector1[1]);
            Assert.AreEqual(3, wVector1[2]);

            Assert.AreEqual(4, wVector2[0]);
            Assert.AreEqual(5, wVector2[1]);
            Assert.AreEqual(6, wVector2[2]);

            Assert.AreEqual(wVector1.Length * wVector1.Length, wVector1 * wVector1, 1.0E-10);
            Assert.AreEqual(wVector2.Length * wVector2.Length, wVector2 * wVector2, 1.0E-10);

            Assert.AreEqual(1, wVector1[0]);
            Assert.AreEqual(2, wVector1[1]);
            Assert.AreEqual(3, wVector1[2]);

            Assert.AreEqual(4, wVector2[0]);
            Assert.AreEqual(5, wVector2[1]);
            Assert.AreEqual(6, wVector2[2]);
        }

        [TestMethod]
        public void TestCrossProduct()
        {
            var wVector1 = new Vector3(0, 0, 0);
            var wVector2 = new Vector3(0, 0, 0);

            Assert.AreEqual(Vector3.Zero, wVector1 % wVector1);
            Assert.AreEqual(Vector3.Zero, wVector2 % wVector2);
            Assert.AreEqual(Vector3.Zero, wVector1 % wVector2);

            Assert.AreEqual(0, wVector1[0]);
            Assert.AreEqual(0, wVector1[1]);
            Assert.AreEqual(0, wVector1[2]);

            Assert.AreEqual(0, wVector2[0]);
            Assert.AreEqual(0, wVector2[1]);
            Assert.AreEqual(0, wVector2[2]);

            wVector1 = new Vector3(1, 2, 3);
            wVector2 = new Vector3(4, 5, 6);

            var wResult = wVector1 % wVector2;

            Assert.AreEqual(-3, wResult[0]);
            Assert.AreEqual(6, wResult[1]);
            Assert.AreEqual(-3, wResult[2]);

            Assert.AreEqual(1, wVector1[0]);
            Assert.AreEqual(2, wVector1[1]);
            Assert.AreEqual(3, wVector1[2]);

            Assert.AreEqual(4, wVector2[0]);
            Assert.AreEqual(5, wVector2[1]);
            Assert.AreEqual(6, wVector2[2]);

            wResult = wVector2 % wVector1;

            Assert.AreEqual(3, wResult[0]);
            Assert.AreEqual(-6, wResult[1]);
            Assert.AreEqual(3, wResult[2]);

            Assert.AreEqual(1, wVector1[0]);
            Assert.AreEqual(2, wVector1[1]);
            Assert.AreEqual(3, wVector1[2]);

            Assert.AreEqual(4, wVector2[0]);
            Assert.AreEqual(5, wVector2[1]);
            Assert.AreEqual(6, wVector2[2]);

            Assert.AreEqual(Vector3.UnitZ, Vector3.UnitX % Vector3.UnitY);
            Assert.AreEqual(Vector3.UnitX, Vector3.UnitY % Vector3.UnitZ);
            Assert.AreEqual(Vector3.UnitY, Vector3.UnitZ % Vector3.UnitX);
        }

        [TestMethod]
        public void TestRotateX()
        {
            var wVector = new Vector3(0, 1, 0);

            var wResult = wVector.RotateX(Math.PI / 2);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(1, wResult[2], 1.0E-10);

            Assert.AreEqual(0, wVector[0]);
            Assert.AreEqual(1, wVector[1]);
            Assert.AreEqual(0, wVector[2]);

            wResult = wVector.RotateX(-Math.PI / 2);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(-1, wResult[2], 1.0E-10);

            Assert.AreEqual(0, wVector[0]);
            Assert.AreEqual(1, wVector[1]);
            Assert.AreEqual(0, wVector[2]);

            wResult = wResult.RotateX(Math.PI);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(1, wResult[2], 1.0E-10);

            wResult = wResult.Rotate(Vector3.UnitX, -Math.PI);
            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(-1, wResult[2], 1.0E-10);
        }

        [TestMethod]
        public void TestRotateY()
        {
            var wVector = new Vector3(0, 0, 1);

            var wResult = wVector.RotateY(Math.PI / 2);

            Assert.AreEqual(1, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);

            Assert.AreEqual(0, wVector[0]);
            Assert.AreEqual(0, wVector[1]);
            Assert.AreEqual(1, wVector[2]);

            wResult = wVector.RotateY(-Math.PI / 2);

            Assert.AreEqual(-1, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);

            Assert.AreEqual(0, wVector[0]);
            Assert.AreEqual(0, wVector[1]);
            Assert.AreEqual(1, wVector[2]);

            wResult = wResult.RotateY(Math.PI);

            Assert.AreEqual(1, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);

            wResult = wResult.Rotate(Vector3.UnitY, -Math.PI);
            Assert.AreEqual(-1, wResult[0], 1.0E-10);
            Assert.AreEqual(0, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);
        }

        [TestMethod]
        public void TestRotateZ()
        {
            var wVector = new Vector3(1, 0, 0);

            var wResult = wVector.RotateZ(Math.PI / 2);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(1, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(0, wVector[1]);
            Assert.AreEqual(0, wVector[2]);

            wResult = wVector.RotateZ(-Math.PI / 2);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(-1, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);

            Assert.AreEqual(1, wVector[0]);
            Assert.AreEqual(0, wVector[1]);
            Assert.AreEqual(0, wVector[2]);

            wResult = wResult.RotateZ(Math.PI);

            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(1, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);

            wResult = wResult.Rotate(Vector3.UnitZ, -Math.PI);
            Assert.AreEqual(0, wResult[0], 1.0E-10);
            Assert.AreEqual(-1, wResult[1], 1.0E-10);
            Assert.AreEqual(0, wResult[2], 1.0E-10);
        }
    }
}
