using Microsoft.Win32;
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

namespace MultiPlayerNIIES.View.MetaVLCPlayer
{

    public delegate void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);

    /// <summary>
    /// Логика взаимодействия для MVLCPlayer.xaml
    /// </summary>
    public partial class MVLCPlayer : UserControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
         typeof(double), typeof(MVLCPlayer),
         new FrameworkPropertyMetadata(new PropertyChangedCallback(PositionPropertyChangedCallback)));

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration",
          typeof(TimeSpan), typeof(MVLCPlayer),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(DurationPropertyChangedCallback)));

        public static readonly DependencyProperty CurTimeProperty = DependencyProperty.Register("CurTime",
            typeof(TimeSpan), typeof(MVLCPlayer),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(CurTimePropertyChangedCallback)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(Uri), typeof(MVLCPlayer),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(SourcePropertyChangedCallback)));

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume",
            typeof(double), typeof(MVLCPlayer),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(VolumePropertyChangedCallback)));


        public double Volume { get { return (double)GetValue(VolumeProperty); } set { SetValue(VolumeProperty, value); } }
        public double Position { get { return (double)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }
        public TimeSpan Duration { get { return (TimeSpan)GetValue(DurationProperty); } set { SetValue(DurationProperty, value); } }
        public TimeSpan CurTime { get { return (TimeSpan)GetValue(CurTimeProperty); } set { SetValue(CurTimeProperty, value); } }
        public Uri Source { get { return (Uri)GetValue(SourceProperty); } set { SetValue(SourceProperty, value); } }

        public event PropertyChanged OnVolumeChanged;
        public event PropertyChanged OnPositionChanged;
        public event PropertyChanged OnDurationChanged;
        public event PropertyChanged OnCurTimeChanged;
        public event PropertyChanged OnSourceChanged;



        static void VolumePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((MVLCPlayer)d).OnVolumeChanged != null)
                ((MVLCPlayer)d).OnVolumeChanged(d, e);
        }

        static void PositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((MVLCPlayer)d).OnPositionChanged != null)
                ((MVLCPlayer)d).OnPositionChanged(d, e);
        }

        static void DurationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((MVLCPlayer)d).OnDurationChanged != null)
                ((MVLCPlayer)d).OnDurationChanged(d, e);
        }

        static void CurTimePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((MVLCPlayer)d).OnCurTimeChanged != null)
                ((MVLCPlayer)d).OnCurTimeChanged(d, e);
        }

        static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((MVLCPlayer)d).OnSourceChanged != null)
                ((MVLCPlayer)d).OnSourceChanged(d, e);
        }




        System.Windows.Threading.DispatcherTimer timer;

        internal void RateIncreace()
        {
            IncSpeedBtn_Click(null, null);
        }
        internal void RateDecreace()
        {
            DecSpeedBtn_Click(null, null);
        }


        public MVLCPlayer()
        {
            InitializeComponent();

            DragDropSwitchOn(MainGrid, vlc);



            Tools.ToolsTimer.Delay(() => { pause(); }, TimeSpan.FromSeconds(0.1));

            this.OnSourceChanged += VideoPlayer_OnPlayerSourceChanged;
            this.OnPositionChanged += Player_OnPositionChanged;
            this.OnCurTimeChanged += Player_OnCurTimeChanged;
            this.OnVolumeChanged += Player_OnVolumeChanged;


            timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick);
            timer.Interval = TimeSpan.FromSeconds(0.05);
            timer.Start();
        }
        private void timerTick(object sender, EventArgs e)
        {
            if (vlc.Time> TimeSpan.FromSeconds(0.01))
            {
                Duration =vlc.Length;
                CurTime = vlc.Time;
                Position = 1000 * CurTime.TotalMilliseconds / Duration.TotalMilliseconds;
            }
        }

        public void SetCurTime(TimeSpan time)
        {
            if (time < TimeSpan.FromSeconds(0)) return;
            vlc.Time = time;
        }

        private void VideoPlayer_OnPlayerSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Source == null) return;
            vlc.Stop();
            vlc.LoadMedia(Source);

            //vlc.Height = ActualHeight - 40;
            //vlc.Width = ActualWidth - 10;

            vlc.Play();
            Volume = 20;
        }

        private void Player_OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((vlc.VlcMediaPlayer != null) && (!vlc.VlcMediaPlayer.IsPlaying))
            {
                vlc.Position = (float)(Position/1000);
            }
        }

        private void Player_OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           // if ((vlc.VlcMediaPlayer.Audio != null))
            {
                vlc.Volume = (int)Volume;
            }
        }

        private void Player_OnCurTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public void pause()
        {
            if (vlc.VlcMediaPlayer.IsPlaying) vlc.Pause();
        }
        public void play()
        {
           vlc.Play();
        }
        public void stop()
        {
            vlc.Stop();
        }


        private void PlayBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (vlc.VlcMediaPlayer.IsPlaying) vlc.Pause();
            else vlc.Play();
        }

        private void SplitBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MuteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vlc.Volume != 0) Volume = 0;
            else Volume = 50;

        }

        public bool IsPlaying { get { return vlc.VlcMediaPlayer.IsPlaying; } }
        bool WasPlaing;

        private void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WasPlaing = vlc.VlcMediaPlayer.IsPlaying;
            if (vlc.VlcMediaPlayer.IsPlaying) { vlc.VlcMediaPlayer.Pause(); timer.Stop(); }
        }

        private void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!vlc.VlcMediaPlayer.IsPlaying)
            {
                if (WasPlaing)
                {
                    vlc.Play(); timer.Start();
                }
                else
                {

                }
            }
        }

        public void SetPosition(double position)// ЧТО ЭТО ЗА ЖЕСТЬ?!!!!!!
        {
            timer.Stop();
            if (vlc.VlcMediaPlayer.IsPlaying)
            {
                vlc.Pause();
                timer.Stop();

                Position = position;
                CurTime = TimeSpan.FromSeconds(Position / 1000 * Duration.TotalSeconds);
                vlc.Position = (float)(Position / 1000);

                ToolsTimer.Delay(() => { vlc.Play(); }, TimeSpan.FromMilliseconds(1000));
            }
            else
            {
                timer.Stop();

                Position = position;
                CurTime = TimeSpan.FromSeconds(Position / 1000 * Duration.TotalSeconds);
                vlc.Position = (float)(Position / 1000);

            }

            ToolsTimer.Delay(() => { timer.Start(); }, TimeSpan.FromMilliseconds(1000));

        }

        private void DecSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vlc.VlcMediaPlayer.IsPlaying) vlc.VlcMediaPlayer.Rate = vlc.VlcMediaPlayer.Rate - 0.1f;
        }

        private void IncSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vlc.VlcMediaPlayer.IsPlaying) vlc.VlcMediaPlayer.Rate = vlc.VlcMediaPlayer.Rate + 0.1f;
        }

        private void FrameBackwardBtn_Click(object sender, RoutedEventArgs e)
        {
            pause();
            Position = Position - (0.1 * 1000 / Duration.TotalSeconds);




        }

        private void FrameForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            pause();
            Position = Position + (0.1 * 1000 / Duration.TotalSeconds);


        }

        public double Rate { get {return (double)vlc.Rate;} }






        #region Реализация Drag'n'Drop

        public UIElement Container;


        public bool IsDragDrop { get; private set; }

        Vector relativeMousePos;
        FrameworkElement draggedObject;

        public void DragDropSwitchOn(UIElement container, FrameworkElement DraggedElement)
        {
            if (IsDragDrop) return;
            Container = container;
            IsDragDrop = true;
            draggedObject = DraggedElement;
            draggedObject.MouseLeftButtonDown += StartDrag;

        }

        public void DragDropSwitchOff()
        {
            if (!IsDragDrop) return;
            Container = null;
            IsDragDrop = false;
            draggedObject.MouseLeftButtonDown -= StartDrag;
            draggedObject = null;
        }

        void StartDrag(object sender, MouseButtonEventArgs e)
        {
            if ((Container == null) || !IsDragDrop) return;
            relativeMousePos = e.GetPosition(draggedObject) - new Point();
            draggedObject.MouseMove += OnDragMove;
            draggedObject.LostMouseCapture += OnLostCapture;
            draggedObject.MouseUp += OnMouseUp;
            Mouse.Capture(draggedObject);
            e.Handled = true;
        }

        void OnDragMove(object sender, MouseEventArgs e)
        {
            UpdatePosition(e);
        }

        void UpdatePosition(MouseEventArgs e)
        {
            var point = e.GetPosition(Container);
            var newPos = point - relativeMousePos;
            draggedObject.Margin = new Thickness(newPos.X, newPos.Y, 0, 0);
        }

        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FinishDrag(sender, e);
            Mouse.Capture(null);
        }

        void OnLostCapture(object sender, MouseEventArgs e)
        {
            FinishDrag(sender, e);
        }

        void FinishDrag(object sender, MouseEventArgs e)
        {
            draggedObject.MouseMove -= OnDragMove;
            draggedObject.LostMouseCapture -= OnLostCapture;
            draggedObject.MouseUp -= OnMouseUp;
            UpdatePosition(e);
        }
        #endregion


    }
}
