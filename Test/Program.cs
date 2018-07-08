using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using System.Threading;
using GLUI;
using System.Drawing;
using OpenTK;
using GLUI.Foundation;

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
                ShowPerformanceInfo = true,
            };
            wApp.Commands.Add(new Command("Exit", Key.Escape).Executes(() =>
            {
                wApp.Exit();
            }));
            wApp.Commands.Add(new Command("FullScreen", Key.AltLeft, Key.Enter).Executes(() =>
            {
                wApp.FullScreen = !wApp.FullScreen;
            }));


            var wWindow = new Window("Test window title")
            {
                //BackgroundColor = Color.FromArgb(100, 255, 0, 0),
                Location = new Vector2(300.0f, 100.0f),
                Size = new Vector2(1300, 500),
                Children =
                {
                    new Window
                    {
                        //BackgroundColor = Color.FromArgb(100, 0, 255, 0),
                        Location = new Vector2(50, 50),
                        Size = new Vector2(200, 100)
                    },
                    new Window
                    {
                        //BackgroundColor = Color.FromArgb(100, 0, 0, 255),
                        Location = new Vector2(90, 50),
                        Size = new Vector2(200, 100)
                    },
                    new Label
                    {
                        Text = "Bared on your tomb\r\nI'm a prayer for your loneliness\r\nAnd would you ever soon\r\nCome above unto me?",
                        FontFamily = "Arial",
                        FontSize = 14,
                        FontColor = Color.LightGray,
                        Location = new Vector2(200, 60),
                        //Size = new Vector2(250, 100),
                        BorderWidth = 1.0f
                    },
                    new RepeatButton("RepeatButton - 1")
                    {
                        Location = new Vector2(200, 200),
                        Size = new Vector2(200, 30)
                    },
                    new CheckBox("Test check box")
                    {
                        Location = new Vector2(200, 250),
                        //Size = new Vector2(100, 100)
                    },
                    new ToggleButton("Test toggle button")
                    {
                        Location = new Vector2(200, 300),
                        Size = new Vector2(200, 30)
                    }
                }
            };

            var wCheckBox = wWindow.Children.First(wChild => wChild is CheckBox) as CheckBox;
            wCheckBox.CheckBoxChanged += (o, e) => { Console.WriteLine("CheckBox changed"); };
            wCheckBox.CheckBoxChecked += (o, e) => { Console.WriteLine("CheckBox checked"); };
            wCheckBox.CheckBoxUnchecked += (o, e) => { Console.WriteLine("CheckBox unchecked"); };

            var wRepeatButton = wWindow.Children.First(wChild => wChild is RepeatButton) as RepeatButton;
            wRepeatButton.ButtonClicked += (o, e) => { wCheckBox.Enabled = !wCheckBox.Enabled; wRepeatButton.Text = wRepeatButton.Text.Substring(0, 15) + $"{Convert.ToInt32(wRepeatButton.Text.Substring(15)) + 1}"; Console.WriteLine(wRepeatButton.Text); };

            var wLabel = wWindow.Children.First(wChild => wChild is Label) as Label;
            wLabel.LabelClicked += (o, e) => { Console.WriteLine(wLabel.Text); };

            wWindow.MouseEntered += (o, e) => { Console.WriteLine("Mouse entered"); };
            wWindow.MouseLeaved += (o, e) => { Console.WriteLine("Mouse leaved"); };

            var wToggleButton = wWindow.Children.First(wChild => wChild is ToggleButton) as ToggleButton;
            wToggleButton.ToggleButtonOn += (o, e) => { wToggleButton.Text = "ON"; };
            wToggleButton.ToggleButtonOff += (o, e) => { wToggleButton.Text = "OFF"; };

            //Task.Run(() =>
            //{
            //    var wLabel = wWindow.Children.First(wChild => wChild is Label) as Label;
            //    for (int i = 0; i < 99999; ++i)
            //    {
            //        Dispatcher.Invoke(() =>
            //        {
            //            wLabel.Text = $"{i}";
            //            wLabel.FontSize = 30 + (float)((Math.Sin(i / Math.PI / 1.0) + 1.0) / 2.0 * 20.0f);
            //            wLabel.FontColor = Color.FromArgb(255, 255, (int)((Math.Sin(i / Math.PI / 5.0) + 1.0) / 2.0 * 255.0), 0);
            //            //wLabel.BackgroundColor = Color.FromArgb(255, 255, (int)((Math.Sin(i / Math.PI / 10.0) + 1.0) / 2.0 * 255.0), 0);
            //        });
            //        System.Threading.Thread.Sleep(50);
            //    }
            //});

            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    wWindow.Children.Add(new Button("ASDASD")
                    {
                        Location = new Vector2(500 + i * 80, 100 + j * 35),
                        Size = new Vector2(70, 30)
                    });
                }
            }

            //var asd = new GLUI.Font("Arial", 256, Color.Black);
            //asd.SaveCharacterSet();

            wApp.AddComponent(wWindow);
            wApp.Run();
        }
    }
}
