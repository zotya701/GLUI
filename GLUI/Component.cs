﻿using System;
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
    public abstract class Component : IDisposable
    {
        private bool mDisposed = false;

        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mColorsId = 0;
        private int mIndicesCount = 0;

        private static Stack<Scissor> mScissorStack = new Stack<Scissor>();

        private Point mLocation;
        private Point mAbsoluteLocation;
        private Size mSize;
        private int mBorderWidth;
        private bool mVisible;
        private bool mInvisible;
        private bool mHighlightable;
        private bool mHighlighted;
        private bool mEnabled;
        private bool mDisabled;

        private Color mOriginalBackgroundColor;
        private Color mOriginalBorderColor;


        public Component Parent { get; private set; } = null;
        public ObservableCollection<Component> Children { get; } = new ObservableCollection<Component>();
        public int X { get { return Location.X; } set { Location = new Point(value, Y); } }
        public int Y { get { return Location.Y; } set { Location = new Point(X, value); } }
        public Point Location { get { return mLocation; } set { if (mLocation == value) return; mLocation = value; Dirty = true; } }
        public Point AbsoluteLocation { get { if (Dirty) { mAbsoluteLocation = (Parent?.AbsoluteLocation ?? new Point(0, 0)) + new Size(X, Y); } return mAbsoluteLocation; } }
        public int Width { get { return Size.Width; } set { Size = new Size(value, Height); } }
        public int Height { get { return Size.Height; } set { Size = new Size(Width, value); } }
        public Size Size { get { return mSize; } set { if (mSize == value) return; mSize = value; Dirty = true; } }
        public int BorderWidth { get { return mBorderWidth; } set { if (mBorderWidth == value) return; mBorderWidth = value; Dirty = true; } }

        public bool Dirty { get; set; }
        public bool Visible { get { return mVisible; } set { mVisible = value; mInvisible = !value; } }
        public bool Invisible { get { return mInvisible; } set { mInvisible = value; mVisible = !value; } }
        public bool Highlightable { get { return mHighlightable; } set { if (mHighlightable == value) return; mHighlightable = value; Dirty = true; } }
        public bool Highlighted { get { return mHighlighted; } set { if (mHighlighted == value) return; mHighlighted = value; Dirty = true; if (value) Highlight(); else ResetColors(); } }
        public bool Enabled { get { return mEnabled; } set { mEnabled = value; mDisabled = !value; } }
        public bool Disabled { get { return mDisabled; } set { mDisabled = value; mEnabled = !value; } }

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }

        public Component()
        {
            Location = new Point(0, 0);
            Size = new Size(0, 0);

            Dirty = true;
            Visible = true;
            Highlightable = false;
            Highlighted = false;
            Enabled = true;

            BackgroundColor = Color.FromArgb(100, 100, 100, 100);
            BorderColor = Color.FromArgb(150, 150, 150);
            BorderWidth = 3;

            Children.CollectionChanged += Children_CollectionChanged;
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
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

        protected bool IsMouseOverDirectly(MouseState mouseState)
        {
            if (mouseState.IsOver == false)
            {
                return false;
            }
            var wIsNotOver = true;
            var wComponent = this;
            while (wComponent != null && wIsNotOver)
            {
                var wSiblingsToTheRight = wComponent.GetSiblingsToTheRight();
                if(wSiblingsToTheRight != null)
                {
                    for (int i = wSiblingsToTheRight.Count - 1; i >= 0 && wIsNotOver; --i)
                    {
                        wIsNotOver = wIsNotOver && !wSiblingsToTheRight[i].IsMouseOver(mouseState);
                    }
                }
                else
                {
                    return wIsNotOver;
                }
                wComponent = wComponent.Parent;
            }
            return wIsNotOver;
        }

        protected List<Component> GetSiblingsToTheRight()
        {
            return Parent?.Children.Skip(Parent.Children.IndexOf(this)).Skip(1).ToList();
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
            mOriginalBorderColor = BorderColor;
            BackgroundColor = Color.FromArgb((int)(BackgroundColor.R * 0.8),
                                             (int)(BackgroundColor.G * 0.8),
                                             (int)(BackgroundColor.B * 0.8));
            BorderColor = Color.FromArgb((int)(BorderColor.R * 0.8),
                                         (int)(BorderColor.G * 0.8),
                                         (int)(BorderColor.B * 0.8));
            Dirty = true;
        }

        protected void Highlight()
        {
            mOriginalBackgroundColor = BackgroundColor;
            mOriginalBorderColor = BorderColor;
            BackgroundColor = Color.FromArgb(255, (int)(BackgroundColor.R * 1.2),
                                             (int)(BackgroundColor.G * 1.2),
                                             (int)(BackgroundColor.B * 1.2));
            BorderColor = Color.FromArgb((int)(BorderColor.R * 1.2),
                                         (int)(BorderColor.G * 1.2),
                                         (int)(BorderColor.B * 1.2));
            Dirty = true;
        }

        protected void ResetColors()
        {
            BackgroundColor = mOriginalBackgroundColor;
            BorderColor = mOriginalBorderColor;
            Dirty = true;
        }

        private Size CalculateSize()
        {
            var wBottomRight = new Point(0, 0);
            var wQueue = new Queue<Component>();
            wQueue.Enqueue(this);
            while (wQueue.Any())
            {
                var wCurrent = wQueue.Dequeue();
                if (wCurrent == null)
                    continue;
                foreach (var wChild in wCurrent.Children)
                {
                    wQueue.Enqueue(wChild);
                }

                wBottomRight = new Point(Math.Max(wBottomRight.X, wCurrent.AbsoluteLocation.X + wCurrent.Width),
                                         Math.Max(wBottomRight.Y, wCurrent.AbsoluteLocation.Y + wCurrent.Height));
            }
            return new Size(wBottomRight.X - AbsoluteLocation.X, wBottomRight.Y - AbsoluteLocation.Y);
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
        protected virtual void OnMouse(MouseState mouseState) { if (Highlightable) Highlighted = mouseState.IsOverDirectly; }

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

            if (mVerticesId == 0) mVerticesId = GL.GenBuffer();
            if (mIndicesId == 0) mIndicesId = GL.GenBuffer();
            if (mColorsId == 0) mColorsId = GL.GenBuffer();
            mIndicesCount = wIndices.Count;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(int) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorsId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(byte) * wColors.Count, wColors.ToArray(), BufferUsageHint.StaticDraw);
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
            mouseState.IsOverDirectly = IsMouseOverDirectly(mouseState);
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
            if (Dirty) Dirty = false;
            Size = CalculateSize();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (mDisposed) return;

            if (disposing)
            {
                if (mVerticesId != 0) GL.DeleteBuffer(mVerticesId);
                if (mIndicesId != 0) GL.DeleteBuffer(mIndicesId);
                if (mColorsId != 0) GL.DeleteBuffer(mColorsId);
            }
            mDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
