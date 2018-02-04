﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using System.Threading;
using GLUI;
using System.Drawing;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Enum.GetNames(typeof(Key)).ToList().ForEach(key => Console.WriteLine($"public static Key {key.ToString()} {{ get {{ return new Key(Key.{key.ToString()}); }} }}"));

            var wApp = new App()
            {
                Title = "Test program",
                X = 100,
                Y = 50,
                Width = 210 * 5,
                Height = 90 * 5,
                VSync = false,
                Commands = new Dictionary<string, Command>
                {
                    {"Exit", Key.Escape },
                    {"Fullscreen", Key.AltLeft + Key.Enter },
                }
            };

            wApp.Commands["Exit"].Activated += (s, e) =>
            {
                wApp.Exit();
            };
            wApp.Commands["Fullscreen"].Activated += (s, e) =>
            {
                wApp.FullScreen = !wApp.FullScreen;
            };

            var wWindow = new Window
            {
                BackgroundColor = Color.FromArgb(255, 0, 0),
                Location = new Point(300, 100),
                Size = new Size(600, 300),
                Children =
                {
                    new Window
                    {
                        BackgroundColor = Color.FromArgb(0, 255, 0),
                        Location = new Point(50, 50),
                        Size = new Size(100, 100)
                    },
                    new Window
                    {
                        BackgroundColor = Color.FromArgb(0, 0, 255),
                        Location = new Point(90, 50),
                        Size = new Size(100, 100)
                    }
                    //new Label
                    //{
                    //    Text = "TEST",
                    //    Font = new Font("Arial", 128, GraphicsUnit.Pixel)
                    //}
                }
            };
            //for (int i = 0; i < 1000; ++i)
            //{
            //    wWindow.Children.Add(new Window
            //    {
            //        BackgroundColor = Color.FromArgb(0, 255, 0),
            //        Location = new Point(10, 10),
            //        Size = new Size(100, 100)
            //    });
            //}

            var asd = new GLUI.Font("Arial", 128, Color.Black);
            asd.SaveCharacterSet();

            wApp.AddComponent(wWindow);
            wApp.Run();
        }
    }
}
