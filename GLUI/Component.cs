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
using Foundation;

namespace GLUI
{
    public abstract class Component : IDisposable
    {
        private bool mDisposed = false;

        private bool mMouseInside = false;

        private bool mLocationChanged = true;

        private bool LocationChanged
        {
            get
            {
                return mLocationChanged;
            }
            set
            {
                if (mLocationChanged == value) return;

                mLocationChanged = value;
                if (mLocationChanged)
                {
                    foreach (var wChild in Children)
                    {
                        wChild.LocationChanged = true;
                    }
                }
            }
        }

        private int mVerticesId = 0;
        private int mIndicesId = 0;
        private int mColorsId = 0;
        private int mIndicesCount = 0;

        private static HashSet<Component> mClickThroughList = new HashSet<Component>();
        private static Stack<Scissor> mScissorStack = new Stack<Scissor>();
        protected List<Action> mDrawingActions = new List<Action>();

        private Vector2 mLocation;
        private Vector2 mAbsoluteLocation;
        private Vector2 mSize;
        private float mBorderWidth;
        private bool mVisible;
        private bool mHighlightable;
        private bool mHighlighted;
        private bool mEnabled;

        private Color mOriginalBackgroundColor;
        private Color mOriginalBorderColor;

        public event MouseHandler MouseEntered;
        public event MouseHandler MouseLeaved;

        /// <summary>
        /// The component's parent.
        /// </summary>
        public Component Parent { get; private set; } = null;

        /// <summary>
        /// The component's children.
        /// </summary>
        public ObservableCollection<Component> Children { get; } = new ObservableCollection<Component>();

        /// <summary>
        /// The X coordinate of the component's location.
        /// </summary>
        public float X
        {
            get
            {
                return Location.X;
            }
            set
            {
                Location = new Vector2(value, Y);
            }
        }

        /// <summary>
        /// The Y coordinate of the component's location.
        /// </summary>
        public float Y
        {
            get
            {
                return Location.Y;
            }
            set
            {
                Location = new Vector2(X, value);
            }
        }

