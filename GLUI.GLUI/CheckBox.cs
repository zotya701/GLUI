using GLUI.Foundation;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.GLUI
{
    public class CheckBox : ButtonBase
    {
        private bool mDisposed = false;

        private Button mCheckButton;
        private bool mIsChecked = false;
        public bool IsChecked
        {
            get
            {
                return mIsChecked;
            }
            set
            {
                if (mIsChecked == value) return;

                mIsChecked = value;
                Dirty = true;
                CheckBoxChanged?.Invoke(this, new EventArgs());
                if (mIsChecked)
                {
                    CheckBoxChecked?.Invoke(this, new EventArgs());
                }
                else
                {
                    CheckBoxUnchecked?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler CheckBoxChecked;
        public event EventHandler CheckBoxUnchecked;
        public event EventHandler CheckBoxChanged;

        public CheckBox(string text)
        {
            Highlightable = true;
            BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            BorderWidth = 0;

            Label.Text = text;

            mCheckButton = new Button(string.Empty)
            {
                Highlightable = Highlightable,
            };
            mCheckButton.AddDrawingAction(() =>
            {
                if (IsChecked)
                {
                    int[] wData = new int[4];
                    GL.GetInteger(GetPName.CurrentColor, wData);
                    GL.Color4(Label.FontColor.R, Label.FontColor.G, Label.FontColor.B, Label.FontColor.A);
                    GL.Begin(PrimitiveType.Polygon);
                    GL.Vertex2(mCheckButton.Width * 0.09, mCheckButton.Height * 0.50);
                    GL.Vertex2(mCheckButton.Width * 0.05, mCheckButton.Height * 0.55);
                    GL.Vertex2(mCheckButton.Width * 0.45, mCheckButton.Height * 0.95);
                    GL.Vertex2(mCheckButton.Width * 0.52, mCheckButton.Height * 0.95);
                    GL.End();
                    GL.Begin(PrimitiveType.Polygon);
                    GL.Vertex2(mCheckButton.Width * 0.45, mCheckButton.Height * 0.95);
                    GL.Vertex2(mCheckButton.Width * 0.52, mCheckButton.Height * 0.95);
                    GL.Vertex2(mCheckButton.Width * 0.95, mCheckButton.Height * 0.14);
                    GL.Vertex2(mCheckButton.Width * 0.91, mCheckButton.Height * 0.10);
                    GL.End();
                    GL.Color4(wData[0], wData[1], wData[2], wData[3]);
                }
            });

            Children.Add(mCheckButton);

            ButtonClicked += (o, e) =>
            {
                IsChecked = !IsChecked;
            };

            Label.mLabelPressed += (o, e) =>
            {
                mCheckButton.OnPressed(o, e);
            };

            Label.mLabelReleased += (o, e) =>
            {
                mCheckButton.OnReleased(o, e);
            };

            MouseLeaved += (o, e) =>
            {
                if (!Pressed) mCheckButton.Highlighted = false;
            };
        }

        protected override void GreyOut()
        {
            //base.GreyOut();
        }

        protected override void Highlight()
        {
            //base.Highlight();
        }

        protected override void ResetColors()
        {
            //base.ResetColors();
        }

        protected override void OnKeyboard(KeyboardState keyboardState)
        {
            base.OnKeyboard(keyboardState);
        }

        protected override void OnMouse(MouseState mouseState)
        {
            base.OnMouse(mouseState);
            if(Pressed || mouseState.IsOverDirectly)
            {
                mCheckButton.Highlighted = true;
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
        }

        protected override void OnUpdate()
        {
            if (Label.Size.X == 0 && Label.Size.Y == 0)
            {
                Size = CalculateSize() + new Vector2(10, 5);
                Size = Size + new Vector2(Size.Y, 0);
            }
            mCheckButton.Location = new Vector2(0, 0);
            mCheckButton.Size = new Vector2(Size.Y, Size.Y);
            Label.Location = new Vector2(Size.Y, 0);
            Label.Size = new Vector2(Size.X - Size.Y, Size.Y);

            Size = new Vector2(Math.Max(Width, Label.Size.X),
                               Math.Max(Height, Label.Size.Y));
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
