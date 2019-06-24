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
    /// Логика взаимодействия для AreaVideoPlayersView.xaml
    /// </summary>
    public partial class AreaVideoPlayersView : UserControl
    {
        public AreaVideoPlayersView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            player1.start();
            player2.start();
            player3.start();
            player4.start();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            player1.Load(@"C:\\tmp\test1.avi");
            player2.Load(@"C:\\tmp\test2.avi");
            player3.Load(@"C:\\tmp\test3.avi");
            player4.Load(@"C:\\tmp\test4.avi");

        }
    }
}
