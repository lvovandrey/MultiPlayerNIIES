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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiPlayerNIIES.View
{
    /// <summary>
    /// Логика взаимодействия для MainSlider.xaml
    /// </summary>
    public partial class MainSlider : UserControl
    {
        public MainSlider()
        {
            InitializeComponent();
        }

        private void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            VM vm = DataContext as VM; // да хуй с ним знаю что уродство
            if (vm == null) return;
            vm.SyncLeadPlayer.Body.VLC.TimeSlider_PreviewMouseDown(sender, e);
        }

        private void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            VM vm = DataContext as VM; // да хуй с ним знаю что уродство
            if (vm == null) return;
            vm.SyncLeadPlayer.Body.VLC.TimeSlider_PreviewMouseUp(sender, e);
        }

    }
}
