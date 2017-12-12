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
        protected int[] mVertices = new int[0];
        protected Dictionary<string, Range<int>> mVerticesRanges = new Dictionary<string, Range<int>>();
        protected uint[] mIndices = new uint[0];
        protected Dictionary<string, Range<int>> mIndicesRanges = new Dictionary<string, Range<int>>();
        protected byte[] mColors = new byte[0];

        Stack<Scissor> mScissorStack = new Stack<Scissor>();
        public Component Parent { get; private set; } = null;
        public ObservableCollection<Component> Children { get; } = new ObservableCollection<Component>();

        public int X { get { return Location.X; } set { Location = new Point(value, Y); } }
        public int Y { get { return Location.Y; } set { Location = new Point(X, value); } }
        public Point Location { get; set; }
        public Point AbsoluteLocation { get { if (Dirty) { mAbsoluteLocation = (Parent?.AbsoluteLocation ?? new Point(0, 0)) + new Size(X, Y); } return mAbsoluteLocation; } }
        private Point mAbsoluteLocation;
        public int Width { get { return Size.Width; } set { Size = new Size(value, Height); } }
        public int Height { get { return Size.Height; } set { Size = new Size(Width, value); } }
        public Size Size { get; set; }

        public double BorderWidth { get; set; }

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

            BorderWidth = 1;

            Dirty = true;
            Visible = true;
            Highlightable = false;
            Highlighted = false;
            Enabled = true;

            BackgroundColor = Color.FromArgb(128, 128, 128);
            ForegroundColor = Color.FromArgb(128, 128, 128);
            BorderColor = Color.FromArgb(150, 150, 150);
            BorderWidth = 1;

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
            var wScissor = new Scissor(Location, Size);
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
            GL.EnableClientState(ArrayCap.ColorArray);

            GL.VertexPointer(2, VertexPointerType.Int, 0, mVertices);
            GL.ColorPointer(3, ColorPointerType.UnsignedByte, 0, mColors);

            GL.DrawRangeElements<uint>(PrimitiveType.Triangles, mVerticesRanges["background"].Minimum, mVerticesRanges["background"].Maximum, mIndicesRanges["background"].Maximum - mIndicesRanges["background"].Minimum + 1, DrawElementsType.UnsignedInt, mIndices);

            //GL.DrawRangeElements<uint>(PrimitiveType.Triangles, mVerticesRanges["border"].Minimum, mVerticesRanges["border"].Maximum, mIndicesRanges["border"].Maximum - mIndicesRanges["border"].Minimum, DrawElementsType.UnsignedInt, mIndices);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }

        /// <summary>
        /// Updates the component
        /// </summary>
        protected virtual void OnUpdate()
        {
            var wVertices = new List<int>
            {
                AbsoluteLocation.X,
                AbsoluteLocation.Y,

                AbsoluteLocation.X + Width,
                AbsoluteLocation.Y,

                AbsoluteLocation.X + Width,
                AbsoluteLocation.Y + Height,

                AbsoluteLocation.X,
                AbsoluteLocation.Y + Height,
            };

            var wColors = new List<Color>
            {
                BackgroundColor,
                BackgroundColor,
                BackgroundColor,
                BackgroundColor,
            };

            var wIndices = new List<uint>
            {
                0,1,2,
                2,3,0
            };

            mVertices = wVertices.ToArray();
            mIndices = wIndices.ToArray();
            var wColorsTemp = new List<byte>();
            wColors.ForEach(color => { wColorsTemp.Add(color.R); wColorsTemp.Add(color.G); wColorsTemp.Add(color.B); });
            mVerticesRanges["background"] = new Range<int>(0, 4);
            mIndicesRanges["background"] = new Range<int>(0, 6);
            mColors = wColorsTemp.ToArray();
        }

        /// <summary>
        /// Calls OnKeyboard and calls KeyboardHandler on child components if the component is visible
        /// </summary>
        /// <param name="sender">The object who raised the event</param>
        /// <param name="keyboardState">The state of the keyboard</param>
        internal void KeyboardHandler(object sender, KeyboardState keyboardState)
        {
            if (Invisible) return;
            if (Disabled) return;
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
            if (Invisible) return;
            if (Disabled) return;
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
