using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;

namespace GLUI
{
    public class Button : ButtonBase
    {
        private bool mDisposed = false;

        private Animator mSizeAnimator = new Animator() { Duration = TimeSpan.FromMilliseconds(30) };
        private Animator mLocationAnimator = new Animator() { Duration = TimeSpan.FromMilliseconds(30) };
        private Animator mLabelAnimator = new Animator() { Duration = TimeSpan.FromMilliseconds(30) };

        public Button(string text)
        {
            Highlightable = true;

            Label.Text = text;

            mButtonPressed += (o, e) =>
            {
                if(mSizeAnimator.IsRunning || mLocationAnimator.IsRunning || mLabelAnimator.IsRunning)
                {
                    Size = mSizeAnimator.Target;
                    Location = mLocationAnimator.Target;
                    Label.GLScale = mLabelAnimator.Target[0];
                }
                mSizeAnimator.Start(Size, Size - Size * 0.1f);
                mLocationAnimator.Start(Location, Location + Size * 0.05f);
                mLabelAnimator.Start(new Vector2(1.0f, 0), new Vector2(mSizeAnimator.Target[0] / mSizeAnimator.Source[0], 0));
                OnClick(this, new EventArgs());
            };
            mButtonReleased += (o, e) =>
            {
                mSizeAnimator.Invert();
                mLocationAnimator.Invert();
                mLabelAnimator.Invert();
            };
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if (Pressed || mSizeAnimator.IsRunning || mLocationAnimator.IsRunning || mLabelAnimator.IsRunning)
            {
                Highlighted = true;
            }
        }

        protected override void OnRender()
        {
            if (mSizeAnimator.IsRunning || mLocationAnimator.IsRunning || mLabelAnimator.IsRunning)
            {
                Size = mSizeAnimator.Current;
                Location = mLocationAnimator.Current;
                Label.GLScale = mLabelAnimator.Current[0];
            }
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
