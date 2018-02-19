using System;
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
                    },
                    new Label
                    {
                        Text = "Bared on your tomb\r\nI'm a prayer for your loneliness\r\nAnd would you ever soon\r\nCome above unto me?",
                        FontFamily = "Arial",
                        FontSize = 14,
                        FontColor = Color.Black,
                        BorderWidth = 2,
                        Location = new Point(200, 60),
                        Size = new Size(350, 100)
                    },
                    new Label
                    {
                        Text = "For once upon a time\r\nFrom the binds of your lowliness\r\nI could always find the right slot for your sacred key",
                        FontFamily = "Arial",
                        FontSize = 14,
                        FontColor = Color.Black,
                        BorderWidth = 2,
                        Location = new Point(200, 200)
                        //Size = new Size(300, 50)
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
            //        System.Threading.Thread.Sleep(100);
            //    }
            //});

            var asd = new GLUI.Font("Arial", 128, Color.Black);
            //asd.SaveCharacterSet();

            wApp.AddComponent(wWindow);
            wApp.Run();
        }
    }
}
