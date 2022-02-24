using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pac_Man.View_Model
{
    class ViewModel : INotifyPropertyChanged
    {
        public void Reset()
        {
            Model.Score_System.Reset();
        }
        public int HighiestScore
        {
            get
            {
                return Model.Score_System.HighiestScore;
            }
            set
            {
                Model.Score_System.HighiestScore = value;
                NotifyPropertyChanged("HighiestScore");
            }
        }
        public int CurrentScore
        {
            get
            {
                return Model.Score_System.CurrentScore;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Model.PositionState m_PositionState = Model.PositionState.Space;
        public  Model.GameArea gameArea = new Model.GameArea();
        public  ViewModel()
        {
            gameArea.OnCurrentScoreChanged += GameArea_OnCurrentScoreChanged;
            //gameArea.ScoreChanged = (() => NotifyPropertyChanged("CurrentScore"));
        }

        private void GameArea_OnCurrentScoreChanged(object Sender, EventArgs e)
        {
            NotifyPropertyChanged("CurrentScore");
        }

        public  Model.PositionState PositionState
        {
            get
            {
                return m_PositionState;
            }
            set
            {
                if(m_PositionState!=value)
                {
                    m_PositionState = value;
                    NotifyPropertyChanged("PositionState");
                }
            }
        }
        public void Save()
        {
            gameArea.Save();
        }
        public void Load()
        {
            gameArea.Load();
        }
    }
}
