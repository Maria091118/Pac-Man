using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Pac_Man.Model
{
    public class PacMan
    {
        public bool IsSuper { get; set; }
        public Point position { get; set; }
         public CircleDirections circleDirection { get; set; }
        public bool IsDead { get; set; } = false;
        public bool IsWin { get; set; } = false;
    }
}
