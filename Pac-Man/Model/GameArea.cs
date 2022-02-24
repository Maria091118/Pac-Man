using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Timers;
using System.Media;

namespace Pac_Man.Model
{
    public enum PositionState { Wall, SpecialBean, CommonBean, Space}
     
    class GameArea
    {
        public PositionState[,] Game_Area = new PositionState[40,20];
        public PacMan pacMan = new PacMan();
        public Timer timer = new Timer(10000);
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();
        public Action ScoreChanged { get; set; } = null;
        public delegate void OnCurrentSocreChangedEventHandler(object Sender, EventArgs e);
        public event OnCurrentSocreChangedEventHandler OnCurrentScoreChanged;
        public string MapName = "";
        SoundPlayer MonsterSoundPlayer = new SoundPlayer();
        SoundPlayer BeanSoundPlayer = new SoundPlayer();
        void CreatMonsters()
        {
            for(int i=0; i<4; i++)
            {
                Monsters.Add(i, new Monster(i, Game_Area.GetLength(1), Game_Area.GetLength(0)));
            }
        }
        public void SuperStateChange()
        {
             pacMan.IsSuper =! pacMan.IsSuper;
            foreach(Monster monster in Monsters.Values)
            {
                monster.IsEdible = pacMan.IsSuper;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();
            pacMan.IsSuper = false;
        }


        public void MonsterUpMove(Monster monster)
        {
            ScoreCountAndDeath(monster);
            if (monster.position.Y >0
                && Game_Area[monster.position.X , monster.position.Y-1] != PositionState.Wall&&!pacMan.IsDead)
            {
                monster.position = new Point(monster.position.X , monster.position.Y-1);
                monster.direction = CircleDirections.Up;
                ScoreCountAndDeath(monster);
            }
        }

        public void MonsterDownMove(Monster monster)
        {
            ScoreCountAndDeath(monster);
            if (monster.position.Y < Game_Area.GetLength(0) - 1
                && Game_Area[monster.position.X, monster.position.Y+1] != PositionState.Wall && !pacMan.IsDead)
            {
                monster.position = new Point(monster.position.X, monster.position.Y+1);
                monster.direction = CircleDirections.Down;
                ScoreCountAndDeath(monster);
            }
        }

        public void MonsterRightMove(Monster monster)
        {
            ScoreCountAndDeath(monster);
            if (monster.position.X < Game_Area.GetLength(0) - 1
                && Game_Area[monster.position.X + 1, monster.position.Y] != PositionState.Wall && !pacMan.IsDead)
            {
                monster.position = new Point(monster.position.X + 1, monster.position.Y);
                monster.direction = CircleDirections.Right;
                ScoreCountAndDeath(monster);
            }
        }

        public void MonsterLeftMove(Monster monster)
        {
            ScoreCountAndDeath(monster);
            if (monster.position.X >0
                && Game_Area[monster.position.X - 1, monster.position.Y] != PositionState.Wall && !pacMan.IsDead)
            {
                monster.position = new Point(monster.position.X - 1, monster.position.Y);
                monster.direction = CircleDirections.Left;
                ScoreCountAndDeath(monster);
            }
        }

        void ScoreCountAndDeath(Monster monster)
        {
            if (monster.position == pacMan.position && pacMan.IsSuper)
            {
                MonsterSoundPlayer.Play();
                monster.position = monster.StartPosition;
                Score_System.CurrentScore += Score_System.MonsterScore;
                OnCurrentScoreChanged?.Invoke(this, new EventArgs());
                ScoreChanged?.Invoke();
            }
            else if (monster.position == pacMan.position && !pacMan.IsSuper)
            {
                pacMan.IsDead = true;
            }
        }
       public CircleDirections GetMonsterFlaseDirection(Monster monster)
       {
            CircleDirections result = CircleDirections.Circle;
            switch (monster.direction)
            {
                case CircleDirections.Down:
                    if (monster.position.Y < Game_Area.GetLength(1) - 1
                            && Game_Area[monster.position.X, monster.position.Y + 1] != PositionState.Wall)
                    {
                        result = CircleDirections.Up;
                    }
                    break;
                case CircleDirections.Up:
                    if (monster.position.Y > 0
    && Game_Area[monster.position.X, monster.position.Y - 1] != PositionState.Wall)
                    {
                        result = CircleDirections.Down;
                    }
                    break;
                case CircleDirections.Left:
                    if (monster.position.X > 0
    && Game_Area[monster.position.X - 1, monster.position.Y] != PositionState.Wall)
                    {
                        result = CircleDirections.Right;
                    }
                    break;
                case CircleDirections.Right:
                    if (monster.position.X < Game_Area.GetLength(0) - 1
&& Game_Area[monster.position.X + 1, monster.position.Y] != PositionState.Wall)
                    {
                        result = CircleDirections.Left;
                    }
                    break;
            }
            return result;
        }
        public List<CircleDirections> MonsterMoveSides(Monster monster)
        {
            List<CircleDirections> result = new List<CircleDirections>();
            if (monster.position.X < Game_Area.GetLength(0) - 1
                 && Game_Area[monster.position.X + 1, monster.position.Y] != PositionState.Wall)
            {
                result.Add(CircleDirections.Right);
            }
            if (monster.position.X > 0
                && Game_Area[monster.position.X - 1, monster.position.Y] != PositionState.Wall)
            {
                result.Add(CircleDirections.Left);
            }
            if (monster.position.Y < Game_Area.GetLength(1) - 1
                && Game_Area[monster.position.X, monster.position.Y + 1] != PositionState.Wall)
            {
                result.Add(CircleDirections.Down);
            }
            if (monster.position.Y > 0
                && Game_Area[monster.position.X, monster.position.Y - 1] != PositionState.Wall)
            {
                result.Add(CircleDirections.Up);
            }
            return result;
        }
        
        void CountScore()
        {
            if (Game_Area[pacMan.position.X, pacMan.position.Y] == PositionState.CommonBean)
            {
                BeanSoundPlayer.Play();
                Score_System.CurrentScore += Score_System.CommonBeanScore;
                OnCurrentScoreChanged?.Invoke(this, new EventArgs());
                ScoreChanged?.Invoke();
            }
            else if (Game_Area[pacMan.position.X, pacMan.position.Y] == PositionState.SpecialBean)
            {
                BeanSoundPlayer.Play();
                pacMan.IsSuper = true;
                Score_System.CurrentScore += Score_System.CommonBeanScore;
                ScoreChanged?.Invoke();
            }
            IsWin();
        }
        public void PacManUpMove()
        {
            pacMan.circleDirection = CircleDirections.Up;
            if (pacMan.position.Y > 0
                &&Game_Area[pacMan.position.X, pacMan.position.Y-1]!=PositionState.Wall && !pacMan.IsDead)
            {

                pacMan.position = new Point(pacMan.position.X, pacMan.position.Y - 1);
                CountScore();
                Game_Area[pacMan.position.X, pacMan.position.Y] = PositionState.Space;
            }
        }
        public void PacManDownMove()
        {
            pacMan.circleDirection = CircleDirections.Down;

            if (pacMan.position.Y<Game_Area.GetLength(1)-1 
                && Game_Area[pacMan.position.X, pacMan.position.Y + 1] != PositionState.Wall && !pacMan.IsDead)
            {

                pacMan.position = new Point(pacMan.position.X, pacMan.position.Y + 1);
                CountScore();
                Game_Area[pacMan.position.X, pacMan.position.Y] = PositionState.Space;
            }
        }
        public void PacManRightMove()
        {
            pacMan.circleDirection = CircleDirections.Right;

            if (pacMan.position.X<Game_Area.GetLength(0)-1
                && Game_Area[pacMan.position.X+1, pacMan.position.Y] != PositionState.Wall && !pacMan.IsDead)
            {

                pacMan.position = new Point(pacMan.position.X + 1, pacMan.position.Y);

                CountScore();
                Game_Area[pacMan.position.X, pacMan.position.Y] = PositionState.Space;
            }
        }
        public void PacManLeftMove()
        {
            pacMan.circleDirection = CircleDirections.Left;

            if (pacMan.position.X > 0
                && Game_Area[pacMan.position.X-1, pacMan.position.Y] != PositionState.Wall && !pacMan.IsDead)
            {
                pacMan.position = new Point(pacMan.position.X - 1, pacMan.position.Y);

                CountScore();
                Game_Area[pacMan.position.X, pacMan.position.Y] = PositionState.Space;
            }
        }
        public void IsWin()
        {
            for(int x = Game_Area.GetLength(0)-1; x>=0; x--)
            {
                for(int y=Game_Area.GetLength(1)-1; y>=0; y--)
                {
                    if(Game_Area[x,y]==PositionState.CommonBean|| Game_Area[x, y] == PositionState.SpecialBean)
                    {
                        pacMan.IsWin = false;
                        return;
                    }
                }
            }
            pacMan.IsWin = true;
        }
        public void ChangeCircleDirection(CircleDirections GoalDirection)
        {
            pacMan.circleDirection = GoalDirection;
        }
         public GameArea()
         {
            MonsterSoundPlayer.SoundLocation = "Sounds\\MonsterSound.wav";
            MonsterSoundPlayer.Load();
            BeanSoundPlayer.SoundLocation = "Sounds\\BeanSound.wav";
            BeanSoundPlayer.Load();
            pacMan.position = new Point(Game_Area.GetLength(0) / 2, Game_Area.GetLength(1) / 2-1);
            timer.Elapsed += Timer_Elapsed;
            for (int x =0; x<Game_Area.GetLength(0); x++)
            {
                for(int y=0; y<Game_Area.GetLength(1); y++)
                {
                    Game_Area[x, y] = PositionState.Space;
                }
            }
            Game_Area[3, 1] = PositionState.SpecialBean;
            CreatMonsters();
         }
        
        public void Save()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "pam|*.pam"; 
            saveFileDialog.InitialDirectory = System.Environment.CurrentDirectory.ToString();
            saveFileDialog.ShowDialog();
            if(saveFileDialog.FileName!="")
            {
                byte[] data = new byte[Game_Area.GetLength(0)*Game_Area.GetLength(1)];
                for(int x=0; x<Game_Area.GetLength(0); x++)
                {
                    for(int y=0; y<Game_Area.GetLength(1); y++)
                    {
                        data[x*Game_Area.GetLength(1)+ y] =(byte) Game_Area[x,y];
                    }
                }
                File.WriteAllBytes(saveFileDialog.FileName, data);
                MapName = saveFileDialog.FileName;
            }
        }
        public void Load()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "pam|*.pam";
            openFileDialog.InitialDirectory = System.Environment.CurrentDirectory.ToString();
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                MapName = openFileDialog.FileName;
                Load(openFileDialog.FileName);
            }
        }
        public void Load(string FileName)
        {
            if (FileName == "")
            {
                return;
            }
            byte[] data = File.ReadAllBytes(FileName);
            for (int x = 0; x < Game_Area.GetLength(0); x++)
            {
                for (int y = 0; y < Game_Area.GetLength(1); y++)
                {
                    Game_Area[x, y] = (PositionState)data[x * Game_Area.GetLength(1) + y];
                }
            }
        }
        public void Reset()
        {
            int i = 0;
            foreach (Monster monster in Monsters.Values)
            {
                monster.position = monster.GetStartPosition(i, Game_Area.GetLength(1), Game_Area.GetLength(0));
                i++;
            }
            pacMan.position = new Point(Game_Area.GetLength(0) / 2, Game_Area.GetLength(1) / 2 - 1);
            pacMan.IsDead = false;
        }
    }
}
