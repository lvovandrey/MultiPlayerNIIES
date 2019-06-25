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
            player1.Load(@"C:\\tmp\test5.avi");
            player2.Load(@"C:\\tmp\test6.avi");
            player3.Load(@"C:\\tmp\test7.avi");
            player4.Load(@"C:\\tmp\test4.avi");

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            TimeSpan curPos = player1.VLC.CurTime;
            player1.SetPosition(curPos);
            player2.SetPosition(curPos);
            player3.SetPosition(curPos);
            player4.SetPosition(curPos);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            player1.Pause();
            player2.Pause();
            player3.Pause();
            player4.Pause();
        }
    }
}
