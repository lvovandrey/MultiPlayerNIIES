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

            double margintop = 20;
            videoInfoRects = new List<VideoInfoRect>();
            this.MainGrid.Children.Clear();

            foreach (var v in vm.videoPlayerVMs)
            {
                VideoInfoRect videoInfoRect = new VideoInfoRect(this.MainGrid);
                videoInfoRect.Width = 200;
                videoInfoRect.Height = 50;
                videoInfoRect.HorizontalAlignment = HorizontalAlignment.Left;
                videoInfoRect.VerticalAlignment = VerticalAlignment.Top;
                videoInfoRect.Margin = new Thickness(10, margintop, 0, 0);
                margintop += 60;
                videoInfoRect.DataContext = v;
                videoInfoRect.SeparatorMarginLeft = this.ColumnLeft.ActualWidth;
                videoInfoRects.Add(videoInfoRect);

            }


            foreach (var virect in videoInfoRects)
            {
                this.MainGrid.Children.Add(virect);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var v in videoInfoRects)
            {
                v.SeparatorMarginLeft = this.ColumnLeft.ActualWidth;
                v.OnSizeContaierChanged();
            }
        }
    }
}
