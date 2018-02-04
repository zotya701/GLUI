using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class ControlKeys
    {
        public static ControlKey Exit { get; set; } = Key.Escape;
        public static ControlKey FullScreen { get; set; } = Key.AltLeft + Key.Enter;

        public static void ListKeys()
        {
            var wProperties = typeof(ControlKeys).GetProperties();
            foreach(var wProperty in wProperties)
            {
                Console.WriteLine($"{wProperty.Name} -> {wProperty.GetValue(wProperty).ToString()}");
            }
        }
    }
}
