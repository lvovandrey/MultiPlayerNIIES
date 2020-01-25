using MultiPlayerNIIES.ViewModel;
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

        public static InfoWindowResult Show(string message, string title = "Сообщение", InfoWindowButtons buttons = InfoWindowButtons.Ok, InfoWindowIcons icon = InfoWindowIcons.Info, Window owner = null)
        {

            InfoWindow wnd = new InfoWindow();
            if (owner == null)
                wnd.Owner = MainWindow.mainWindow;
            else
                wnd.Owner = owner;

            InfoWindowVM infoWindowVM = new InfoWindowVM(message, title, buttons, icon);
            wnd.DataContext = infoWindowVM;

            if (wnd.ShowDialog().GetValueOrDefault())
                return wnd.Result;
            else return InfoWindowResult.Cancel; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InfoWindowResult Res = InfoWindowResult.Cancel;
            Enum.TryParse((sender as Button).Tag.ToString(), out Res); 
            Result = Res;
            this.DialogResult = true;
            this.Close();
        }
    }


}