        /// <summary>
        /// The component's location, containing the X and Y coordinates, relative to it's parent component's top left corner.
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return mLocation;
            }
            set
            {
                if (mLocation == value) return;

                mLocation = value;
                LocationChanged = true;
            }
        }

        // The component's location, containing the X and Y coordinates, relative to application window's top left corner.
        public Vector2 AbsoluteLocation
        {
            get
            {
                if (LocationChanged)
                {
                    mAbsoluteLocation = (Parent?.AbsoluteLocation ?? new Vector2(0, 0)) + Location;
                    LocationChanged = false;
                }
                return mAbsoluteLocation;
            }
        }

        /// <summary>
        /// The width of the component.
        /// </summary>
        public float Width
        {
            get
            {
                return Size.X;
            }
            set
            {
                Size = new Vector2(value, Height);
            }
        }

        /// <summary>
        /// The Height of the component.
        /// </summary>
        public float Height
        {
            get
            {
                return Size.Y;
            }
            set
            {
                Size = new Vector2(Width, value);
            }
        }

        /// <summary>
        /// The size of the component, containing the Width and Height values.
        /// </summary>
        public Vector2 Size
        {
            get
            {
                return mSize;
            }
            set
            {
                if (mSize == value) return;

                mSize = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// The component's border's width.
        /// </summary>
        public float BorderWidth
        {
            get
            {
                return mBorderWidth;
            }
            set
            {
                if (mBorderWidth == value) return;

                mBorderWidth = value;
                Dirty = true;
            }
        }


        protected internal bool Dirty { get; set; }
        public bool Visible
        {
            get
            {
                return mVisible;
            }
            set
            {
                if (mVisible == value) return;

                mVisible = value;
            }
        }
        public bool Invisible
        {
            get
            {
                return !Visible;
            }
            set
            {
                Visible = !value;
            }
        }
        public bool Highlightable
        {
            get
            {
                return mHighlightable;
            }
            set
            {
                if (mHighlightable == value) return;

                mHighlightable = value;
                Dirty = true;
            }
        }
        public bool Highlighted
        {
            get
            {
                return mHighlighted;
            }
            set
            {
                if (mHighlighted == value) return;

                mHighlighted = value;
                Dirty = true;
                if (value)
                {
                    Highlight();
                }
                else
                {
                    ResetColors();
                }
            }
        }
        public bool Enabled
        {
            get
            {
                return mEnabled;
            }
            set
            {
                if (mEnabled == value) return;

                mEnabled = value;
                Dirty = true;
            }
        }
        public bool Disabled
        {
            get
            {
                return !Enabled;
            }
            set
            {
                Enabled = !value;
            }
        }
        public bool ClickThrough
        {
            get
            {
                return mClickThroughList.Contains(this);
            }
            set
            {
                if (value && !ClickThrough) mClickThroughList.Add(this);

                if(ClickThrough == false && value == true)
                {
                    mClickThroughList.Add(this);
                }
                else if(ClickThrough == true && value == false)
                {
                    mClickThroughList.Remove(this);
                }
            }
        }

        public Color BackgroundColor { get; set; }
        public Color BorderColor { get; set; }

        public Component()
        {
            Location = new Vector2(0, 0);
            Size = new Vector2(0, 0);

            Dirty = true;
            Visible = true;
            Highlightable = false;
            Highlighted = false;
            Enabled = true;

            BackgroundColor = Color.FromArgb(100, 100, 100, 100);
            BorderColor = Color.FromArgb(150, 150, 150);
            BorderWidth = 1.5f;

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
        public void AddDrawingAction(Action action)
        {
            mDrawingActions.Add(action);
        }

        public void RemoveDrawingAction(Action action)
        {
            mDrawingActions.Remove(action);
        }

        protected bool IsMouseOver(MouseState mouseState)
        {
            return ClickThrough ? false : new Box2(AbsoluteLocation, AbsoluteLocation + Size).Contains(new Vector2(mouseState.X, mouseState.Y));
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
                if (wSiblingsToTheRight != null)
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

        public void BringFront()
        {
            if (Parent == null) return;
            var wSiblings = Parent.Children;
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

        protected Vector2 CalculateSize()
        {
            var wBottomRight = new Vector2(0, 0);
            var wQueue = new Queue<Component>();
            wQueue.Enqueue(this);
            while (wQueue.Any())
            {
                var wCurrent = wQueue.Dequeue();
                if (wCurrent == null) continue;
                foreach (var wChild in wCurrent.Children)
                {
                    wQueue.Enqueue(wChild);
                }
                if (wCurrent != this) wCurrent.OnUpdate();
                wBottomRight.X = Math.Max(wBottomRight.X, wCurrent.AbsoluteLocation.X + wCurrent.Width);
                wBottomRight.Y = Math.Max(wBottomRight.Y, wCurrent.AbsoluteLocation.Y + wCurrent.Height);
            }
            return wBottomRight - AbsoluteLocation;
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
        protected virtual void OnMouse(MouseState mouseState)
        {
            if (mMouseInside == false && mouseState.IsOverDirectly == true)
            {
                mMouseInside = true;
                OnMouseEntered(mouseState);
            }
            else if (mMouseInside == true && mouseState.IsOverDirectly == false)
            {
                mMouseInside = false;
                OnMouseLeaved(mouseState);
            }
        }

        protected virtual void OnMouseEntered(MouseState mouseState)
        {
            if (Highlightable) Highlighted = true;
            MouseEntered?.Invoke(this, mouseState);
        }

        protected virtual void OnMouseLeaved(MouseState mouseState)
        {
            if (Highlightable) Highlighted = false;
            MouseLeaved?.Invoke(this, mouseState);
        }

        /// <summary>
        /// Renders the component
        /// </summary>
        protected virtual void OnRender()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.VertexPointer(2, VertexPointerType.Float, 0, 0);

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
            var wVertices = new List<float>
            {                                                   // INDICES
                BorderWidth, BorderWidth,                       // 0, Upper left
                Width - BorderWidth, BorderWidth,               // 1, Upper right
                Width - BorderWidth, Height - BorderWidth,      // 2, Bottom right
                BorderWidth, Height - BorderWidth               // 3, Bottom left
            };
            var wIndices = new List<uint>
            {
                0, 1, 2,  2, 3, 0
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
            if (BorderWidth > 0)
            {
                wVertices.AddRange(new List<float>
                {
                    BorderWidth, BorderWidth,                   // 4, same as 0
                    Width - BorderWidth,  BorderWidth,          // 5, same as 1
                    Width - BorderWidth, Height - BorderWidth,  // 6, same as 2
                    BorderWidth, Height - BorderWidth,          // 7, same as 3
                    0, 0,                                       // 8
                    Width, 0,                                   // 9
                    Width, BorderWidth,                         // 10
                    Width, Height - BorderWidth,                // 11
                    Width, Height,                              // 12
                    0, Height,                                  // 13
                    0, Height - BorderWidth,                    // 14
                    0, BorderWidth,                             // 15
                });
                wIndices.AddRange(new List<uint>
                {
                    15,  8,  9,  9, 10, 15,                     // Top rectangle
                     5, 10, 11, 11,  6,  5,                     // Right
                    11, 12, 13, 13, 14, 11,                     // Bottom
                     7, 14, 15, 15,  4,  7                      // Left
                });
                for (int i = 0; i < 12; ++i)
                {
                    wColors.Add(BorderColor.R);
                    wColors.Add(BorderColor.G);
                    wColors.Add(BorderColor.B);
                    wColors.Add(BorderColor.A);
                }
            }

            if (mVerticesId == 0) mVerticesId = GL.GenBuffer();
            if (mIndicesId == 0) mIndicesId = GL.GenBuffer();
            if (mColorsId == 0) mColorsId = GL.GenBuffer();
            mIndicesCount = wIndices.Count;

            GL.BindBuffer(BufferTarget.ArrayBuffer, mVerticesId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * wVertices.Count, wVertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mIndicesId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * wIndices.Count, wIndices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mColorsId);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(byte) * wColors.Count, wColors.ToArray(), BufferUsageHint.StaticDraw);
        }

        /// <summary>
        /// Calls OnKeyboard and calls KeyboardHandler on child components if the component is visible and enabled
        /// </summary>
        /// <param name="keyboardState">The state of the keyboard</param>
        internal void KeyboardHandler(KeyboardState keyboardState)
        {
            if (Invisible || Disabled) return;
            OnKeyboard(keyboardState);
            foreach (var wChild in Children)
            {
                wChild.KeyboardHandler(keyboardState);
            }
        }

        /// <summary>
        /// Calls OnMouse and calls MouseHandler on the child components if the component is visible and enabled
        /// </summary>
        /// <param name="mouseState">The state of the mouse</param>
        internal void MouseHandler(MouseState mouseState)
        {
            if (Invisible || Disabled) return;
            mouseState.IsOver = IsMouseOver(mouseState);
            mouseState.IsOverDirectly = IsMouseOverDirectly(mouseState);
            OnMouse(mouseState);
            foreach (var wChild in Children)
            {
                wChild.MouseHandler(mouseState);
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
            GL.PushMatrix();
            GL.Translate(X, Y, 0);
            OnRender();
            mDrawingActions.ForEach(wAction => wAction());
            if (Disabled) ResetColors();
            foreach (var wChild in Children)
            {
                wChild.Render();
            }
            GL.PopMatrix();
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
        }

        /// <summary>
        /// Releases the OpenGL resources
        /// </summary>
        /// <param name="disposing"></param>
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

        /// <summary>
        /// Releases the OpenGL resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            foreach(var wChild in Children)
            {
                wChild.Dispose();
            }
        }
        #endregion
    }
}
