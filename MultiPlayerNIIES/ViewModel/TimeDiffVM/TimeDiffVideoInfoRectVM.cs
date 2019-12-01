using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel.TimeDiffVM
{
    class TimeDiffVideoInfoRectVM : INPCBase
    {

        #region Поля 
        public VideoPlayerVM VideoPlayerVM;
        #endregion

        #region Конструкторы
        public TimeDiffVideoInfoRectVM(int Number, VideoPlayerVM videoPlayerVM)
        {
            number = Number;
            VideoPlayerVM = videoPlayerVM;
        }
        #endregion

        #region Методы

        #endregion

        #region Свойства

        public int CurrentPosition
        {
            get
            {
                return TimeDiffVideo.CurrentPosition.Number-1;
            }
            set
            {
                if (value < TimeDiffMeasuringManager.TimeDiffPositions.Count && value >= 0)
                {
                    TimeDiffVideo.CurrentPosition = TimeDiffMeasuringManager.TimeDiffPositions[value];
                    Console.WriteLine(TimeDiffMeasuringManager.TimeDiffPositions[value]);
                }
                OnPropertyChanged("CurrentPosition");
                OnPropertyChanged("SnapShot");
            }
        }

        public TimeSpan PositionTime
        {
            get
            {
                return TimeDiffVideo.CurrentPosition.Time;
            }
        }



        public bool IsSyncronizeLeader
        {
            get { return TimeDiffVideo.IsSyncLead; }
        }
        public string FilenameForTitle
        {
            get { return TimeDiffVideo.FileName; }
        }

        private int number;
        public int Number { get { return number; } }

        public TimeDiffVideo TimeDiffVideo
        { get { return TimeDiffMeasuringManager.TimeDiffVideos[number]; } }

        public System.Drawing.Bitmap SnapShot
        {
            get
            {
                return TimeDiffVideo.SnapShotsOnPositions[CurrentPosition];
            }
        }

        #endregion

        #region Комманды

        private RelayCommand emptyCommand;
        public RelayCommand EmptyCommand
        {
            get
            {
                return emptyCommand ?? (emptyCommand = new RelayCommand(obj =>
                {


                }));
            }
        }







        #endregion

    }
}
