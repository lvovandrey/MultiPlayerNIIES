using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.Tools.Subtitles;
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
            player1.subtitleProcessor = new SubtitleProcessor();
            player2.subtitleProcessor = new SubtitleProcessor();
            player3.subtitleProcessor = new SubtitleProcessor();
            player4.subtitleProcessor = new SubtitleProcessor();


            player1.subtitleProcessor.LoadSubtitles(@"C:\\tmp\test1.srt");
            player2.subtitleProcessor.LoadSubtitles(@"C:\\tmp\test2.srt");
            player3.subtitleProcessor.LoadSubtitles(@"C:\\tmp\test3.srt");
            player4.subtitleProcessor.LoadSubtitles(@"C:\\tmp\test4.srt");
        }



        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ////////player1.Pause();
            ////////player2.Pause();

            ////////Берем от титров на первой камере время титров
            ////////int tmp = player1.subtitleProcessor.BinarySearch(player1.VLC.CurTime);
            ////////if (tmp < 0) return;
            ////////TimeSpan SyncTitlesTime = player1.subtitleProcessor.Subtitles[tmp].TimeFromTextBegin;

            TimeSpan SyncTime = player1.VLC.CurTime;
            TimeSpan SyncTitlesTime = player1.subtitleProcessor.GetSubtitle(SyncTime).TimeFromTextBegin;

            int t = SearchAndTools.SmartSearchRecord(SyncTime, player1.subtitleProcessor, player2.subtitleProcessor);
            TimeSpan SyncTime2 = player2.subtitleProcessor.GetSyncTime(SyncTitlesTime, t);


            
            player2.SetPosition(SyncTime2);
            player1.SetPosition(SyncTime2);


            ////////tmp = player1.subtitleProcessor.BinarySearchInTimesFromTitles(SyncTitlesTime); if (tmp < 0) return;

            //////////ищем на 1 камере в титрах время Begin которому соответствует время из текста титров SyncTitlesTime - это чтобы точно поставить и первую камеру в нужное время
            ////////TimeSpan SyncCam1Time = player1.subtitleProcessor.Subtitles[tmp].Begin;

            //////////ищем на 2 камере в титрах время Begin которому соответствует время из текста титров SyncTitlesTime
            ////////tmp = player2.subtitleProcessor.BinarySearchInTimesFromTitles(SyncTitlesTime); if (tmp < 0) return;
            ////////TimeSpan SyncCam2Time = player2.subtitleProcessor.Subtitles[tmp].Begin;

            ////////tmp = player3.subtitleProcessor.BinarySearchInTimesFromTitles(SyncTitlesTime); if (tmp < 0) return;
            ////////TimeSpan SyncCam3Time = player3.subtitleProcessor.Subtitles[tmp].Begin;

            ////////tmp = player4.subtitleProcessor.BinarySearchInTimesFromTitles(SyncTitlesTime); if (tmp < 0) return;
            ////////TimeSpan SyncCam4Time = player4.subtitleProcessor.Subtitles[tmp].Begin;


            ////////player1.Pause();
            ////////player2.Pause();
            ////////player3.Pause();
            ////////player4.Pause();

            //////////Устанавливаем вычисленное время на 2 камере. По аналогии - с остальными камерами поступать так же.
            ////////player1.SetPosition(SyncCam1Time);
            ////////player2.SetPosition(SyncCam2Time);
            ////////player3.SetPosition(SyncCam3Time);
            ////////player4.SetPosition(SyncCam4Time);

            ////////ToolsTimer.Delay(() => 
            ////////{
            ////////    player1.start();
            ////////    player2.start();
            ////////    player3.start();
            ////////    player4.start();

            ////////}, TimeSpan.FromSeconds(3));

        }
    }
}
