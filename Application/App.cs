using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using GLUI;
using System.Drawing;

namespace Application
{
    public class App
    {
        private GameWindow mWindow;
        private RootComponent mRoot;
        private GLUI.KeyboardState mKeyboardState;
        private GLUI.MouseState mMouseState;

        public event KeyboardHandler OnKeyboard;
        public event MouseHandler OnMouse;

        #region Properties
        public int X { get { return mWindow.X; } set { mWindow.X = value; } }
        public int Y { get { return mWindow.Y; } set { mWindow.Y = value; } }
        public Point Location { get { return mWindow.Location; } set { mWindow.Location = value; } }
        public int Width { get { return mWindow.Width; } set { mWindow.Width = value; } }
        public int Height { get { return mWindow.Height; } set { mWindow.Height = value; } }
        public Size Size { get { return mWindow.ClientSize; } set { mWindow.ClientSize = value; } }
        public bool FullScreen { get { return mWindow.WindowState == WindowState.Fullscreen; } set { if (value) { mWindow.WindowState = WindowState.Fullscreen; } else { mWindow.WindowState = WindowState.Normal; } } }
        public bool VSync { get { return mWindow.VSync == VSyncMode.On; } set { if (value) { mWindow.VSync = VSyncMode.On; } else { mWindow.VSync = VSyncMode.Off; } } }
        public double FPS { get { return mWindow.TargetRenderFrequency; } set { mWindow.TargetRenderFrequency = value; mWindow.TargetUpdateFrequency = value; } }
        public bool ShowFPS { get; set; }
        public string Title { get { return mWindow.Title; } set { mWindow.Title = value; } }
        public System.Drawing.Icon Icon { get { return mWindow.Icon; } set { mWindow.Icon = value; } }
        public bool CursorVisible { get { return mWindow.CursorVisible; } set { mWindow.CursorVisible = value; } }
        public Dictionary<string, Command> Commands { get; set; }
        #endregion

        public App()
        {
            mKeyboardState = new GLUI.KeyboardState();
            mMouseState = new GLUI.MouseState();
            mWindow = new GameWindow();
            Commands = new Dictionary<string, Command>();

            mWindow.Load += OnLoad;

            mWindow.Resize += OnResize;
            mWindow.UpdateFrame += OnUpdateFrame;
            mWindow.RenderFrame += OnRenderFrame;

            mWindow.KeyDown += OnKeyDown;
            mWindow.KeyUp += OnKeyUp;
            mWindow.KeyPress += OnKeyPress;

            mWindow.MouseDown += OnMouseDown;
            mWindow.MouseUp += OnMouseUp;
            mWindow.MouseWheel += OnMouseWheel;
            mWindow.MouseMove += OnMouseMove;
            mWindow.MouseEnter += OnMouseEnter;
            mWindow.MouseLeave += OnMouseLeave;

            mRoot = new RootComponent
            {
                BackgroundColor = Color.FromArgb(0, 0, 0, 0),
                ForegroundColor = Color.FromArgb(0, 0, 0, 0),
                BorderColor = Color.FromArgb(0, 0, 0, 0),
                BorderWidth = 0,
            };
            OnKeyboard += mRoot.KeyboardHandler;
            OnMouse += mRoot.MouseHandler;
        }
        public void Run()
        {
            Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
            Console.WriteLine("Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Shading language version: " + GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine("Renderer: " + GL.GetString(StringName.Renderer));
            //Console.WriteLine("Extensions:");
            //var wExtensions = GL.GetString(StringName.Extensions).Split(' ');
            //foreach (var wExtension in wExtensions)
            //{
            //    Console.WriteLine($"  {wExtension}");
            //}
            Console.WriteLine("Commands:");
            foreach (var wControlKey in Commands)
            {
                Console.WriteLine($"  {wControlKey.Key} -> {wControlKey.Value}");
            }

            mRoot.Size = Size;
            mWindow.Run();
        }
        public void Exit()
        {
            mWindow.Exit();
        }
        public void AddComponent(Component component)
        {
            mRoot.Children.Add(component);
        }
        public void RemoveComponent(Component component)
        {
            mRoot.Children.Remove(component);
        }

        #region EventHandlers
        private void OnLoad(object sender, EventArgs e)
        {
            
        }
        private void OnResize(object sender, EventArgs e)
        {
            mRoot.Size = Size;
            GL.Viewport(mWindow.ClientRectangle);
        }
        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            mRoot.Update();
        }
        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            GL.ClearColor(Color4.Green);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.ScissorTest);
            mRoot.Render();
            GL.Disable(EnableCap.ScissorTest);


