using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.Tools.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
using System.Windows.Threading;

namespace MultiPlayerNIIES.View.DSPlayer
{
    public delegate void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);

    /// <summary>
    /// Логика взаимодействия для DSPlayer.xaml
    /// </summary>
    public partial class DSPlayer : UserControl
    {

        DxPlay dxPlay; //TODO: нарушена инкапсуляция


        public DSPlayer()
        {
            InitializeComponent();

            DragDropSwitchOn(MainGrid, vlc);

            //dxPlay = new DxPlay(this.VideoPanel, );

            //Tools.ToolsTimer.Delay(() => { // pause(); 
            //}, TimeSpan.FromSeconds(0.1));

            this.OnSourceChanged += VideoPlayer_OnPlayerSourceChanged;
            this.OnPositionChanged += Player_OnPositionChanged;
            this.OnCurTimeChanged += Player_OnCurTimeChanged;
            this.OnVolumeChanged += Player_OnVolumeChanged;


            timer = new System.Windows.Threading.DispatcherTimer();

            timer.Tick += new EventHandler(timerTick);
            timer.Interval = TimeSpan.FromSeconds(0.05);
            timer.Start();
        }


        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
       typeof(double), typeof(DSPlayer),
       new FrameworkPropertyMetadata(new PropertyChangedCallback(PositionPropertyChangedCallback)));

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration",
          typeof(TimeSpan), typeof(DSPlayer),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(DurationPropertyChangedCallback)));

        public static readonly DependencyProperty CurTimeProperty = DependencyProperty.Register("CurTime",
            typeof(TimeSpan), typeof(DSPlayer),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(CurTimePropertyChangedCallback)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(Uri), typeof(DSPlayer),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(SourcePropertyChangedCallback)));

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register("Volume",
            typeof(double), typeof(DSPlayer),
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
            if (((DSPlayer)d).OnVolumeChanged != null)
                ((DSPlayer)d).OnVolumeChanged(d, e);
        }

        static void PositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((DSPlayer)d).OnPositionChanged != null)
                ((DSPlayer)d).OnPositionChanged(d, e);
        }

        static void DurationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((DSPlayer)d).OnDurationChanged != null)
                ((DSPlayer)d).OnDurationChanged(d, e);
        }

        static void CurTimePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((DSPlayer)d).OnCurTimeChanged != null)
                ((DSPlayer)d).OnCurTimeChanged(d, e);
        }

        static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((DSPlayer)d).OnSourceChanged != null)
                ((DSPlayer)d).OnSourceChanged(d, e);
        }


        public TimeSpan CurTimeEx
        {
            get
            {
                if (dxPlay == null) return TimeSpan.Zero;
                return dxPlay.CurTime;
            }
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


        private void timerTick(object sender, EventArgs e)
        {
            if (dxPlay == null) return;
            if (dxPlay.CurTime > TimeSpan.FromSeconds(0.01) && dxPlay.IsPlaying)
            {
                Duration = dxPlay.Duration;
                CurTime = dxPlay.CurTime;
                Position = 1000 * CurTime.TotalMilliseconds / Duration.TotalMilliseconds;
            }
        }

        public void SetCurTime(TimeSpan time)
        {
            if (time < TimeSpan.FromSeconds(0)) return;
            if (time > dxPlay.Duration) return;
            dxPlay.CurTime = time;
        }

        private void VideoPlayer_OnPlayerSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Source == null) return;
            //if (dxPlay != null)
            //{
            //    dxPlay.Dispose();
            //    dxPlay = null;
            //}

            DxPlay testDx = new DxPlay(VideoPanel2, Source.LocalPath, true); // тестируем - открываем проверяем.... жуткий просто ужасный костыль
            //TODO: Нет, братан, ну с этим надо реально что-то делать.....
            testDx.Start();
            //ToolsTimer.Delay(() =>
            // {
            bool RateOk = testDx.TryRate();

            //  ToolsTimer.Delay(() =>
            //   {

            dxPlay = new DxPlay(VideoPanel, Source.LocalPath, RateOk);

            Duration = dxPlay.Duration;
            //vlc.Height = ActualHeight - 40;
            //vlc.Width = ActualWidth - 10;

            dxPlay.Start();
            Volume = 20;
            testDx.Stop();

            dxPlay.Pause();
            //  testDx.CloseInterfaces();
            //     }, TimeSpan.FromSeconds(0.5));
            // }, TimeSpan.FromSeconds(0.5));
        }


        private void Player_OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((dxPlay != null) && (!dxPlay.IsPlaying))
            {
                dxPlay.Position = Position / 1000;
            }
        }

        private void Player_OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            dxPlay.Volume = Volume;
        }

        private void Player_OnCurTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public void pause()
        {
            if (dxPlay.IsPlaying)
                dxPlay.Pause();

        }
        public void play()
        {
            if (!dxPlay.IsPlaying)
                dxPlay.Start();
        }
        public void stop()
        {
            dxPlay.Stop();
        }


        private void PlayBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (dxPlay.IsPlaying) dxPlay.Pause();
            else dxPlay.Start();
        }



        private void MuteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dxPlay.Volume != 0) Volume = 0;
            else Volume = 50;

        }

        public bool IsPlaying { get { return dxPlay.IsPlaying; } }
        bool WasPlaing;

        public void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            WasPlaing = IsPlaying;
            if (IsPlaying) { pause(); timer.Stop(); }
        }



        public void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsPlaying && WasPlaing)
            {
                play(); timer.Start();
            }
        }

        public void SetPosition(double position)// ЧТО ЭТО ЗА ЖЕСТЬ?!!!!!!
        {
            timer.Stop();
            if (IsPlaying)
            {
                pause();
                timer.Stop();

                Position = position;
                CurTime = TimeSpan.FromSeconds(Position / 1000 * Duration.TotalSeconds);
                dxPlay.Position = Position / 1000;

                ToolsTimer.Delay(() => { play(); }, TimeSpan.FromMilliseconds(1000));
            }
            else
            {
                timer.Stop();
                Position = position;
                CurTime = TimeSpan.FromSeconds(Position / 1000 * Duration.TotalSeconds);
                dxPlay.Position = Position / 1000;
            }

            ToolsTimer.Delay(() => { timer.Start(); }, TimeSpan.FromMilliseconds(1000));
        }

        private void DecSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            Rate += 0.1;
        }

        private void IncSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            Rate -= 0.1;
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

        internal void Step(TimeSpan step)
        {
            Position = Position + (step.TotalSeconds * 1000 / Duration.TotalSeconds);
        }

        public double Rate
        {
            get { return dxPlay.Rate; }
            set { dxPlay.Rate = value; }
        }



        public void OnClosing()
        {
            dxPlay.Dispose();

        }


        #region Реализация Drag'n'Drop

        public UIElement Container;


        public bool IsDragDrop { get; private set; }
        public object GraphicTools { get; private set; }

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
            relativeMousePos = e.GetPosition(draggedObject) - new System.Windows.Point();
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


        public void OnResize()
        {
            Rect r = new Rect(0, 0, vlc.ActualWidth, vlc.ActualHeight); // TODO: преобразовать в пиксели - winforms пиксели понимает....
            dxPlay.SetWindowPosition(r);
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) // TODO: какой-то тупой стиль
        {
            OnResize();
        }


        private void VideoPanel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MessageBox.Show("123");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToolsTimer.Timer(ShowSnap, TimeSpan.FromSeconds(0.0005));
        }


        Stopwatch sw = new Stopwatch();


        public void ShowSnap()
        {
            sw.Restart();
            IntPtr IP = dxPlay.SnapShot();
            Bitmap bmp = dxPlay.IPToBmp(IP);
            BitmapSource imgsrc = GraphicsTools.ToBitmapSource(bmp);
            IMG.Source = imgsrc;
            sw.Stop();
            txt.Text = sw.ElapsedMilliseconds.ToString();
        }
    }
}
