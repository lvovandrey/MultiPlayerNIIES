using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.View;
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
        public VideoPlayerView Body; //Ну это не настоящая VM
        #endregion

        #region Конструкторы
        public PlayerPanelVM(VideoPlayerView body)
        {
            Body = body;
            SubscriptedToEventsInPlayerDepPropChanges();
        }
        #endregion

        #region Методы
        private void SubscriptedToEventsInPlayerDepPropChanges()
        {
            Body.VLC.OnPositionChanged += (d, e) => { OnPropertyChanged("Position"); };
            Body.VLC.OnVolumeChanged += (d, e) => { OnPropertyChanged("Volume"); };
            Body.VLC.OnCurTimeChanged += (d, e) => { OnPropertyChanged("CurTime"); };
        }
        #endregion

        #region Свойства
        [Magic]
        public double Position { get { return Body.VLC.Position; } set { Body.VLC.Position = value; } }

        [Magic]
        public double Volume { get { return Body.VLC.Volume; } set { Body.VLC.Volume = value; } }

        [Magic]
        public TimeSpan CurTime { get { return Body.VLC.CurTime; } set { Body.VLC.CurTime = value; } }
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
                    if (Body.IsPlaying) Body.Pause();
                    else Body.Play();
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
                    if (Volume > 0) Volume = 0;
                    else Volume = 50;
                }));
            }
        }

        private RelayCommand decSpeedCommand;
        public RelayCommand DecSpeedCommand
        {
            get
            {
                return decSpeedCommand ?? (decSpeedCommand = new RelayCommand(obj => {
                    Body.Rate -= 0.1;
                }));
            }
        }

        private RelayCommand incSpeedCommand;
        public RelayCommand IncSpeedCommand
        {
            get
            {
                return incSpeedCommand ?? (incSpeedCommand = new RelayCommand(obj => {
                    Body.Rate += 0.1;
                }));
            }
        }

        private RelayCommand stepBackwardCommand;
        public RelayCommand StepBackwardCommand
        {
            get
            {
                return stepBackwardCommand ?? (stepBackwardCommand = new RelayCommand(obj => {
                    Body.Pause();
                    Position = Position - (0.1 * 1000 / Body.VLC.Duration.TotalSeconds);
                }));
            }
        }

        private RelayCommand stepForwardCommand;
        public RelayCommand StepForwardCommand
        {
            get
            {
                return stepForwardCommand ?? (stepForwardCommand = new RelayCommand(obj => {
                    Body.Pause();
                    Position = Position + (0.1 * 1000 / Body.VLC.Duration.TotalSeconds);
                }));
            }
        }


        #endregion


    }
}
