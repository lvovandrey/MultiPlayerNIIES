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

namespace MultiPlayerNIIES.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindowView.xaml
    /// </summary>
    public partial class SettingsWindowView : Window
    {
        public SettingsWindowView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Hide();
        }
    }
}
