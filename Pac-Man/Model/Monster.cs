using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Pac_Man.Model
{
    public class Monster
    {
        public Point position = new Point();
        public bool IsEdible = false;
        public CircleDirections direction = CircleDirections.Circle;
        public Point StartPosition = new Point();
        /// <summary>
        /// 移动当前monster
        /// </summary>
        public void Move()
        {

        }
        /// <summary>
        /// 新建monster实例
        /// </summary>
        /// <param name="index">当前monster索引</param>
        /// <param name="Height">当前GameArea的高度</param>
        /// <param name="Width">当前GameArea的宽度/param>
        public Monster(int index, int Height, int Width)
        {
           StartPosition= GetStartPosition(index, Height, Width);
            position = StartPosition;
        }
        /// <summary>
        /// 根据index计算开始位置
        /// </summary>
        /// <param name="index">当前monster索引</param>
        /// <param name="Height">当前GameArea的高度</param>
        /// <param name="Width">当前GameArea的宽度/param>
        /// <returns>开始位置</returns>
        public Point GetStartPosition(int index, int Height, int Width)
        {
            Point result = new Point(0, 0);
            switch(index)
            {
                case 1:
                    result = new Point(Width-1, 0);
                        break;
                case 2:
                    result = new Point(0, Height-1);
                    break;
                case 3:
                    result = new Point(Width-1, Height-1);
                    break;
                case 4:
                    result = new Point(Width / 2-1, 0);
                    break;
                case 5:
                    result = new Point(Width / 2-1, Height-1);
                    break;
                case 6:
                    result = new Point(0, Height / 2-1);
                    break;
                case 7: result = new Point(Width-1, Height / 2-1);
                    break;
            }
            return result;
        }
    }
}
