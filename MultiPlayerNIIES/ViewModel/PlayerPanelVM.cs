using MultiPlayerNIIES.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{

    public class PlayerPanelVM : INPCBase
    {

        #region Поля 

        #endregion

        #region Конструкторы
        #endregion

        #region Методы
        #endregion

        #region Свойства
        private double position { get; set; }
        [Magic]
        public double Position { get { return position; } set { position = value; } }

        public double volume { get; set; }
        [Magic]
        public double Volume { get { return volume; } set { volume = value; } }

        public TimeSpan curTime { get; set; }
        [Magic]
        public TimeSpan CurTime { get { return curTime; } set { curTime = value; } }
        #endregion


        #region Комманды
        /* Просто добавь ctrl+v =) 
        private RelayCommand Command;  
        public RelayCommand Command { get { return Command ?? (Command = new RelayCommand(obj =>{ 
        }));}}             */

        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get
            {
                return playPauseCommand ?? (playPauseCommand = new RelayCommand(obj =>
                {

                }));
            }
        }


        private RelayCommand muteCommand;
        public RelayCommand MuteCommand
        {
            get
            {
                return muteCommand ?? (muteCommand = new RelayCommand(obj =>
                {
                }));
            }
        }

        private RelayCommand decSpeedCommand;
        public RelayCommand DecSpeedCommand
        {
            get
            {
                return decSpeedCommand ?? (decSpeedCommand = new RelayCommand(obj => {
                }));
            }
        }

        private RelayCommand incSpeedCommand;
        public RelayCommand IncSpeedCommand
        {
            get
            {
                return incSpeedCommand ?? (incSpeedCommand = new RelayCommand(obj => {
                }));
            }
        }

        private RelayCommand stepBackwardCommand;
        public RelayCommand StepBackwardCommand
        {
            get
            {
                return stepBackwardCommand ?? (stepBackwardCommand = new RelayCommand(obj => {
                }));
            }
        }

        private RelayCommand stepForwardCommand;
        public RelayCommand StepForwardCommand
        {
            get
            {
                return stepForwardCommand ?? (stepForwardCommand = new RelayCommand(obj => {
                }));
            }
        }


        #endregion


    }
}
