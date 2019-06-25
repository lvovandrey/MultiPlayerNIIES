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
    /// Логика взаимодействия для VideoPlayerView.xaml
    /// </summary>
    public partial class VideoPlayerView : UserControl
    {
        public VideoPlayerView()
        {
            InitializeComponent();
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
            //VLC.vlc.MediaPlayer.Pause();
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(1));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(2));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(3));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(4));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(5));

            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(6));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(7));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(8));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(9));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(10));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(11));

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
    }
}
