using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View.DialogWindows
{
    public enum InfoWindowButtons
    {
        Ok,
        YesNo,
        OkCancel
    }
    public enum InfoWindowIcons
    {
        Info,
        Warning,
        Error,
        None
    }
    public enum InfoWindowResult
    {
        Ok,
        Cancel,
        Yes,
        No
    }
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        InfoWindowResult Result = InfoWindowResult.Cancel;

        public static InfoWindowResult Show(string message, string title="Информация", InfoWindowButtons buttons = InfoWindowButtons.Ok, InfoWindowIcons icon = InfoWindowIcons.Info)
        {

            var wnd = new InfoWindow();

            if (wnd.ShowDialog().GetValueOrDefault())
                return wnd.Result;
            else return InfoWindowResult.Cancel; 
        }



        private void buttonOk_onClick(object sender, RoutedEventArgs e)
        {
            Result = InfoWindowResult.Ok;
            this.DialogResult = true;
            this.Close();
        }

        private void buttonCancel_onClick(object sender, RoutedEventArgs e)
        {
            Result = InfoWindowResult.Cancel;
            this.DialogResult = true;
            this.Close();

        }
    }


}
