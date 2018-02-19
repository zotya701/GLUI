using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLUI
{
    public enum Horizontal
    {
        Left,
        Center,
        Right
    }

    public enum Vertical
    {
        Top,
        Center,
        Bottom
    }

    public class Alignment
    {
        public Vertical Vertical { get; set; }
        public Horizontal Horizontal { get; set; }
    }
}
