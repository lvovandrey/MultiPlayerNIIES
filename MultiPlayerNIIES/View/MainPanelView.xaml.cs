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

namespace MultiPlayerNIIES.View
{
    /// <summary>
    /// Логика взаимодействия для MainPanelView.xaml
    /// </summary>
    public partial class MainPanelView : UserControl
    {
        public MainPanelView()
        {
            InitializeComponent();
            Timer = new System.Windows.Threading.DispatcherTimer();
        }

        System.Windows.Threading.DispatcherTimer Timer;


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (Timer == null)
            {
                Timer = new System.Windows.Threading.DispatcherTimer();
            }
            Timer.Tick += (s, _) =>
            {
                var DC = DataContext as ViewModel.VM;
                DC.SyncronizationShiftCommand.Execute(null);
            };
            Timer.Interval = TimeSpan.FromSeconds(20);
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }
    }
}
