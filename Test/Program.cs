﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using System.Threading;
using GLUI;
using System.Drawing;
using OpenTK;

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
                Width = 210 * 8,
                Height = 90 * 8,
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
                //BackgroundColor = Color.FromArgb(100, 255, 0, 0),
                Location = new Vector2(300, 100),
                Size = new Vector2(1300, 500),
                Children =
                {
                    new Window
                    {
                        //BackgroundColor = Color.FromArgb(100, 0, 255, 0),
                        Location = new Vector2(50, 50),
                        Size = new Vector2(100, 100)
                    },
                    new Window
                    {
                        //BackgroundColor = Color.FromArgb(100, 0, 0, 255),
                        Location = new Vector2(90, 50),
                        Size = new Vector2(100, 100)
                    },
                    new Label
                    {
                        Text = "Bared on your tomb\r\nI'm a prayer for your loneliness\r\nAnd would you ever soon\r\nCome above unto me?",
                        FontFamily = "Arial",
                        FontSize = 14,
                        FontColor = Color.Black,
                        BorderWidth = 1.5f,
                        Location = new Vector2(200, 60),
                        Size = new Vector2(250, 100)
                    },
                    new Button("ASD")
                    {
                        Location = new Vector2(200, 200),
                        Size = new Vector2(70, 30)
                    }
                }
            };

            //Task.Run(() =>
            //{
            //    var wLabel = wWindow.Children.First(wChild => wChild is Label) as Label;
            //    for (int i = 0; i < 99999; ++i)
            //    {
            //        wLabel.Text = $"{i}";
            //        wLabel.FontColor = Color.FromArgb(255, 255, (int)((Math.Sin(i / Math.PI / 5.0) + 1.0) / 2.0 * 255.0), 0);
            //        //wLabel.BackgroundColor = Color.FromArgb(255, 255, (int)((Math.Sin(i / Math.PI / 10.0) + 1.0) / 2.0 * 255.0), 0);
            //        System.Threading.Thread.Sleep(10);
            //    }
            //});

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    wWindow.Children.Add(new Button("ASDASDASD")
                    {
                        Location = new Vector2(500 + i * 75, 100 + j * 35),
                        Size = new Vector2(70, 30)
                    });
                }
            }

            var asd = new GLUI.Font("Arial", 256, Color.Black);
            asd.SaveCharacterSet();

            wApp.AddComponent(wWindow);
            wApp.Run();
        }
    }
}
