using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace GLUI
{
    public abstract class Component
    {
        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mColorsId = 0;
        private int mIndicesCount = 0;

        private static Stack<Scissor> mScissorStack = new Stack<Scissor>();
        public Component Parent { get; private set; } = null;
        public ObservableCollection<Component> Children { get; } = new ObservableCollection<Component>();

        public int X { get { return Location.X; } set { Location = new Point(value, Y); } }
        public int Y { get { return Location.Y; } set { Location = new Point(X, value); } }
        public Point Location { get { return mLocation; } set { mLocation = value; Dirty = true; } }
        private Point mLocation;
        public Point AbsoluteLocation { get { if (Dirty) { mAbsoluteLocation = (Parent?.AbsoluteLocation ?? new Point(0, 0)) + new Size(X, Y); } return mAbsoluteLocation; } }
        private Point mAbsoluteLocation;
        public int Width { get { return Size.Width; } set { Size = new Size(value, Height); } }
        public int Height { get { return Size.Height; } set { Size = new Size(Width, value); } }
        public Size Size { get { return mSize; } set { mSize = value; Dirty = true; } }
        private Size mSize;

        public int BorderWidth { get { return mBorderWidth; } set { mBorderWidth = value; Dirty = true; } }
        private int mBorderWidth;

        public bool Dirty { get; set; }
        public bool Visible { get { return mVisible; } set { mVisible = value; mInvisible = !value; } }
        private bool mVisible;
        public bool Invisible { get { return mInvisible; } set { mInvisible = value; mVisible = !value; } }
        private bool mInvisible;
        public bool Highlightable { get; set; }
        public bool Highlighted { get; set; }
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; mDisabled = !value; } }
        private bool mEnabled;
        public bool Disabled { get { return mDisabled; } set { mDisabled = value; mEnabled = !value; } }
        private bool mDisabled;

        public Color BackgroundColor { get; set; }
        private Color mOriginalBackgroundColor;
        public Color ForegroundColor { get; set; }
        private Color mOriginalForegroundColor;
        public Color BorderColor { get; set; }
        private Color mOriginalBorderColor;

        public Component()
        {
            Location = new Point(0, 0);
            Size = new Size(0, 0);

            Dirty = true;
            Visible = true;
            Highlightable = false;
            Highlighted = false;
            Enabled = true;

            BackgroundColor = Color.FromArgb(128, 128, 128);
            ForegroundColor = Color.FromArgb(128, 128, 128);
            BorderColor = Color.FromArgb(150, 150, 150);
            BorderWidth = 3;

            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (Component wComponent in e.NewItems)
                        {
                            wComponent.Parent = this;
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (Component wComponent in e.OldItems)
                        {
                            wComponent.Parent = null;
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        foreach (Component wComponent in e.NewItems)
                        {
                            wComponent.Parent = this;
                        }
                        foreach (Component wComponent in e.OldItems)
                        {
                            wComponent.Parent = null;
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (Component wComponent in e.OldItems)
                        {
                            wComponent.Parent = null;
                        }
                        break;
                    }
            }
        }

        #region Methods
        protected bool IsMouseOver(MouseState mouseState)
        {
            return (new Rectangle(AbsoluteLocation, Size)).Contains(mouseState.X, mouseState.Y);
        }

        protected void BringFront(MouseState mouseState)
        {
            bool wFoundTheClosestComponent;
            var wComponents = Children;
            do
            {
                wFoundTheClosestComponent = false;
                for (int i = wComponents.Count - 1; i >= 0 && !wFoundTheClosestComponent; --i)
                {
                    var wComponent = wComponents[i];
                    if (wComponent.Visible && wComponent.IsMouseOver(mouseState))
                    {
                        var wSiblings = wComponent.Parent?.Children;
                        wSiblings.Remove(wComponent);
                        wSiblings.Add(wComponent);
                        wComponents = wComponent.Children;
                        wFoundTheClosestComponent = true;
                    }
                }
            } while (wComponents.Count > 1 && wFoundTheClosestComponent);
        }

        protected void BringFront()
        {
            var wSiblings = Parent?.Children;
            wSiblings.Remove(this);
            wSiblings.Add(this);
            Parent?.BringFront();
        }

        protected void PushScissor()
        {
            var wScissor = new Scissor(AbsoluteLocation, Size).Merge(mScissorStack.FirstOrDefault());
            wScissor.Apply();
            mScissorStack.Push(wScissor);
        }

        protected void PopScissor()
        {
            var wScissor = mScissorStack.Pop();
            wScissor.Apply();
        }

        protected void GreyOut()
        {
            mOriginalBackgroundColor = BackgroundColor;
            mOriginalForegroundColor = ForegroundColor;
            mOriginalBorderColor = BorderColor;
            BackgroundColor = Color.FromArgb((int)(BackgroundColor.R * 0.8),
                                             (int)(BackgroundColor.G * 0.8),
                                             (int)(BackgroundColor.B * 0.8));
            ForegroundColor = Color.FromArgb((int)(ForegroundColor.R * 0.8),
                                             (int)(ForegroundColor.G * 0.8),
                                             (int)(ForegroundColor.B * 0.8));
            BorderColor = Color.FromArgb((int)(BorderColor.R * 0.8),
                                         (int)(BorderColor.G * 0.8),
                                         (int)(BorderColor.B * 0.8));
        }

        protected void ResetColors()
        {
            BackgroundColor = mOriginalBackgroundColor;
            ForegroundColor = mOriginalForegroundColor;
            BorderColor = mOriginalBorderColor;
        }

        /// <summary>
        /// Handles the keyboard events
        /// </summary>
        /// <param name="keyboardState"></param>
        protected virtual void OnKeyboard(KeyboardState keyboardState) { }

        /// <summary>
        /// Handles the mouse events
        /// </summary>
        /// <param name="mouseState"></param>
        protected virtual void OnMouse(MouseState mouseState) { }

        /// <summary>
        /// Renders the component
        /// </summary>
        protected virtual void OnRender()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.VertexPointer(2, VertexPointerType.Int, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorsId);
            GL.ColorPointer(4, ColorPointerType.UnsignedByte, 0, 0);

            GL.DrawElements(BeginMode.Triangles, mIndicesCount, DrawElementsType.UnsignedInt, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }

        /// <summary>
        /// Updates the component
        /// </summary>
        protected virtual void OnUpdate()
        {
            // The background
            var wVertices = new List<int>
            {
                AbsoluteLocation.X + BorderWidth, AbsoluteLocation.Y + BorderWidth,                     // 0, Upper left
                AbsoluteLocation.X + Width - BorderWidth, AbsoluteLocation.Y + BorderWidth,             // 1, Upper right
                AbsoluteLocation.X + Width - BorderWidth, AbsoluteLocation.Y + Height - BorderWidth,    // 2, Bottom right
                AbsoluteLocation.X + BorderWidth, AbsoluteLocation.Y + Height - BorderWidth             // 3, Bottom left
            };
            var wIndices = new List<uint>
            {
                0,1,2, 2,3,0
            };
            var wColors = new List<byte>();
            for (int i = 0; i < 4; ++i)
            {
                wColors.Add(BackgroundColor.R);
                wColors.Add(BackgroundColor.G);
                wColors.Add(BackgroundColor.B);
                wColors.Add(BackgroundColor.A);
            }

            // The border
            wVertices.AddRange(new List<int>
            {
                AbsoluteLocation.X + BorderWidth, AbsoluteLocation.Y + BorderWidth,                     // 4, same as 0
                AbsoluteLocation.X + Width - BorderWidth, AbsoluteLocation.Y + BorderWidth,             // 5, same as 1
                AbsoluteLocation.X + Width - BorderWidth, AbsoluteLocation.Y + Height - BorderWidth,    // 6, same as 2
                AbsoluteLocation.X + BorderWidth, AbsoluteLocation.Y + Height - BorderWidth,            // 7, same as 3
                AbsoluteLocation.X, AbsoluteLocation.Y,                                                 // 8
                AbsoluteLocation.X + Width, AbsoluteLocation.Y,                                         // 9
                AbsoluteLocation.X + Width, AbsoluteLocation.Y + BorderWidth,                           // 10
                AbsoluteLocation.X + Width, AbsoluteLocation.Y + Height - BorderWidth,                  // 11
                AbsoluteLocation.X + Width, AbsoluteLocation.Y + Height,                                // 12
                AbsoluteLocation.X, AbsoluteLocation.Y + Height,                                        // 13
                AbsoluteLocation.X, AbsoluteLocation.Y + Height - BorderWidth,                          // 14
                AbsoluteLocation.X, AbsoluteLocation.Y + BorderWidth,                                   // 15
            });
            wIndices.AddRange(new List<uint>
            {
                15,  8,  9,  9, 10, 15,    // Top
                 5, 10, 11, 11,  6,  5,    // Right
                11, 12, 13, 13, 14, 11,    // Bottom
                 7, 14, 15, 15,  4,  7     // Left
            });
            for (int i = 0; i < 12; ++i)
            {
                wColors.Add(BorderColor.R);
                wColors.Add(BorderColor.G);
                wColors.Add(BorderColor.B);
                wColors.Add(BorderColor.A);
            }

            if (mVerticesId != 0)
            {
                GL.DeleteBuffer(mVerticesId);
            }
            if (mIndicesId != 0)
            {
                GL.DeleteBuffer(mIndicesId);
            }
            if (mColorsId != 0)
            {
                GL.DeleteBuffer(mColorsId);
            }

            mVerticesId = GL.GenBuffer();
            mIndicesId = GL.GenBuffer();
            mColorsId = GL.GenBuffer();
            mIndicesCount = wIndices.Count;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(int) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorsId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(byte) * wColors.Count, wColors.ToArray(), BufferUsageHint.DynamicDraw);
        }

        /// <summary>
        /// Calls OnKeyboard and calls KeyboardHandler on child components if the component is visible
        /// </summary>
        /// <param name="sender">The object who raised the event</param>
        /// <param name="keyboardState">The state of the keyboard</param>
        internal void KeyboardHandler(object sender, KeyboardState keyboardState)
        {
            if (Invisible || Disabled) return;
            OnKeyboard(keyboardState);
            foreach (var wChild in Children)
            {
                wChild.KeyboardHandler(sender, keyboardState);
            }
        }

        /// <summary>
        /// Calls OnMouse and calls MouseHandler on the child components if the component is visible
        /// </summary>
        /// <param name="sender">The object who raised the event</param>
        /// <param name="mouseState">The state of the mouse</param>
        internal void MouseHandler(object sender, MouseState mouseState)
        {
            if (Invisible || Disabled) return;
            mouseState.IsOver = IsMouseOver(mouseState);
            OnMouse(mouseState);
            foreach (var wChild in Children)
            {
                wChild.MouseHandler(sender, mouseState);
            }
        }

        /// <summary>
        /// Calls OnRender and calls Render on the child components if the component is visible
        /// </summary>
        internal void Render()
        {
            if (Invisible) return;
            PushScissor();
            if (Disabled) GreyOut();
            OnRender();
            if (Disabled) ResetColors();
            foreach (var wChild in Children)
            {
                wChild.Render();
            }
            PopScissor();
        }

        /// <summary>
        /// Calls OnUpdate if the Dirty flag is true and calls Update on the child components if the component is visible
        /// </summary>
        internal void Update()
        {
            if (Invisible) return;
            if (Dirty) OnUpdate();
            foreach (var wChild in Children)
            {
                wChild.Update();
            }
        }
        #endregion
    }
}
