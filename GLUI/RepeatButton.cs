using Foundation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI
{
    public class RepeatButton : Button
    {
        private bool mDisposed = false;

        private bool mDelayOccured = false;
        private Stopwatch mTimer = new Stopwatch();

        public TimeSpan Delay { get; set; }
        public TimeSpan Interval{ get; set; }

        public RepeatButton(string text) : base(text)
        {
            Delay = TimeSpan.FromMilliseconds(500);
            Interval = TimeSpan.FromMilliseconds(50);

            mButtonPressed += (o, e) =>
            {
                mDelayOccured = false;
                mTimer.Restart();
            };
            mButtonReleased += (o, e) =>
            {
                mDelayOccured = false;
                mTimer.Stop();
            };
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if (Pressed && mTimer.IsRunning)
            {
                if (!mDelayOccured && mTimer.Elapsed >= Delay)
                {
                    mDelayOccured = true;
                    OnClick(this, new EventArgs());
                    mTimer.Restart();
                }
                else if (mDelayOccured && mTimer.Elapsed >= Interval)
                {
                    OnClick(this, new EventArgs());
                    mTimer.Restart();
                }
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {

            }

            mDisposed = true;

            base.Dispose(disposing);
        }
    }
}