            //var wViewport = new int[4];
            //GL.GetInteger(GetPName.Viewport, wViewport);
            //GL.MatrixMode(MatrixMode.Projection);
            //GL.LoadIdentity();
            //GL.Viewport(wViewport[0], wViewport[1], wViewport[2], wViewport[3]);
            //GL.MatrixMode(MatrixMode.Modelview);
            //GL.LoadIdentity();
            //GL.Ortho(0, wViewport[2], wViewport[3], 0, 1, -1);

            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            //GL.Disable(EnableCap.Lighting);
            //GL.Enable(EnableCap.Texture2D);
            //GL.BindTexture(TextureTarget.Texture2D, 3);
            //GL.Begin(BeginMode.Quads);
            //GL.TexCoord2(0, 0);
            //GL.Vertex2(0, 0);
            //GL.TexCoord2(1, 0);
            //GL.Vertex2(wViewport[2], 0);
            //GL.TexCoord2(1, 1);
            //GL.Vertex2(wViewport[2], wViewport[3]);
            //GL.TexCoord2(0, 1);
            //GL.Vertex2(0, wViewport[3]);
            //GL.End();

            mWindow.SwapBuffers();
        }
        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            mKeyboardState.Alt = e.Alt;
            mKeyboardState.Control = e.Control;
            mKeyboardState.Shift = e.Shift;
            mKeyboardState.KeyDown[e.Key] = true;

            foreach(var wControlKey in Commands.Values)
            {
                wControlKey.Check(mKeyboardState);
            }

            OnKeyboard?.Invoke(this, mKeyboardState);
        }
        private void OnKeyUp(object sender, KeyboardKeyEventArgs e)
        {
            mKeyboardState.Alt = e.Alt;
            mKeyboardState.Control = e.Control;
            mKeyboardState.Shift = e.Shift;
            mKeyboardState.KeyDown[e.Key] = false;

            OnKeyboard?.Invoke(this, mKeyboardState);
        }
        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            mKeyboardState.KeyChar = e.KeyChar;
            mKeyboardState.IsPressed = true;

            OnKeyboard?.Invoke(this, mKeyboardState);

            mKeyboardState.IsPressed = false;
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            mMouseState.X = e.X;
            mMouseState.Y = e.Y;
            mMouseState.Button = e.Button;
            mMouseState.IsPressed = e.IsPressed;
            mMouseState.ButtonDown[e.Button] = true;

            OnMouse?.Invoke(this, mMouseState);

            mMouseState.IsPressed = false;
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mMouseState.X = e.X;
            mMouseState.Y = e.Y;
            mMouseState.Button = e.Button;
            mMouseState.IsPressed = e.IsPressed;
            mMouseState.ButtonDown[e.Button] = false;

            OnMouse?.Invoke(this, mMouseState);

            mMouseState.IsPressed = false;
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            mMouseState.X = e.X;
            mMouseState.Y = e.Y;
            mMouseState.Delta = e.Delta;
            mMouseState.IsScrolled = true;

            OnMouse?.Invoke(this, mMouseState);

            mMouseState.IsScrolled = false;
        }
        private void OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            mMouseState.X = e.X;
            mMouseState.Y = e.Y;
            mMouseState.XDelta = e.XDelta;
            mMouseState.YDelta = e.YDelta;
            mMouseState.IsMoved = true;

            OnMouse?.Invoke(this, mMouseState);

            mMouseState.IsMoved = false;
        }
        private void OnMouseEnter(object sender, EventArgs e)
        {

        }
        private void OnMouseLeave(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
