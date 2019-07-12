using MultiPlayerNIIES.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Логика взаимодействия для VideoPlayerView.xaml
    /// </summary>
    public partial class VideoPlayerView : UserControl
    {
        public VideoPlayerView()
        {
            InitializeComponent();
            ToolsTimer.Timer(TimerTick, TimeSpan.FromSeconds(0.05));
        }

        private void TimerTick()
        {
            if (subtitler != null && subtitler.Ready)
            {
                TextBlockSubtitles.Text = subtitler.Subtitles[subtitler.BinarySearch(VLC.CurTime)].Text;
            }
        }


        public void start()
        {
            VLC.play();
            
            //ME.Play();
           // FFME.Play();
        }
        public void Load(string filepath)
        {

            //--------------------------------------
            //VLC
            VLC.Source = new Uri(@filepath);

            //--------------------------------------
            //MediaELEMENT
            //ME.Source = new Uri(@filepath);
            //ME.Play();
            //Tools.ToolsTimer.Delay(() =>
            //{
            //    ME.Pause();
            //    ME.Position = TimeSpan.FromSeconds(4);
            //}, TimeSpan.FromSeconds(0.1));

            //--------------------------------------
            //FFME
            //FFME.Source = new Uri(@filepath);
            //FFME.Play();
            //Tools.ToolsTimer.Delay(() =>
            //{
            //    FFME.Pause();
            //    FFME.Position = TimeSpan.FromSeconds(4);
            //}, TimeSpan.FromSeconds(0.1));

            
        }

        internal void Pause()
        {
            VLC.pause();
        }

        public void SetPosition(TimeSpan position)
        {
            VLC.pause();

            double pos = 1000* position.TotalSeconds / VLC.Duration.TotalSeconds;

            Tools.ToolsTimer.Delay(() => { VLC.Position = pos; }, TimeSpan.FromSeconds(2));
        }

        public Subtitler subtitler;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CropBtn_Click(object sender, RoutedEventArgs e)
        {
            VLC.vlc.MediaPlayer.Video.CropGeometry = "1000x1000+300+200";
        }

        private void AspectRatioBtn_Click(object sender, RoutedEventArgs e)
        {
            VLC.vlc.MediaPlayer.Video.AspectRatio = "10:10";
        }
    }
}
