using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI
{
    public static class Raster
    {
        public static Point Location { get; set; } = new Point(0, 0);
        public static void Move(int right, int down)
        {
            Location = Location + new Size(right, down);
        }
    }
}
