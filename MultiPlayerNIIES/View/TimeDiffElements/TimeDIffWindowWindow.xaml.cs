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
        public TimeDIffWindowWindow()
        {
            InitializeComponent();
        }

        public void AddVideoInfoRects()
        {

            VM vm = DataContext as VM;
            if (vm == null) return;

            videoInfoRects = new List<VideoInfoRect>();
            this.MainGrid.Children.Clear();

           

            foreach (var v in vm.videoPlayerVMs)
            {
                VideoInfoRect videoInfoRect = new VideoInfoRect(this.MainGrid);

                videoInfoRect.DataContext = v;
                videoInfoRect.SeparatorMarginLeft = this.ColumnLeft.ActualWidth;
                videoInfoRects.Add(videoInfoRect);

            }


            foreach (var virect in videoInfoRects)
            {
                this.MainGrid.Children.Add(virect);
            }

            ToolsTimer.Delay(() =>
            {
                FormatWindowContent();

            }, TimeSpan.FromSeconds(0.1));
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


        private void FormatWindowContent()
        {
            VideoInfoRect PrevVideoInfoRect = null;
            foreach (var item in this.MainGrid.Children)
            {
                VideoInfoRect videoInfoRect = item as VideoInfoRect;
                if (item == null) return;
                videoInfoRect.HorizontalAlignment = HorizontalAlignment.Left;
                videoInfoRect.VerticalAlignment = VerticalAlignment.Top;
                videoInfoRect.Height = videoInfoRect.TextVideoFileName.ActualHeight + videoInfoRect.TextSyncLead.ActualHeight + 10;
                if (PrevVideoInfoRect == null)
                    videoInfoRect.Margin = new Thickness(50, 50, 0, 0);
                else
                {
                    videoInfoRect.Margin = new Thickness(50, PrevVideoInfoRect.Margin.Top + PrevVideoInfoRect.Height + 10, 0, 0);
                    videoInfoRect.Position = 1;
                    videoInfoRect.OnSizeContaierChanged();
                }
                PrevVideoInfoRect = videoInfoRect;
            }




        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VM vm = DataContext as VM;
            if (vm == null) return;

            Dictionary<VideoPlayerVM, TimeSpan> dict = new Dictionary<VideoPlayerVM, TimeSpan>();
            int SyncLeadPos = 0;

            foreach (var v in videoInfoRects)
            {
                VideoPlayerVM vpvm = v.DataContext as VideoPlayerVM;
                if (vpvm.IsSyncronizeLeader)
                    SyncLeadPos = v.Position;
            }

            foreach (var v in videoInfoRects)
            {
                TimeSpan T = TimeSpan.Zero;
                VideoPlayerVM vpvm = v.DataContext as VideoPlayerVM;
                if (!vpvm.IsSyncronizeLeader)
                {
                    if(v.Position != SyncLeadPos && v.Position==1)
                        T = vm.TimeDiffMeasured + vpvm.SyncronizationShiftVM.CurrentShiftTime;
                    if (v.Position != SyncLeadPos && v.Position == 0)
                        T = -vm.TimeDiffMeasured + vpvm.SyncronizationShiftVM.CurrentShiftTime;
                }
                dict.Add(vpvm, T);
            }

            vm.SetCustomShiftsOfSyncronization(dict);

            Hide();
        }




        internal void AddVideo(TimeDiffVideoInfoRectVM videoVM)
        {
            MessageBox.Show("метод AddVideo не создан");
        }

        internal void AddColumn(TimeDiffColumnVM columnVM)
        {
            Column column = new Column();
            column.DataContext = columnVM;
            this.ColumnStack.Children.Add(column);
        }

        internal void ClearColumns()
        {
            this.ColumnStack.Children.Clear();
        }

    }
}
