using MultiPlayerNIIES.Tools;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View
{
    /// <summary>
    /// Логика взаимодействия для InfoWindowView.xaml
    /// </summary>
    public partial class InfoWindowView : Window
    {
        VM vm;
        public InfoWindowView(VM vm)
        {
            InitializeComponent();
            this.vm = vm;
        }

        public void ShowMe(string text, TimeSpan interval)
        {
            this.Left = vm.MainWindow.Left + vm.MainWindow.ActualWidth / 2 - this.ActualWidth / 2;
            this.Top = vm.MainWindow.Top + vm.MainWindow.ActualHeight / 2 - this.ActualHeight / 2;
            WaitProgressBar.TextBlockInfo.Text = text;
            Panel.SetZIndex(this, 1000);
            this.Visibility = Visibility.Visible;
            this.Activate();
            WaitProgressBar.WaitProgressBar1.BeginAnimation(ProgressBar.ValueProperty, new DoubleAnimation(0, 100, interval));
            ToolsTimer.Delay(() => { this.Visibility = Visibility.Hidden; WaitProgressBar.TextBlockInfo.Text = ""; WaitProgressBar.WaitProgressBar1.Value = 0; }, interval);
        }
    }
}
