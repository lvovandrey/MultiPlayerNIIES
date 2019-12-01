using MultiPlayerNIIES.Model;
using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.ViewModel;
using MultiPlayerNIIES.ViewModel.TimeDiffVM;
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

namespace MultiPlayerNIIES.View.TimeDiffElements
{
    /// <summary>
    /// Логика взаимодействия для TimeDIffWindowWindow.xaml
    /// </summary>
    public partial class TimeDIffWindowWindow : Window
    {
        List<VideoInfoRect> videoInfoRects;
        double ColWidth = 300;

        public TimeDIffWindowWindow()
        {
            InitializeComponent();
        }

     


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //foreach (var v in videoInfoRects)
            //{
            //    v.SeparatorMarginLeft = this.ColumnLeft.ActualWidth;
            //    v.OnSizeContaierChanged();
            //}
        }



        public void FormatWindowContent()
        {
            VideoInfoRect PrevVideoInfoRect = null;
            foreach (var item in this.MainGrid.Children)
            {
                VideoInfoRect videoInfoRect = item as VideoInfoRect;
                if (item == null) return;
                FormatingVideoInfoRect(videoInfoRect, PrevVideoInfoRect);
                PrevVideoInfoRect = videoInfoRect;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Dictionary<VideoPlayerVM, TimeSpan> dict = new Dictionary<VideoPlayerVM, TimeSpan>();
            int SyncLeadPos = 0;

            TimeSpan TimeSyncLead = TimeSpan.Zero;
            foreach (var v in TimeDiffMeasuringManager.TimeDiffVideos)
                if (v.IsSyncLead) TimeSyncLead = v.CurrentPosition.Time;


            foreach (var v in TimeDiffMeasuringManager.TimeDiffVideos)
            {
                TimeSpan T = TimeSpan.Zero;
                T = v.CurrentPosition.Time - TimeSyncLead + v.VideoPlayerVM.SyncronizationShiftVM.CurrentShiftTime;
                dict.Add(v.VideoPlayerVM, T);
            }

            ((TimeDiffWindowVM)DataContext).VM.SetCustomShiftsOfSyncronization(dict);

            Hide();
        }




        internal void AddVideo(TimeDiffVideoInfoRectVM videoVM)
        {
            VideoInfoRect video = new VideoInfoRect(this.MainGrid, ColWidth);
            video.DataContext = videoVM;
            this.MainGrid.Children.Add(video);
        }

        internal void FormatingVideoInfoRect(VideoInfoRect v, VideoInfoRect prev)
        {

            v.HorizontalAlignment = HorizontalAlignment.Left;
            v.VerticalAlignment = VerticalAlignment.Top;
            v.Height = v.TextVideoFileName.ActualHeight + v.TextSyncLead.ActualHeight + 10;

            if (prev == null)
                v.Margin = new Thickness(20, 50, 0, 0);
            else
                v.Margin = new Thickness(20, prev.Margin.Top + prev.Height + 10, 0, 0);

            v.Margin = new Thickness(((TimeDiffVideoInfoRectVM)v.DataContext).CurrentPosition * ColWidth + 20, v.Margin.Top, 0, 0);
        }


        internal void AddColumn(TimeDiffColumnVM columnVM)
        {
            Column column = new Column();
            column.Width = ColWidth;
            column.DataContext = columnVM;
            this.ColumnStack.Children.Add(column);
        }

        internal void ClearColumns()
        {
            ColumnStack.Children.Clear();
        }
        internal void ClearVideoInfoRects()
        {
            MainGrid.Children.Clear();
        }
    }
}

