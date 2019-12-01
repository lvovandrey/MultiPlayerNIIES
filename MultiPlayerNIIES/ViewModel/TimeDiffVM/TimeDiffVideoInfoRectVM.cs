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
        
        #endregion

        #region Конструкторы
        public TimeDiffVideoInfoRectVM(int Number)
        {
            number = Number;
        }
        #endregion

        #region Методы

        #endregion

        #region Свойства
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

        public System.Drawing.Bitmap SnapShotTimeDiff
        {
            get
            {
                return TimeDiffVideo.SnapShotsOnPositions[0]; //TODO: тут не всегда так! исправить
            }

            // изменяется когда меняется position
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
