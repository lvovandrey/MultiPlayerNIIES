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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.View.DSPlayer;
using MultiPlayerNIIES.ViewModel;

namespace MultiPlayerNIIES
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VM vm;
        public static MainWindow mainWindow;
        public MainWindow()
        {
            InitializeComponent();
            Unosquare.FFME.Library.FFmpegDirectory = @"c:\ffmpeg\";

            vm = new VM(AreaVideoPlayers.GridMain, this);
            DataContext = vm;

           if(CustomTitleGrid.Visibility==Visibility.Visible && this.WindowStyle == WindowStyle.None) this.MaxHeight = SystemParameters.WorkArea.Height+12;
            // TODO: разобраться с WindowChrome wC = WindowChrome.GetWindowChrome(this);

            Dragger.MouseLeftButtonDown += new MouseButtonEventHandler(layoutRoot_MouseLeftButtonDown);
            mainWindow = this;
        }

        void layoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                vm.CloseAppCommand.Execute(null);
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        private void Dragger_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) vm.MaximizeRestoreWindowCommand.Execute(null);
        }
    }
}
