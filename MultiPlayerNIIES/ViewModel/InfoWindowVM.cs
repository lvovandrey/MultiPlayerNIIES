using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{
    public class InfoWindowVM : INPCBase
    {

        #region Поля 
        #endregion

        #region Конструкторы
        public InfoWindowVM(string message, string windowTitle)
        {

        }
        #endregion

        #region Методы
   
        #endregion

        #region Свойства

        [Magic]
        public string Message
        {
            get;
            set;
        }

      
        #endregion


        #region Комманды

        #endregion


    }
}
