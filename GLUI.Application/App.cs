﻿#region dependencies
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK.Input;
using GLUI.GLUI;
using System.Drawing;
using GLUI.Foundation;
using System.Diagnostics;
using System.Threading;
#endregion

namespace GLUI.Application
{
    public class App
    {
        private GameWindow mWindow;
        private RootComponent mRoot;
        //private Label mPerformanceLabel;
        //private Stopwatch mPerformanceLabelTimer = Stopwatch.StartNew();
        private Stopwatch mTimer = Stopwatch.StartNew();
        private LinkedList<DateTime> mFrameDates = new LinkedList<DateTime>();
        private double mElapsedTime;
        private double mWaitTime;
        private ulong mFrameCounter = 0;
        private Foundation.KeyboardState mKeyboardState;
        private Foundation.MouseState mMouseState;

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
        public double FPS { get; set; } = 100;
        public bool ShowPerformanceInfo { get; set; }
        public string Title { get { return mWindow.Title; } set { mWindow.Title = value; } }
        public System.Drawing.Icon Icon { get { return mWindow.Icon; } set { mWindow.Icon = value; } }
        public bool CursorVisible { get { return mWindow.CursorVisible; } set { mWindow.CursorVisible = value; } }
        public List<Command> Commands { get; } = new List<Command>();
        #endregion

        public App()
        {
            mKeyboardState = new Foundation.KeyboardState();
            mMouseState = new Foundation.MouseState();
            // Enable antialiasing
            mWindow = new GameWindow(1, 1, new OpenTK.Graphics.GraphicsMode(32, 0, 8, 8));
            mWindow.VSync = VSyncMode.Off;

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
                BorderColor = Color.FromArgb(0, 0, 0, 0),
                BorderWidth = 0,
            };
            OnKeyboard += mRoot.KeyboardHandler;
            OnMouse += mRoot.MouseHandler;

            //mPerformanceLabel = new Label()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Right,
            //    VerticalAlignment = VerticalAlignment.Bottom,
            //    ClickThrough = true,
            //    //FontFamily = "Arial",
            //    //FontSize = 12,
            //    //FontColor = Color.Orange
            //};
            //AddComponent(mPerformanceLabel);
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
            foreach(var wCommand in Commands)
            {
                Console.WriteLine($"  {wCommand}");
            }
            
            mRoot.Size = new Vector2(Width, Height);
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
        private Texture mWallpaperTexture;
        private void OnLoad(object sender, EventArgs e)
        {
            var wWallpaper = new Bitmap(@"XP.bmp");
            mWallpaperTexture = new Texture
            {
                Width = wWallpaper.Width,
                Height = wWallpaper.Height
            };
            GL.BindTexture(TextureTarget.Texture2D, mWallpaperTexture.Id);
            var wData = wWallpaper.LockBits(new Rectangle(0, 0, mWallpaperTexture.Width, mWallpaperTexture.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, mWallpaperTexture.Width, mWallpaperTexture.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, wData.Scan0);
            wWallpaper.UnlockBits(wData);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            mRoot.Update();
        }

        private void OnResize(object sender, EventArgs e)
        {
            mRoot.Size = new Vector2(Width, Height);
            //mPerformanceLabel.Size = mRoot.Size;
            GL.Viewport(mWindow.ClientRectangle);
            OnUpdateFrame(null, null);
            OnRenderFrame(null, null);
        }

        private void OnUpdateFrame(object sender, FrameEventArgs e)
        {
            Dispatcher.ExecuteNextAction();

            mElapsedTime = mTimer.Elapsed.TotalSeconds;
            if (mElapsedTime < mWaitTime) return;

            OnMouse?.Invoke(this, mMouseState);
            OnKeyboard?.Invoke(this, mKeyboardState);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.ScissorTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            mRoot.Update();
        }

        private void OnRenderFrame(object sender, FrameEventArgs e)
        {
            if (FPS >= 30 && mElapsedTime < mWaitTime) return;

            mWaitTime = Math.Max((1.0f - (mTimer.Elapsed.TotalSeconds * FPS / 1 - FPS * mWaitTime)) / FPS, 0.0f);  // Calculate the dt between frames
            mTimer.Restart();

            //mPerformanceLabel.Visible = ShowPerformanceInfo;
            //if (mPerformanceLabel.Visible)
            //{
            //    mPerformanceLabel.BringFront();
            //    mFrameCounter++;
            //    if (mPerformanceLabelTimer.Elapsed.TotalSeconds >= 0.5f)
            //    {
            //        mFrameDates.AddLast(DateTime.Now);
            //        var wFrameCounter = mFrameCounter;
            //        if (mFrameDates.Count >= 3)
            //        {
            //            Dispatcher.BeginInvoke(() =>
            //            {
            //                var wPerformanceInfos = new List<string>
            //                {
            //                    $"FPS: {Math.Round(wFrameCounter / (mFrameDates.Last.Value - mFrameDates.First.Value).TotalSeconds)}",
            //                    $"CPU utilization: {PerformanceInfo.CPUUtilization}%",
            //                    $"Used RAM: {PerformanceInfo.UsedRam}MB",
            //                    $"Available RAM: {PerformanceInfo.AvailableRAM}MB",
            //                    $"Total used RAM: {PerformanceInfo.TotalUsedRAM}MB",
            //                    $"Total RAM: {PerformanceInfo.TotalRAM}MB"
            //                };
            //                mPerformanceLabel.Text = string.Join("\r\n", wPerformanceInfos);
            //            });
            //            mFrameDates.RemoveFirst();
            //            mFrameCounter = 0;
            //        }
            //        mPerformanceLabelTimer.Restart();
            //    }
            //}

            GL.ClearColor(0.1f, 0.2f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindTexture(TextureTarget.Texture2D, mWallpaperTexture.Id);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(0, 0);
            GL.TexCoord2(1, 0); GL.Vertex2(Width, 0);
            GL.TexCoord2(1, 1); GL.Vertex2(Width, Height);
            GL.TexCoord2(0, 1); GL.Vertex2(0, Height);
            GL.End();
            GL.BindTexture(TextureTarget.Texture2D, 0);

            mRoot.Render();

            mWindow.SwapBuffers();
        }

        private void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            mKeyboardState.Alt = e.Alt;
            mKeyboardState.Control = e.Control;
            mKeyboardState.Shift = e.Shift;
            mKeyboardState.KeyDown[e.Key] = true;

            Commands.ForEach(wCommand => wCommand.Check(mKeyboardState));

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
