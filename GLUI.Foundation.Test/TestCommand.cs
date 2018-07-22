using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GLUI.Foundation.Test
{
    [TestClass]
    public class TestCommand
    {
        [TestMethod]
        public void TestConstructor()
        {
            Assert.ThrowsException<InvalidOperationException>(() => { var wInvalid = new Command("Invalid"); });

            var wCommand = new Command("Test", Key.A);
            Assert.AreEqual("Test", wCommand.Name);
            Assert.AreEqual(1, wCommand.Keys.Count);
            Assert.AreEqual(Key.A, wCommand.Keys[0]);

            wCommand = new Command("Fullscreen", Key.AltLeft, Key.Enter);
            Assert.AreEqual("Fullscreen", wCommand.Name);
            Assert.AreEqual(2, wCommand.Keys.Count);
            Assert.AreEqual(Key.AltLeft, wCommand.Keys[0]);
            Assert.AreEqual(Key.Enter, wCommand.Keys[1]);
        }

        [TestMethod]
        public void TestToString()
        {
            var wCommand = new Command("A very complex command with lots of keys", Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G);

            Assert.AreEqual("A very complex command with lots of keys -> A + B + C + D + E + F + G", wCommand.ToString());
        }

        [TestMethod]
        public void TestCheck()
        {
            var wCommand = new Command("Fullscreen", Key.AltLeft, Key.Enter);
            var wActivated = false;
            var wEventHandler = new EventHandler<EventArgs>((o, e) =>
            {
                wActivated = true;
            });
            wCommand.Activated += wEventHandler;
            var wKeyboardState = new KeyboardState();

            wCommand.Check(wKeyboardState);
            Assert.IsFalse(wActivated);

            wKeyboardState.KeyDown[Key.Enter] = true;
            wCommand.Check(wKeyboardState);
            Assert.IsFalse(wActivated);

            wKeyboardState.KeyDown[Key.LAlt] = true;
            wCommand.Check(wKeyboardState);
            Assert.IsFalse(wActivated);

            wKeyboardState.KeyDown[Key.Enter] = false;
            wCommand.Check(wKeyboardState);
            Assert.IsFalse(wActivated);

            wKeyboardState.KeyDown[Key.Enter] = true;
            wCommand.Check(wKeyboardState);
            Assert.IsTrue(wActivated);

            wActivated = false;
            wKeyboardState.KeyDown[Key.LAlt] = false;
            wKeyboardState.KeyDown[Key.Enter] = false;
            wCommand.Check(wKeyboardState);
            Assert.IsFalse(wActivated);

            wCommand.Activated -= wEventHandler;

            wKeyboardState.KeyDown[Key.LAlt] = true;
            wKeyboardState.KeyDown[Key.Enter] = true;
            wCommand.Check(wKeyboardState);
            Assert.IsFalse(wActivated);
        }

        [TestMethod]
        public void TestExecutes()
        {
            var wCommand = new Command("Fullscreen", Key.AltLeft, Key.Enter);
            var wKeyboardState = new KeyboardState();
            var wNumberOfExecutions = 0;

            wCommand.Executes(() => { wNumberOfExecutions++; })
                    .Executes(null)
                    .Executes(() => { wNumberOfExecutions++; })
                    .Executes(null);

            wKeyboardState.KeyDown[Key.LAlt] = true;
            wKeyboardState.KeyDown[Key.Enter] = true;
            wCommand.Check(wKeyboardState);

            Assert.AreEqual(2, wNumberOfExecutions);
        }
    }
}
