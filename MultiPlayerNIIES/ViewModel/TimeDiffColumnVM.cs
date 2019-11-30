using MultiPlayerNIIES.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{
    class TimeDiffColumnVM : INPCBase
    {

        #region Поля 

        #endregion

        #region Конструкторы

        #endregion

        #region Методы

        #endregion

        #region Свойства

        private TimeSpan timeDiffMeasured;
        public TimeSpan TimeDiffMeasured
        {
            get { return timeDiffMeasured; }
            set { timeDiffMeasured = value; OnPropertyChanged("TimeDiffMeasured");  }
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
