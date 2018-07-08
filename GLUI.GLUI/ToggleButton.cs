using GLUI.Foundation;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.GLUI
{
    public class ToggleButton : ButtonBase
    {
        private bool mDisposed = false;

        private bool mOn = false;

        private Animator mSizeAnimator = new Animator() { Duration = TimeSpan.FromMilliseconds(30) };
        private Animator mLocationAnimator = new Animator() { Duration = TimeSpan.FromMilliseconds(30) };
        private Animator mLabelAnimator = new Animator() { Duration = TimeSpan.FromMilliseconds(30) };

        public bool On
        {
            get
            {
                return mOn;
            }
            set
            {
                if (mOn == value) return;

                mOn = value;
                Highlighted = value;

                ToggleButtonToggled?.Invoke(this, new EventArgs());
                if (mOn)
                {
                    if (mSizeAnimator.IsRunning || mLocationAnimator.IsRunning || mLabelAnimator.IsRunning)
                    {
                        Size = mSizeAnimator.Target;
                        Location = mLocationAnimator.Target;
                        Label.GLScale = mLabelAnimator.Target[0];
                    }
                    mSizeAnimator.Start(Size, Size - new Vector2(4, 4));
                    mLocationAnimator.Start(Location, Location + new Vector2(2, 2));
                    mLabelAnimator.Start(new Vector2(1.0f, 0), new Vector2(mSizeAnimator.Target[0] / mSizeAnimator.Source[0], 0));
                    ToggleButtonOn?.Invoke(this, new EventArgs());
                }
                else
                {
                    mSizeAnimator.Invert();
                    mLocationAnimator.Invert();
                    mLabelAnimator.Invert();
                    ToggleButtonOff?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler ToggleButtonOn;
        public event EventHandler ToggleButtonOff;
        public event EventHandler ToggleButtonToggled;

        public ToggleButton(string text)
        {
            Label.Text = text;

            ButtonClicked += (o, e) =>
            {
                On = !On;
            };

            MouseLeaved += (o, e) =>
            {
                if (!On) Highlighted = false;
            };
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if(On || mouseState.IsOverDirectly)
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
            if (Label.Size.X == 0 && Label.Size.Y == 0)
            {
                Size = CalculateSize() + new Vector2(10, 5);
            }
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
