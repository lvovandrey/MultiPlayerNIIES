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
            //VLC.play();
            
            //ME.Play();
            FFME.Play();
        }
        public void Load(string filepath)
        {

            //--------------------------------------
            //VLC
            //VLC.vlc.MediaPlayer.Play(new Uri(@filepath));
            //Tools.ToolsTimer.Delay(() => { VLC.pause(); }, TimeSpan.FromSeconds(0.1));

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
            FFME.Source = new Uri(@filepath);
            FFME.Play();
            Tools.ToolsTimer.Delay(() =>
            {
                FFME.Pause();
                FFME.Position = TimeSpan.FromSeconds(4);
            }, TimeSpan.FromSeconds(0.1));
        }
    }
}
