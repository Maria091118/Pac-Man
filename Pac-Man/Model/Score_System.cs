using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pac_Man.Model
{
    public static class Score_System
    {
        public const int CommonBeanScore = 10;
        public const int SpecialBeanScore= 50;
        public const int MonsterScore = 100;
        static public int HighiestScore { get; set; } = 0;
        static public int CurrentScore { get; set; } = 0;

        static public void Reset()
        {
            if(CurrentScore>HighiestScore)
            {
                HighiestScore = CurrentScore;
            }
            CurrentScore = 0;
        }
    }
}
