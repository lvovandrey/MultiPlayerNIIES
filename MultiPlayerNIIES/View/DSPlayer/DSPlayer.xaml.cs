using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.Tools.Graphics;
using MultiPlayerNIIES.ViewModel;
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
        DxPlay testDx;

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


        public double Volume
        {
            get
            {
                double t = Application.Current.Dispatcher.Invoke(new Func<double>(() =>
                {
                    return (double)GetValue(VolumeProperty);
                }));
                return t;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetValue(VolumeProperty, value);
                }));
            }
        }
        public double Position
        {
            get
            {
                double t = Application.Current.Dispatcher.Invoke(new Func<double>(() =>
                {
                    return (double)GetValue(PositionProperty);
                }));
                return t;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetValue(PositionProperty, value);
                }));
            }
        }

        public TimeSpan Duration
        {
            get
            {
                TimeSpan t = Application.Current.Dispatcher.Invoke(new Func<TimeSpan>(() =>
                {
                    return (TimeSpan)GetValue(DurationProperty);
                }));
                return t;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetValue(DurationProperty, value);
                }));
            }
        }

        public TimeSpan CurTime
        {
            get
            {
                TimeSpan t = Application.Current.Dispatcher.Invoke(new Func<TimeSpan>(() =>
                {
                    return (TimeSpan)GetValue(CurTimeProperty);
                }));
                return t;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetValue(CurTimeProperty, value);
                }));

            }
        }

        public Uri Source
        {
            get
            {
                Uri t = Application.Current.Dispatcher.Invoke(new Func<Uri>(() =>
                {
                    return (Uri)GetValue(SourceProperty);
                }));
                return t;
            }
            set
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    SetValue(SourceProperty, value);
                }));

            }
        }

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

        /// <summary>
        /// Соотношение сторон
        /// </summary>
        public AspectRatio AspectRatio
        {
            get { return (AspectRatio)VideoMMM.AspectRatio; }
            set { VideoMMM.AspectRatio = (WindowsFormsVideoControl.AspectRatio)value; }
        }


        public System.Windows.Size OriginalSize
        {
            get
            {
                return dxPlay.OriginalSize;
            }
            set
            {
                dxPlay.OriginalSize = value;
            }
        }

        internal void RefreshSize()
        {
            VideoMMM.ResizeVideoContainer();
            OnResize();
        }
        System.Windows.Threading.DispatcherTimer timer;



        internal void RateIncreace()
        {
            if (dxPlay == null) return;
            IncSpeedBtn_Click(null, null);
        }
        internal void RateDecreace()
        {
            if (dxPlay == null) return;
            DecSpeedBtn_Click(null, null);
        }


        private void timerTick(object sender, EventArgs e)
        {
            if (dxPlay == null) return;
            if (dxPlay.CurTime > TimeSpan.FromSeconds(0.01) && (dxPlay.IsPlaying || dxPlay.IsPaused))
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
            bool Ok = true;
            testDx = new DxPlay(VideoPanel2, Source.LocalPath, ref Ok); // тестируем - открываем проверяем.... жуткий просто ужасный костыль
                                                                        //TODO: Нет, братан, ну с этим надо реально что-то делать.....
            if (!Ok) return;
            testDx.Start();
            //ToolsTimer.Delay(() =>
            // {
            bool RateOk = testDx.TryRate();

            bool AllOk = Ok && RateOk;
            if (!Ok) return;
            dxPlay = new DxPlay(VideoMMM.SelectablePictureBox1, Source.LocalPath, ref RateOk);

            Duration = dxPlay.Duration;
            //vlc.Height = ActualHeight - 40;
            //vlc.Width = ActualWidth - 10;

            dxPlay.Start();
            Volume = 50;
            testDx.Stop();

            dxPlay.Pause();


            ToolsTimer.Delay(() =>
             {
                 VideoMMM.FitToFill();
                 OnResize();
             }, TimeSpan.FromSeconds(1.5));

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
            if (dxPlay != null)
                dxPlay.Volume = Volume;
        }

        private void Player_OnCurTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        public void pause()
        {
            if (dxPlay != null)
                if (dxPlay.IsPlaying)
                    dxPlay.Pause();

        }
        public void play()
        {
            if (dxPlay != null)
                if (!dxPlay.IsPlaying)
                    dxPlay.Start();
        }
        public void stop()
        {
            if (dxPlay != null)
                dxPlay.Stop();
        }


        private void PlayBtn_Click_1(object sender, RoutedEventArgs e)
        {
            if (dxPlay != null)
                if (dxPlay.IsPlaying) dxPlay.Pause();
                else dxPlay.Start();
        }


        public bool IsPlaying
        {
            get
            {
                if (dxPlay != null)
                    return dxPlay.IsPlaying;
                else return false;
            }
        }
        public bool IsPaused
        {
            get
            {
                if (dxPlay != null)
                    return dxPlay.IsPaused;
                else return false;
            }
        }

        public bool IsPausedEx
        {
            get
            {
                if (dxPlay != null)
                    return dxPlay.IsPausedEx;
                else return false;
            }
        }

        public bool IsPlayingEx
        {
            get
            {
                if (dxPlay != null)
                    return dxPlay.IsPlayingEx;
                else return false;
            }
        }



        bool WasPlaing;

        public void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dxPlay == null) return;
            WasPlaing = IsPlaying;
            if (IsPlaying)
            { pause(); timer.Stop(); }
        }



        public void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (dxPlay == null) return;
            if (!IsPlaying && WasPlaing)
            {
                play(); timer.Start();
            }
        }

        public void SetPosition(double position)// ЧТО ЭТО ЗА ЖЕСТЬ?!!!!!!
        {
            if (dxPlay == null) return;

            timer.Stop();
            if (IsPlaying)
            {
                pause();
                timer.Stop();

                Position = position;
                CurTime = TimeSpan.FromSeconds(Position / 1000 * Duration.TotalSeconds);
                dxPlay.Position = Position / 1000;

                ToolsTimer.Delay(() => { play(); }, TimeSpan.FromMilliseconds(300));
            }
            else
            {
                timer.Stop();
                Position = position;
                CurTime = TimeSpan.FromSeconds(Position / 1000 * Duration.TotalSeconds);
                dxPlay.Position = Position / 1000;
            }

            ToolsTimer.Delay(() => { timer.Start(); }, TimeSpan.FromMilliseconds(300));
        }

        private void DecSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dxPlay == null) return;
            Rate += 0.1;
        }

        internal void SetSoom(Rect zoomedArea)
        {
            VideoMMM.SetZoom(zoomedArea);
        }

        public Rect GetZoomedArea()
        {
            return VideoMMM.GetZoomedArea();
        }

        private void IncSpeedBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dxPlay == null) return;
            Rate -= 0.1;
        }

        private void FrameBackwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dxPlay == null) return;
            pause();
            Position = Position - (0.1 * 1000 / Duration.TotalSeconds);
        }

        private void FrameForwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (dxPlay == null) return;
            pause();
            Position = Position + (0.1 * 1000 / Duration.TotalSeconds);
        }

        internal void Step(TimeSpan step)
        {
            if (dxPlay == null) return;
            Position = Position + (step.TotalSeconds * 1000 / Duration.TotalSeconds);
        }

        public double Rate
        {
            get
            {
                if (dxPlay != null)
                    return dxPlay.Rate;
                else return 0;
            }
            set
            {
                if (dxPlay != null)
                    dxPlay.Rate = value;
            }
        }



        public void OnClosing()
        {
            if (dxPlay == null) return;

            dxPlay.Dispose();

            testDx.Dispose();
        }


        #region Реализация Drag'n'Drop

        public UIElement Container;


        public bool IsDragDrop { get; private set; }
        public object GraphicTools { get; private set; }

        Vector relativeMousePos;
        FrameworkElement draggedObject;

        public void DragDropSwitchOn(UIElement container, FrameworkElement DraggedElement)
        {
            if (dxPlay == null) return;
            if (IsDragDrop) return;
            Container = container;
            IsDragDrop = true;
            draggedObject = DraggedElement;
            draggedObject.MouseLeftButtonDown += StartDrag;

        }

        public void DragDropSwitchOff()
        {
            if (dxPlay == null) return;
            if (!IsDragDrop) return;
            Container = null;
            IsDragDrop = false;
            draggedObject.MouseLeftButtonDown -= StartDrag;
            draggedObject = null;
        }

        void StartDrag(object sender, MouseButtonEventArgs e)
        {
            if (dxPlay == null) return;
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
            if (dxPlay == null) return;
            UpdatePosition(e);
        }

        void UpdatePosition(MouseEventArgs e)
        {
            if (dxPlay == null) return;
            var point = e.GetPosition(Container);
            var newPos = point - relativeMousePos;
            draggedObject.Margin = new Thickness(newPos.X, newPos.Y, 0, 0);
        }

        void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (dxPlay == null) return;
            FinishDrag(sender, e);
            Mouse.Capture(null);
        }

        void OnLostCapture(object sender, MouseEventArgs e)
        {
            if (dxPlay == null) return;
            FinishDrag(sender, e);
        }

        void FinishDrag(object sender, MouseEventArgs e)
        {
            if (dxPlay == null) return;
            draggedObject.MouseMove -= OnDragMove;
            draggedObject.LostMouseCapture -= OnLostCapture;
            draggedObject.MouseUp -= OnMouseUp;
            UpdatePosition(e);
        }












        #endregion


        public void OnResize(SizeChangedEventArgs e)
        {
            if (dxPlay == null) return;
            OnResize();
        }
        public void OnResize()
        {
            if (dxPlay == null) return;
            dxPlay.FillWindowPosition(VideoMMM.SelectablePictureBox1);
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) // TODO: какой-то тупой стиль
        {
            if (dxPlay == null) return;
            OnResize(e);
        }

        public Bitmap GetSnapShot()
        {
            return dxPlay.GetSnapShot();
        }

        #region Реализация ЗУМА

        private void VideoPanel_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (dxPlay == null) return;
            OnResize();
        }



        #endregion


    }
}
