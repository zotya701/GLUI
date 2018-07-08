using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI.GLUI
{
    public static class Raster
    {
        public static Vector2 Location { get; set; } = new Vector2(0, 0);

        public static void Move(float right, float down)
        {
            Location = Location + new Vector2(right, down);
        }

        public static void Move(Vector2 vector)
        {
            Location = Location + vector;
        }
    }
}
