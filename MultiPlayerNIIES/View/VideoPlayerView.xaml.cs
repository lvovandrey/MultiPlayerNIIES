using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.Tools.Subtitles;
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
            if (subtitleProcessor != null && subtitleProcessor.Ready)
            {
                TextBlockSubtitles.Text = subtitleProcessor.GetSubtitle(VLC.CurTime).Text;
            }
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



        public void SetPosition(TimeSpan position)
        {
            VLC.pause();

            double pos = 1000 * position.TotalSeconds / VLC.Duration.TotalSeconds;

            Tools.ToolsTimer.Delay(() => { VLC.Position = pos; }, TimeSpan.FromSeconds(2));
        }

        public SubtitleProcessor subtitleProcessor;


        private void CropBtn_Click(object sender, RoutedEventArgs e)
        {
            //            VLC.vlc.VlcMediaPlayer.CropGeometry = "1000x1000+300+200";
        }

        internal void Pause()
        {
            VLC.pause();
        }
        internal void Play()
        {
            VLC.play();
        }
        internal void Stop()
        {
            VLC.stop();
        }

        private void AspectRatioBtn_Click(object sender, RoutedEventArgs e)
        {
            //            VLC.vlc.VlcMediaPlayer.Video.AspectRatio = "10:10";
        }

        #region Реализация Drag'n'Drop

        public UIElement Container;

        public bool IsDragDrop { get; private set; }

        Vector relativeMousePos;
        FrameworkElement draggedObject;
        UIElement DraggerArea;



        public void DragDropSwitchOn(UIElement container, UIElement dragger)
        {
            if (IsDragDrop) return;
            Container = container;
            DraggerArea = dragger;
            IsDragDrop = true;
            MouseLeftButtonDown += StartDrag;

        }

        public void DragDropSwitchOff(UIElement container)
        {
            if (!IsDragDrop) return;
            Container = null;
            IsDragDrop = false;
            MouseLeftButtonDown -= StartDrag;

        }

        void StartDrag(object sender, MouseButtonEventArgs e)
        {
            if (!DraggerArea.IsMouseOver) return;
            if ((Container == null) || !IsDragDrop) return;
            draggedObject = (FrameworkElement)sender;
            relativeMousePos = e.GetPosition(draggedObject) - new Point();
            draggedObject.MouseMove += OnDragMove;
            draggedObject.LostMouseCapture += OnLostCapture;
            draggedObject.MouseUp += OnMouseUp;
            Mouse.Capture(draggedObject);
        }

        internal void RateIncreace()//TODO: Подключить VM и убрать это отсюда
        {
            VLC.RateIncreace();
        }
        internal void RateDecreace()//TODO: Подключить VM и убрать это отсюда
        {
            VLC.RateDecreace();
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


        #region Реализация Resize
        public UIElement ResizeContainer;

        public bool IsResize { get; private set; }
        public bool IsPlaying { get { return VLC.IsPlaying; } }

        public double Rate { get { return VLC.Rate; } }

        public string SourceFilename { get { return VLC.Source.ToString(); } }

        public string SubtitlesFilename { get { return System.IO.Path.ChangeExtension(SourceFilename, "srt"); } }

        Vector ResizeRelativeMousePos;
        Vector ResizeGlobalRelativeMousePos;
        FrameworkElement ResizedObject;
        UIElement Resizer;
        double oldWidth;
        double oldHeight;
        double oldLeft;
        double oldTop;

        public void ResizeSwitchOn(UIElement container)
        {
            if (IsResize) return;
            ResizeContainer = container;
            IsResize = true;
        }

        public void ResizieSwitchOff(UIElement container)
        {
            if (!IsResize) return;
            ResizeContainer = null;
            IsResize = false;
        }
        private void Sizer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            StartResize(sender, e);
        }

        void StartResize(object sender, MouseButtonEventArgs e)
        {
            if ((ResizeContainer == null) || !IsResize) return;
            ResizedObject = (FrameworkElement)this;
            Resizer = (UIElement)sender;
            ResizeRelativeMousePos = e.GetPosition(ResizedObject) - new Point();
            ResizeGlobalRelativeMousePos = e.GetPosition(Container) - new Point();
            oldWidth = ResizedObject.ActualWidth;
            oldHeight = ResizedObject.ActualHeight;
            oldTop = ResizedObject.Margin.Top;
            oldLeft = ResizedObject.Margin.Left;

            ResizedObject.MouseMove += OnResizeMove;
            ResizedObject.LostMouseCapture += OnLostCaptureResize;
            ResizedObject.MouseUp += OnMouseUpResize;
            Mouse.Capture(ResizedObject);
        }

        void OnResizeMove(object sender, MouseEventArgs e)
        {
            UpdateResizePosition(e);
        }

        void UpdateResizePosition(MouseEventArgs e)
        {
            var point = e.GetPosition(Container);
            var newPos = point - ResizeRelativeMousePos;
            double newWidth = oldWidth;
            double newHeight = oldHeight;
            double newLeft = oldLeft;
            double newTop = oldTop;

            if (Resizer.Equals(this.SizerRightBottom))
            {
                newWidth = oldWidth + newPos.X - ResizedObject.Margin.Left;
                newHeight = oldHeight + newPos.Y - ResizedObject.Margin.Top;
            }

            if (Resizer.Equals(this.SizerRightTop))
            {
                newWidth = oldWidth + newPos.X - ResizedObject.Margin.Left;
                newHeight = oldHeight + ResizeGlobalRelativeMousePos.Y - point.Y;
                newTop = newPos.Y;
            }
            if (Resizer.Equals(this.SizerLeftBottom))
            {
                newWidth = oldWidth + ResizeGlobalRelativeMousePos.X - point.X;
                newLeft = newPos.X;
                newHeight = oldHeight + newPos.Y - ResizedObject.Margin.Top;
            }
            if (Resizer.Equals(this.SizerLeftTop))
            {
                newWidth = oldWidth + ResizeGlobalRelativeMousePos.X - point.X;
                newLeft = newPos.X;
                newHeight = oldHeight + ResizeGlobalRelativeMousePos.Y - point.Y;
                newTop = newPos.Y;
            }
            if (Resizer.Equals(this.SizerRight))
            {
                newWidth = oldWidth + newPos.X - ResizedObject.Margin.Left;
            }
            if (Resizer.Equals(this.SizerLeft))
            {
                newWidth = oldWidth + ResizeGlobalRelativeMousePos.X - point.X;
                newLeft = newPos.X;
            }
            if (Resizer.Equals(this.SizerTop))
            {
                newHeight = oldHeight + ResizeGlobalRelativeMousePos.Y - point.Y;
                newTop = newPos.Y;
            }
            if (Resizer.Equals(this.SizerBottom))
            {
                newHeight = oldHeight + newPos.Y - ResizedObject.Margin.Top;
            }

            ResizedObject.Margin = new Thickness(newLeft, newTop, ResizedObject.Margin.Right, ResizedObject.Margin.Bottom);
            if (newWidth > 50)
                ResizedObject.Width = newWidth;
            if (newHeight > 50)
                ResizedObject.Height = newHeight;
        }

        void OnMouseUpResize(object sender, MouseButtonEventArgs e)
        {
            FinishResize(sender, e);
            Mouse.Capture(null);
        }

        void OnLostCaptureResize(object sender, MouseEventArgs e)
        {
            FinishResize(sender, e);
        }

        void FinishResize(object sender, MouseEventArgs e)
        {
            ResizedObject.MouseMove -= OnResizeMove;
            ResizedObject.LostMouseCapture -= OnLostCaptureResize;
            ResizedObject.MouseUp -= OnMouseUpResize;
            UpdateResizePosition(e);
        }


        #endregion


        #region Реализация Focused
        public event VoidDelegate UpFocus;

        private void UserControl_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            UpFocus();
        }
        #endregion
        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }
        #region СИНХРОНИЗАЦИЯ и все что с ней связано
        public event EventHandler OnSyncLeaderSet;
        private void SyncLeadIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnSyncLeaderSet(this, null);
        }
        #endregion
    }
}
