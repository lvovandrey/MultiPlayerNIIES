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

namespace MultiPlayerNIIES.View.Elements
{
    /// <summary>
    /// Логика взаимодействия для SyncronizationShiftView.xaml
    /// </summary>
    public partial class SyncronizationShiftView : UserControl
    {
        public SyncronizationShiftView()
        {
            InitializeComponent();
        }

        private void ShiftMaxTimeHidingImage_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IntervalColumn.ActualWidth > 50) IntervalColumn.Width = new GridLength(0);
            else IntervalColumn.Width = new GridLength(100);
        }
    }
}
