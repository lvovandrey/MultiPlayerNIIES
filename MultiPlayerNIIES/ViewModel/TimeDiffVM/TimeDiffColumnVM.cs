using MultiPlayerNIIES.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiPlayerNIIES.Model;

namespace MultiPlayerNIIES.ViewModel.TimeDiffVM
{
    class TimeDiffColumnVM : INPCBase
    {

        #region Поля 
        private TimeDiffPosition position;
        private TimeSpan TimeSyncLead;
        #endregion

        #region Конструкторы
        public TimeDiffColumnVM(int ColNumber)
        {
            colNumber = ColNumber;
            position = TimeDiffMeasuringManager.TimeDiffPositions[ColNumber - 1];

            TimeSyncLead = TimeDiffMeasuringManager.TimeSyncLead;
        }
        #endregion

        #region Методы

        #endregion

        #region Свойства

        public TimeSpan TimeDiffMeasured
        {
            get { return position.Time - TimeSyncLead; }
        }

        private int colNumber;
        public int ColNumber
        {
            get { return colNumber; }
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
