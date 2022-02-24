using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Media;

namespace Pac_Man
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
     
     public enum CircleDirections { Circle, Down, Left, Right, Up};
    public partial class MainWindow : Window
    {
        public WriteableBitmap paper;
        Thread MonsterPlayThread = null;
        Thread PacManPlayThread = null;
        Dictionary<CircleDirections, System.Drawing.Image> CommonPacManImages = new Dictionary<CircleDirections, System.Drawing.Image>();
        Dictionary<CircleDirections, System.Drawing.Image> SuperPacManImages = new Dictionary<CircleDirections, System.Drawing.Image>();
        Dictionary<int, System.Drawing.Image> MonstersImages = new Dictionary<int, System.Drawing.Image>();
        const int StepLength = 20;
        const int CommonBeanLength = 5;
        View_Model.ViewModel VM = new View_Model.ViewModel();
        System.Drawing.Image GameOverImage;
        System.Drawing.Image YouWinImage;
        bool IsDead = false;
        SoundPlayer WinSoundPlayer = new SoundPlayer();
        SoundPlayer DeadSoundPlayer = new SoundPlayer();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = VM;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            paper = new WriteableBitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), 96, 96, PixelFormats.Bgra32, null);
            DisplayImage.Source = paper;

            CommonPacManImages.Add(CircleDirections.Circle, System.Drawing.Image.FromFile("Pictures\\Small Circle.bmp"));
            CommonPacManImages.Add(CircleDirections.Left, System.Drawing.Image.FromFile("Pictures\\Small Left.bmp"));
            CommonPacManImages.Add(CircleDirections.Right, System.Drawing.Image.FromFile("Pictures\\Small Right.bmp"));
            CommonPacManImages.Add(CircleDirections.Down, System.Drawing.Image.FromFile("Pictures\\Small Down.bmp"));
            CommonPacManImages.Add(CircleDirections.Up, System.Drawing.Image.FromFile("Pictures\\Small Up.bmp"));
            SuperPacManImages.Add(CircleDirections.Up, System.Drawing.Image.FromFile("Pictures\\S_Small_Up.bmp"));
            SuperPacManImages.Add(CircleDirections.Left, System.Drawing.Image.FromFile("Pictures\\S_Small_Left.bmp"));
            SuperPacManImages.Add(CircleDirections.Down, System.Drawing.Image.FromFile("Pictures\\S_Small_Down.bmp"));
            SuperPacManImages.Add(CircleDirections.Right, System.Drawing.Image.FromFile("Pictures\\S_Small_Right.bmp"));
            SuperPacManImages.Add(CircleDirections.Circle, System.Drawing.Image.FromFile("Pictures\\S_Small_Circle.bmp"));
            MonstersImages.Add(0, System.Drawing.Image.FromFile("Pictures\\Small_Monster1.bmp"));
            MonstersImages.Add(1, System.Drawing.Image.FromFile("Pictures\\Small_Monster2.bmp"));
            MonstersImages.Add(2, System.Drawing.Image.FromFile("Pictures\\Small_Monster3.bmp"));
            MonstersImages.Add(3, System.Drawing.Image.FromFile("Pictures\\Small_Monster4.bmp"));
            MonstersImages.Add(4, System.Drawing.Image.FromFile("Pictures\\Small_Monster5.bmp"));
            GameOverImage = System.Drawing.Image.FromFile("Pictures\\GAMEOVER.bmp");
            YouWinImage = System.Drawing.Image.FromFile("Pictures\\YouWinImage.bmp");
            WinSoundPlayer.SoundLocation = "Sounds\\WinSound.wav";
            WinSoundPlayer.Load();
            DeadSoundPlayer.SoundLocation = "Sounds\\DeadSound.wav";
            DeadSoundPlayer.Load();

            //Bitmap BackPaper = new Bitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), paper.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, paper.BackBuffer);
            //Graphics graphic = Graphics.FromImage(BackPaper);
            if (!File.Exists("HighestScore"))
            {
                File.WriteAllText("HighestScore", "0");
            }
            VM.HighiestScore = Convert.ToInt32(File.ReadAllText("HighestScore"));
            DrawGame_Area();
            DrawPacManImage();
            DrawMonstersImages();
            //Erase(new System.Drawing.Point(0, 0));
            //DrawOneRectangle(new System.Drawing.Point(1000,1000), graphic);
            //VM.gameArea.pacMan.IsSuper = true;
        }

        void DrawPacManImage()
        {
            paper.Lock();
            System.Drawing.Image image = CommonPacManImages[VM.gameArea.pacMan.circleDirection];
            if (VM.gameArea.pacMan.IsSuper)
            {
                image = SuperPacManImages[VM.gameArea.pacMan.circleDirection];
            }
            float x = VM.gameArea.pacMan.position.X * StepLength;
            float y = VM.gameArea.pacMan.position.Y * StepLength;
            System.Drawing.RectangleF rectangle = new System.Drawing.RectangleF(x, y, StepLength, StepLength);
            Bitmap BackPaper = new Bitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), paper.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, paper.BackBuffer);
            Graphics graphics = Graphics.FromImage(BackPaper);
            graphics.DrawImage(image, x, y, rectangle.Width, rectangle.Height);
            //paper.AddDirtyRect(new Int32Rect(0, 150, 300, 169));
            paper.AddDirtyRect(new Int32Rect(0, 0, Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight)));
            paper.Unlock();
        }
        void DrawGameImage(System.Drawing.Image Image)
        {
            paper.Lock();
            float x = (VM.gameArea.Game_Area.GetLength(0)* StepLength - Image.Width)/2;
            float y = (VM.gameArea.Game_Area.GetLength(1) * StepLength - Image.Height) / 2;
            System.Drawing.RectangleF rectangle = new System.Drawing.RectangleF(x, y, StepLength, StepLength);
            Bitmap BackPaper = new Bitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), paper.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, paper.BackBuffer);
            Graphics graphics = Graphics.FromImage(BackPaper);
            graphics.DrawImage(Image, x, y, Image.Width, Image.Height);
            //paper.AddDirtyRect(new Int32Rect(0, 150, 300, 169));
            paper.AddDirtyRect(new Int32Rect(0, 0, Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight)));
            paper.Unlock();
        }
        void DrawMonstersImages()
        {
            paper.Lock();
            for (int i = 0; i < VM.gameArea.Monsters.Count; i++)
            {
                int index = i % 4;
                System.Drawing.Image image = MonstersImages[index];
                if (VM.gameArea.Monsters[index].IsEdible)
                {
                    image = MonstersImages[4];
                }
                float x = VM.gameArea.Monsters[index].position.X * StepLength;
                float y = VM.gameArea.Monsters[index].position.Y * StepLength;
                System.Drawing.RectangleF rectangle = new System.Drawing.RectangleF(x, y, StepLength, StepLength);
                Bitmap BackPaper = new Bitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), paper.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, paper.BackBuffer);
                Graphics graphics = Graphics.FromImage(BackPaper);
                graphics.DrawImage(image, x, y, rectangle.Width, rectangle.Height);
                //paper.AddDirtyRect(new Int32Rect(0, 150, 300, 169));
                paper.AddDirtyRect(new Int32Rect(0, 0, Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight)));

            }
            paper.Unlock();
        }
        void PacManMovePart2(bool IsCounting, int count, CircleDirections direction)
        {
            if (IsCounting) count = count + 70;
            App.Current.Dispatcher.Invoke(() => Erase(VM.gameArea.pacMan.position));
            switch (direction)
            {
                case CircleDirections.Up:
                    VM.gameArea.PacManUpMove();
                    break;
                case CircleDirections.Down:
                    VM.gameArea.PacManDownMove();
                    break;
                case CircleDirections.Left:
                    VM.gameArea.PacManLeftMove();
                    break;
                case CircleDirections.Right:
                    VM.gameArea.PacManRightMove();
                    break;
            }
            App.Current.Dispatcher.Invoke(() => DrawPacManImage());
        }


        CircleDirections PacManMovePart1()
        {
            CircleDirections direction = CircleDirections.Circle;
            while (direction == CircleDirections.Circle && !IsDead)
            {
                Thread.Sleep(5);
                if (App.Current.Dispatcher.Invoke(() => Keyboard.IsKeyDown(Key.Up)))
                {
                    direction = CircleDirections.Up;
                }
                else if (App.Current.Dispatcher.Invoke(() => Keyboard.IsKeyDown(Key.Down)))
                {
                    direction = CircleDirections.Down;
                }
                else if (App.Current.Dispatcher.Invoke(() => Keyboard.IsKeyDown(Key.Left)))
                {
                    direction = CircleDirections.Left;
                }
                else if (App.Current.Dispatcher.Invoke(() => Keyboard.IsKeyDown(Key.Right)))
                {
                    direction = CircleDirections.Right;
                }
            }

            VM.gameArea.pacMan.circleDirection = CircleDirections.Circle;
            App.Current.Dispatcher.Invoke(() => DrawPacManImage());
            return direction;
        }

        Random random = new Random();
        void MonstersMovePart2()
            {
            foreach (Model.Monster monster in VM.gameArea.Monsters.Values)
            {
                List<CircleDirections> directions = VM.gameArea.MonsterMoveSides(monster);

                CircleDirections FalseDirection = VM.gameArea.GetMonsterFlaseDirection(monster);
                int index = random.Next(directions.Count);
                while (directions[index] == FalseDirection)
                {
                    index = random.Next(directions.Count);
                }
                if (directions[index] == CircleDirections.Down)
                {
                    VM.gameArea.MonsterDownMove(monster);
                }
                else if (directions[index] == CircleDirections.Up)
                {
                    VM.gameArea.MonsterUpMove(monster);
                }
                else if (directions[index] == CircleDirections.Right)
                {
                    VM.gameArea.MonsterRightMove(monster);
                }
                else if (directions[index] == CircleDirections.Left)
                {
                    VM.gameArea.MonsterLeftMove(monster);
                }
            }
            App.Current.Dispatcher.Invoke(() => DrawMonstersImages());
        }
        
            void MonstersMovePart1()
            {
                foreach (Model.Monster monster in VM.gameArea.Monsters.Values)
                {
                    App.Current.Dispatcher.Invoke(() => Erase(monster.position));
                }
                App.Current.Dispatcher.Invoke(() => DrawGame_Area());
            }

        void MonsterPlay()
        {
            bool IsSuper = VM.gameArea.pacMan.IsSuper;
            bool IsCounting = false;
            int count = 0;
            while (!IsDead)
            {
                if (IsSuper != VM.gameArea.pacMan.IsSuper)
                {
                    App.Current.Dispatcher.Invoke(() => DrawPacManImage());
                    IsCounting = true;
                    IsSuper = VM.gameArea.pacMan.IsSuper;
                    count = 0;
                    for (int i = 0; i < VM.gameArea.Monsters.Count; i++)
                    {
                        int index = i % 4;
                        VM.gameArea.Monsters[index].IsEdible = true;
                    }
                    if (VM.gameArea.pacMan.IsWin)
                    {
                        App.Current.Dispatcher.Invoke(() => DrawGameImage(YouWinImage));
                        IsDead = true;
                    }
                }
                if(!VM.gameArea.pacMan.IsDead)
                {
                    MonstersMovePart1();
                    MonstersMovePart2();
                    Thread.Sleep(140);
                }


                if (IsCounting)
                {
                    count = count + 140;
                    if (count >= 20000)
                    {
                        VM.gameArea.pacMan.IsSuper = false;
                        App.Current.Dispatcher.Invoke(() => DrawPacManImage());
                        IsSuper = false;
                        IsCounting = false;
                        for (int i = 0; i < VM.gameArea.Monsters.Count; i++)
                        {
                            int index = i % 4;
                            VM.gameArea.Monsters[index].IsEdible = false;
                        }
                    }
                }
                if (VM.gameArea.pacMan.IsDead)
                {
                    IsDead = true;
                }
             }
        }

        void PacManPlay()
        {
            bool IsSuper = VM.gameArea.pacMan.IsSuper;
            bool IsCounting = false;
            int count = 0;
            while (!IsDead)
            {
                if (IsSuper != VM.gameArea.pacMan.IsSuper)
                {
                    App.Current.Dispatcher.Invoke(() => DrawPacManImage());
                    IsCounting = true;
                    IsSuper = VM.gameArea.pacMan.IsSuper;
                    count = 0;
                }

                if (!VM.gameArea.pacMan.IsDead)
                {
                    CircleDirections direction = PacManMovePart1();
                    Thread.Sleep(70);
                    PacManMovePart2(IsCounting, count, direction);
                    Thread.Sleep(70);
                }

                if (IsCounting)
                {
                    count = count + 140;
                    if (count >= 20000)
                    {
                        VM.gameArea.pacMan.IsSuper = false;
                        App.Current.Dispatcher.Invoke(() => DrawPacManImage());
                        IsSuper = false;
                        IsCounting = false;
                    }
                }
                if (VM.gameArea.pacMan.IsDead)
                {
                    IsDead = true;
                    App.Current.Dispatcher.Invoke(() => DrawGameImage(GameOverImage));
                    DeadSoundPlayer.Play();
                    Thread.Sleep(1000);
                }
                if (VM.gameArea.pacMan.IsWin)
                {
                    Win();
                }
            }
        }
        void Win()
        {
            IsDead = true;
            App.Current.Dispatcher.Invoke(() => DrawGameImage(YouWinImage));
            WinSoundPlayer.Play();
            Thread.Sleep(1000);
            MessageBoxResult ClickResult = MessageBox.Show("Go to next map?", "Finished!", MessageBoxButton.YesNo);
            if(ClickResult==MessageBoxResult.No)
            {
                App.Current.Dispatcher.Invoke(() => BT_Play_Click(this, null));
            }
            if(ClickResult==MessageBoxResult.Yes)
            {
                VM.gameArea.Load();
                App.Current.Dispatcher.Invoke(() => BT_Play_Click(this, null));
            }
        }
            void Erase(System.Drawing.Point position)
            {
                paper.Lock();

                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(position.X * StepLength, position.Y * StepLength, StepLength, StepLength);
            if (position.X == -1)
            {
                rectangle = new System.Drawing.Rectangle(0, 0, Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight));
            }
            Bitmap BackPaper = new Bitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), paper.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, paper.BackBuffer);
                Graphics graphics = Graphics.FromImage(BackPaper);
                SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.Black);
                graphics.FillRectangle(solidBrush, rectangle);
                graphics.Flush();
                graphics.Dispose();
                graphics = null;
                BackPaper.Dispose();
                BackPaper = null;
                paper.AddDirtyRect(new Int32Rect(0, 0, Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight)));
                paper.Unlock();
            }
            void DrawOneRectangle(System.Drawing.Point location, Graphics graphics, System.Drawing.Color color)
            {
                SolidBrush solidBrush = new SolidBrush(color);
                System.Drawing.Rectangle rectangle = System.Drawing.Rectangle.FromLTRB(location.X, location.Y, location.X + StepLength, location.Y + StepLength);
                graphics.FillRectangle(solidBrush, rectangle);
            }


            System.Drawing.Point OldPosition = new System.Drawing.Point();
            bool IsMove = true;

            void Move(CircleDirections direction)
            {
                if (IsMove)
                {
                    VM.gameArea.pacMan.circleDirection = CircleDirections.Circle;
                    DrawPacManImage();
                    OldPosition = VM.gameArea.pacMan.position;
                    switch (direction)
                    {
                        case CircleDirections.Up:
                            VM.gameArea.PacManUpMove();
                            break;
                        case CircleDirections.Down:
                            VM.gameArea.PacManDownMove();
                            break;
                        case CircleDirections.Left:
                            VM.gameArea.PacManLeftMove();
                            break;
                        case CircleDirections.Right:
                            VM.gameArea.PacManRightMove();
                            break;
                    }

                }
                else
                {
                    Erase(OldPosition);
                    DrawPacManImage();
                }
                IsMove = !IsMove;
            }

            void DrawOneCircle(System.Drawing.Point location, Graphics graphics, Model.PositionState BeanKind)
            {
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.LightYellow);
                System.Drawing.Rectangle rectangle = System.Drawing.Rectangle.FromLTRB(location.X + (StepLength - CommonBeanLength) / 2, location.Y + (StepLength - CommonBeanLength) / 2, location.X + (StepLength - CommonBeanLength) / 2 + CommonBeanLength, location.Y + (StepLength - CommonBeanLength) / 2 + CommonBeanLength);
                if (BeanKind == Model.PositionState.SpecialBean)
                {
                    brush = new SolidBrush(System.Drawing.Color.Orange);
                    rectangle.Inflate(new System.Drawing.Size(2, 2));
                }
                graphics.FillEllipse(brush, rectangle);
            }
            void DrawGame_Area()
            {
                paper.Lock();
                Bitmap BackPaper = new Bitmap(Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight), paper.BackBufferStride, System.Drawing.Imaging.PixelFormat.Format32bppArgb, paper.BackBuffer);
                Graphics graphics = Graphics.FromImage(BackPaper);
            for (int y = 0; y < VM.gameArea.Game_Area.GetLength(1); y++)
                {
                    for (int x = 0; x < VM.gameArea.Game_Area.GetLength(0); x++)
                    {
                        switch (VM.gameArea.Game_Area[x, y])
                        {
                            case Model.PositionState.CommonBean:
                                DrawCommonBean(new System.Drawing.Point(x * StepLength, y * StepLength), graphics);
                                break;
                            case Model.PositionState.SpecialBean:
                                DrawSpecialBean(new System.Drawing.Point(x * StepLength, y * StepLength), graphics);
                                break;
                            case Model.PositionState.Wall:
                                DrawOneRectangle(new System.Drawing.Point(x * StepLength, y * StepLength), graphics, System.Drawing.Color.Blue);
                                break;
                        default:
                            Erase(new System.Drawing.Point(x, y));
                            break;
                        }
                    }
                }
            if(VM.gameArea.pacMan.IsWin)
            {
                DrawGameImage(YouWinImage);
            }
            else if (VM.gameArea.pacMan.IsDead)
            {
                DrawGameImage(GameOverImage);
            }
            DrawPacManImage();
                graphics.Flush();
                graphics.Dispose();
                graphics = null;
                BackPaper.Dispose();
                BackPaper = null;
                paper.AddDirtyRect(new Int32Rect(0, 0, Convert.ToInt32(CanvasDraw.ActualWidth), Convert.ToInt32(CanvasDraw.ActualHeight)));
                paper.Unlock();
            }
            void DrawCommonBean(System.Drawing.Point location, Graphics graphics)
            {
                DrawOneRectangle(location, graphics, System.Drawing.Color.Black);
                DrawOneCircle(location, graphics, Model.PositionState.CommonBean);
            }
            void DrawSpecialBean(System.Drawing.Point location, Graphics graphics)
            {
                DrawOneRectangle(location, graphics, System.Drawing.Color.Black);
                DrawOneCircle(location, graphics, Model.PositionState.SpecialBean);
            }
            private void Wall_BT_Click(object sender, RoutedEventArgs e)
            {
                Common_BT.IsChecked = false;
                Special_BT.IsChecked = false;
            }

            private void Common_BT_Click(object sender, RoutedEventArgs e)
            {
                Wall_BT.IsChecked = false;
                Special_BT.IsChecked = false;
            }

            private void Special_BT_Click(object sender, RoutedEventArgs e)
            {
                Wall_BT.IsChecked = false;
                Common_BT.IsChecked = false;
            }

            private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
            {
                System.Windows.Point ClickPoint = Mouse.GetPosition(CanvasDraw);
                System.Drawing.Point DrawPoint = new System.Drawing.Point((int)ClickPoint.X / StepLength, (int)ClickPoint.Y / StepLength);
                if (Common_BT.IsChecked == true)
                {
                    VM.gameArea.Game_Area[DrawPoint.X, DrawPoint.Y] = Model.PositionState.CommonBean;
                }
                if (Special_BT.IsChecked == true)
                {
                    VM.gameArea.Game_Area[DrawPoint.X, DrawPoint.Y] = Model.PositionState.SpecialBean;
                }
                if (Wall_BT.IsChecked == true)
                {
                    VM.gameArea.Game_Area[DrawPoint.X, DrawPoint.Y] = Model.PositionState.Wall;
                }
                DrawGame_Area();
            }

            private void BT_Save_Click(object sender, RoutedEventArgs e)
            {
                VM.Save();
            }

            private void BT_Load_Click(object sender, RoutedEventArgs e)
            {
                Erase(new System.Drawing.Point(-1, -1));
                VM.Load();
            VM.Reset();
            VM.gameArea.Reset();
            VM.gameArea.Load(VM.gameArea.MapName);
            DrawGame_Area();
            IsDead = false;
            VM.gameArea.pacMan.IsWin = false;
        }

            Stopwatch stopwatch = new Stopwatch();
            private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
            {
                //if (stopwatch.IsRunning)
                //{
                //    stopwatch.Stop();
                //    Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                //    stopwatch.Reset();
                //    stopwatch.Start();
                //}
                //else stopwatch.Start();
                //switch (e.Key)
                //{
                //    case Key.Up:
                //        Move(CircleDirections.Up);
                //        break;
                //    case Key.Down:
                //        Move(CircleDirections.Down);
                //        break;
                //    case Key.Left:
                //        Move(CircleDirections.Left);
                //        break;
                //    case Key.Right:
                //        Move(CircleDirections.Right);
                //        break;
                //}
            }

            private void Window_KeyUp(object sender, KeyEventArgs e)
            {

                //if (!IsMove)
                //{
                //    Window_PreviewKeyDown(null, e);
                //}
                //if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
                //{
                //    DrawImage();
                //}
            }

        private void BT_Play_Click(object sender, RoutedEventArgs e)
        {
            if(VM.gameArea.MapName=="")
            {
                MessageBox.Show(this,"Please Load/Save+Load the map first","error");
                return;
            }
            VM.Reset();
            VM.gameArea.Reset();
            VM.gameArea.Load(VM.gameArea.MapName);
            DrawGame_Area();
            IsDead = false;
            VM.gameArea.pacMan.IsWin = false;
            if (MonsterPlayThread == null || !MonsterPlayThread.IsAlive)
            {
            MonsterPlayThread = new Thread(MonsterPlay);
            MonsterPlayThread.Name = "MonsterPlayThread";
            MonsterPlayThread.IsBackground = true;
            MonsterPlayThread.Start();
            }
            if (PacManPlayThread == null || !PacManPlayThread.IsAlive)
            {
                PacManPlayThread = new Thread(PacManPlay);
                PacManPlayThread.Name = "PacManPlayThread";
                PacManPlayThread.IsBackground = true;
                PacManPlayThread.Start();
            }
    }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VM.Reset();
            File.WriteAllText("HighestScore", VM.HighiestScore.ToString());
        }


    }
}
