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
                Width = 210 * 5,
                Height = 90 * 5,
            };

            var wWindow = new Window
            {
                BackgroundColor = Color.FromArgb(255, 0, 0),
                Location = new Point(300, 100),
                Size = new Size(400, 200),
                Children =
                {
                    new Window
                    {
                        BackgroundColor = Color.FromArgb(0, 255, 0),
                        Location = new Point(10, 10),
                        Size = new Size(100, 100)
                    },
                    new Window
                    {
                        BackgroundColor = Color.FromArgb(0, 0, 255),
                        Location = new Point(50, 10),
                        Size = new Size(100, 100)
                    },
                    new Label
                    {
                        Text = "TEST",
                        Font = new Font("Arial", 128, GraphicsUnit.Pixel)
                    }
                }
            };
            //for (int i = 0; i < 10000; ++i)
            //{
            //    wWindow.Children.Add(new Window
            //    {
            //        BackgroundColor = Color.FromArgb(0, 255, 0),
            //        Location = new Point(10, 10),
            //        Size = new Size(100, 100)
            //    });
            //}

            ControlKeys.ListKeys();

            wApp.AddComponent(wWindow);
            wApp.Run();
        }
    }
}
