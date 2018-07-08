using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GLUI.Foundation;

namespace GLUI.GLUI
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
                //mSizeAnimator.Start(Size, Size - Size * 0.1f);
                //mLocationAnimator.Start(Location, Location + Size * 0.05f);
                mSizeAnimator.Start(Size, Size - new Vector2(4,4));
                mLocationAnimator.Start(Location, Location + new Vector2(2,2));
                mLabelAnimator.Start(new Vector2(1.0f, 0), new Vector2(mSizeAnimator.Target[0] / mSizeAnimator.Source[0], 0));
            };
            mButtonReleased += (o, e) =>
            {
                mSizeAnimator.Invert();
                mLocationAnimator.Invert();
                mLabelAnimator.Invert();
                Highlighted = false;
            };
            MouseLeaved += (o, e) =>
            {
                if (!Pressed) Highlighted = false;
            };
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            //if (Pressed || mSizeAnimator.IsRunning || mLocationAnimator.IsRunning || mLabelAnimator.IsRunning)
            if (Pressed || mouseState.IsOverDirectly)
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
            //if (Label.Size.X == 0 && Label.Size.Y == 0)
            //{
            //    Size = CalculateSize() + new Vector2(10, 5);
            //}
            //Size = new Vector2(Math.Max(Width, Label.Size.X),
            //                   Math.Max(Height, Label.Size.Y));
            Label.Size = Size;
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
