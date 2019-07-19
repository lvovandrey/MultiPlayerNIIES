using MultiPlayerNIIES.Tools;
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
            player1.DragDropSwitchOn(GridMain);
            player2.DragDropSwitchOn(GridMain);
            player3.DragDropSwitchOn(GridMain);
            player4.DragDropSwitchOn(GridMain);
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            player1.subtitler = new Subtitler();
            player2.subtitler = new Subtitler();
            player3.subtitler = new Subtitler();
            player4.subtitler = new Subtitler();


            player1.subtitler.LoadSubtitles(@"C:\\tmp\test1.srt");
            player2.subtitler.LoadSubtitles(@"C:\\tmp\test2.srt");
            player3.subtitler.LoadSubtitles(@"C:\\tmp\test3.srt");
            player4.subtitler.LoadSubtitles(@"C:\\tmp\test4.srt");
        }



        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            player1.Pause();
            player2.Pause();

            //Берем от титров на первой камере время титров
            TimeSpan SyncTitlesTime = player1.subtitler.Subtitles[player1.subtitler.BinarySearch(player1.VLC.CurTime)].TimeFromText;

            //ищем на 1 камере в титрах время Begin которому соответствует время из текста титров SyncTitlesTime - это чтобы точно поставить и первую камеру в нужное время
            TimeSpan SyncCam1Time = player1.subtitler.Subtitles[player1.subtitler.BinarySearchInTimesFromTitles(SyncTitlesTime)].Begin;

            //ищем на 2 камере в титрах время Begin которому соответствует время из текста титров SyncTitlesTime
            TimeSpan SyncCam2Time = player2.subtitler.Subtitles[player2.subtitler.BinarySearchInTimesFromTitles(SyncTitlesTime)].Begin;
            TimeSpan SyncCam3Time = player3.subtitler.Subtitles[player3.subtitler.BinarySearchInTimesFromTitles(SyncTitlesTime)].Begin;
            TimeSpan SyncCam4Time = player4.subtitler.Subtitles[player4.subtitler.BinarySearchInTimesFromTitles(SyncTitlesTime)].Begin;

            //Устанавливаем вычисленное время на 2 камере. По аналогии - с остальными камерами поступать так же.
            player1.SetPosition(SyncCam1Time);
            player2.SetPosition(SyncCam2Time);
            player3.SetPosition(SyncCam3Time);
            player4.SetPosition(SyncCam4Time);

            ToolsTimer.Delay(() => 
            {
                player1.start();
                player2.start();
                player3.start();
                player4.start();

            }, TimeSpan.FromSeconds(3));

        }
    }
}
