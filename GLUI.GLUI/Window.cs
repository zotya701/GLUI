using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Collections.ObjectModel;
using GLUI.Foundation;
using System.Drawing;
using OpenTK;

namespace GLUI.GLUI
{
    public class Window : Component
    {
        private bool mDisposed = false;

        private bool mTitleGrabbed = false;

        private Label mTitle;
        private Button mMinimizeButton;
        private Button mMaximizeButton;
        private Button mCloseButton;

        public Window(string title = "")
        {
            mTitle = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                BorderWidth = 1.0f,
                Text = title
            };
            Children.Add(mTitle);

            mMinimizeButton = new Button("")
            {
                BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                Label = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                    BorderWidth = 0,
                    //Text = "_"
                    Text = "\u2014"
                }
            };
            Children.Add(mMinimizeButton);

            mMaximizeButton = new Button("")
            {
                BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                Label = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                    BorderWidth = 0,
                    //Text = "[]"
                    Text = "\u25A1"
                }
            };
            Children.Add(mMaximizeButton);

            mCloseButton = new Button("")
            {
                BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                Label = new Label()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                    BorderWidth = 0,
                    //Text = "X"
                    Text = "\u00D7"
                }
            };
            Children.Add(mCloseButton);

            mTitle.mLabelPressed += (o, e) =>
            {
                mTitleGrabbed = true;
            };
            mTitle.mLabelReleased += (o, e) =>
            {
                mTitleGrabbed = false;
            };
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if (mTitleGrabbed && mouseState.IsMoved)
            {
                Location = Location + new Vector2(mouseState.XDelta, mouseState.YDelta);
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnUpdate()
        {
            mCloseButton.Size = new Vector2(30, 30);
            mCloseButton.Location = new Vector2(Width - mCloseButton.Width, 0);
            mMaximizeButton.Size = new Vector2(30, 30);
            mMaximizeButton.Location = new Vector2(Width - mCloseButton.Width - mMaximizeButton.Width, 0);
            mMinimizeButton.Size = new Vector2(30, 30);
            mMinimizeButton.Location = new Vector2(Width - mCloseButton.Width - mMaximizeButton.Width - mMinimizeButton.Width, 0);

            mTitle.Size = new Vector2(Width - mMinimizeButton.Width - mMaximizeButton.Width - mCloseButton.Width, 30);
            mTitle.Location = new Vector2(0, 0);
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
