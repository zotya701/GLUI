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
        #endregion

        public App()
        {
            mKeyboardState = new GLUI.KeyboardState();
            mMouseState = new GLUI.MouseState();
            mWindow = new GameWindow();

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
            mRoot.ListenTo(ref OnKeyboard);
            mRoot.ListenTo(ref OnMouse);

            Console.WriteLine("Vendor: " + GL.GetString(StringName.Vendor));
            Console.WriteLine("Version: " + GL.GetString(StringName.Version));
            Console.WriteLine("Shading language version: " + GL.GetString(StringName.ShadingLanguageVersion));
            Console.WriteLine("Renderer: " + GL.GetString(StringName.Renderer));
            Console.WriteLine("Extensions:");
            GL.GetString(StringName.Extensions).Split(' ').ToList().ForEach(extension => Console.WriteLine("  " + extension));
        }
        public void Run()
        {
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
            //var wCharSet = "";
            //foreach(var wCharCode in Enumerable.Range(0, Byte.MaxValue + 1))
            //{
            //    if((32 <= wCharCode && wCharCode <= 126) || (160 <= wCharCode && wCharCode <= Byte.MaxValue))
            //    {
            //        wCharSet = wCharSet + Convert.ToChar(wCharCode);
            //    }
            //    else
            //    {
            //        wCharSet = wCharSet + " ";
            //    }
            //}

            //var Font = new Font("Arial", 32, GraphicsUnit.Pixel);
            //var wSize = System.Windows.Forms.TextRenderer.MeasureText(wCharSet, Font);
            //wCharSet.ToList().ForEach(c => Console.WriteLine(c + " " + System.Windows.Forms.TextRenderer.MeasureText("" + c, Font)));
            //var wImage = new Bitmap(wSize.Width, wSize.Height);
            //using (var wGraphics = Graphics.FromImage(wImage))
            //{
            //    wGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //    wGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //    wGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            //    wGraphics.DrawString(wCharSet, Font, new SolidBrush(Color.FromArgb(0, 0, 0)), new PointF(0, 0));
            //    wGraphics.Flush();
            //}
            //wImage.Save("TEST.bmp");

            //int mFontTexture;
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
            //GL.GenTextures(1, out mFontTexture);
            //GL.BindTexture(TextureTarget.Texture2D, mFontTexture);
            //var wData = wImage.LockBits(new Rectangle(0, 0, wImage.Width, wImage.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, wImage.Width, wImage.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            //wImage.UnlockBits(wData);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
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

            mRoot.Render();


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

            if (mKeyboardState == ControlKeys.Exit)
            {
                Exit();
            }
            if (mKeyboardState == ControlKeys.FullScreen)
            {
                FullScreen = !FullScreen;
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
        }
        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            mMouseState.X = e.X;
            mMouseState.Y = e.Y;
            mMouseState.Button = e.Button;
            mMouseState.IsPressed = e.IsPressed;
            mMouseState.ButtonDown[e.Button] = false;

            OnMouse?.Invoke(this, mMouseState);
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
