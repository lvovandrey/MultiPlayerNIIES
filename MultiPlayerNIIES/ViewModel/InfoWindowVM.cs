using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.View;
using MultiPlayerNIIES.View.DialogWindows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MultiPlayerNIIES.ViewModel
{
    public class InfoWindowVM : INPCBase
    {

        #region Поля 
        #endregion

        #region Конструкторы
        public InfoWindowVM(string message, string caption,  InfoWindowButtons buttons, InfoWindowIcons icon)
        {
            Message = message;
            Caption = caption;
            IsOkCentralButtonVisible = false;
            IsOkButtonVisible = false;
            IsYesButtonVisible = false;
            IsNoButtonVisible = false;
            IsCancelButtonVisible = false;
            switch (buttons)
            {
                case InfoWindowButtons.Ok: IsOkCentralButtonVisible = true;
                    break;
                case InfoWindowButtons.YesNo: IsYesButtonVisible = true; IsNoButtonVisible = true;
                    break;
                case InfoWindowButtons.OkCancel: IsOkButtonVisible = true; IsCancelButtonVisible = true;
                    break;
                default:
                    break;
            }

        }
        #endregion

        #region Методы

        #endregion

        #region Свойства

        [Magic]
        public string Message { get; set; }
        [Magic]
        public string Caption { get; set; }

        string Imgfilename;
        [Magic]
        public ImageSource ImageSource
        {
            get
            {
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(Imgfilename, UriKind.Relative);
                bi3.EndInit();
                return  bi3;
            }
        }




        [Magic]
        public bool IsOkButtonVisible { get; set; }
        [Magic]
        public bool IsCancelButtonVisible { get; set; }
        [Magic]
        public bool IsYesButtonVisible { get; set; }
        [Magic]
        public bool IsNoButtonVisible { get; set; }
        [Magic]
        public bool IsOkCentralButtonVisible { get; set; }


        #endregion


        #region Комманды

        #endregion


    }
}
