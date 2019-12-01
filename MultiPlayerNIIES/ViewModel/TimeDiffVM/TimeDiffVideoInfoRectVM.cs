using MultiPlayerNIIES.Abstract;
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
        public TimeDiffVideoInfoRectVM()
        {

        }
        #endregion

        #region Методы

        #endregion

        #region Свойства
        public bool IsSyncronizeLeader
        {
            get; set;
        }
        public string FilenameForTitle
        {
            get; set;
        }

        public System.Drawing.Bitmap SnapShotTimeDiff
        {
            get; set;// изменяется когда меняется position
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
