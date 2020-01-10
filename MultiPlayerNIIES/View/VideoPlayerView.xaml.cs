using MultiPlayerNIIES.Model;
using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.Tools.Subtitles;
using MultiPlayerNIIES.ViewModel;
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
           // ToolsTimer.Delay(() => { ButtonHideInstruments_Click(null, null);  }, TimeSpan.FromSeconds(1));
        }



        private void TimerTick()
        {
            if (subtitleProcessor != null && subtitleProcessor.Ready)
            {
                TextBlockSubtitles.Text = subtitleProcessor.GetSubtitle(VLC.CurTime).Text;
            }
            // ((dynamic)DataContext).OnPropertyChanged("SourceFilename");

        }


        public void Load(string filepath)
        {
            VLC.Source = new Uri(@filepath);
        }



        public void SetPosition(TimeSpan position)
        {
            if (VLC.Duration <= TimeSpan.Zero) Tools.ToolsTimer.Delay(() =>
            {
                VLC.pause();
                double pos = 1000 * position.TotalSeconds / VLC.Duration.TotalSeconds;

                Tools.ToolsTimer.Delay(() => { VLC.Position = pos; }, TimeSpan.FromSeconds(2));
            }, TimeSpan.FromSeconds(1));
            else
            {
                double pos = 1000 * position.TotalSeconds / VLC.Duration.TotalSeconds;
                VLC.Position = pos;
            }
        }
        public void SetSliderPosition(double sl_position)
        {
            if (VLC.Duration <= TimeSpan.Zero)
                Tools.ToolsTimer.Delay(() =>
                {
                    VLC.pause();
                    Tools.ToolsTimer.Delay(() =>
                        {
                            VLC.Position = sl_position;
                        }, TimeSpan.FromSeconds(0.2));
                }, TimeSpan.FromSeconds(0.1));
            else
            {
                VLC.Position = sl_position;
            }
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

        public double Rate { get { return VLC.Rate; } set { VLC.Rate = value; } }

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
            ((VideoPlayerVM)this.DataContext).VideoPlayerWindowState = VideoPlayerWindowState.Restored;

            if ((ResizeContainer == null) || !IsResize) return;
            ResizedObject = (FrameworkElement)this;
            Resizer = (UIElement)sender;
            ResizeRelativeMousePos = e.GetPosition(ResizedObject) - new Point();
            ResizeGlobalRelativeMousePos = e.GetPosition(Container) - new Point();
            oldWidth = ResizedObject.ActualWidth;
            oldHeight = ResizedObject.ActualHeight;
            oldTop = ResizedObject.Margin.Top;
            oldLeft = ResizedObject.Margin.Left;
            DefineOldVLCInnerPosition();

            ResizedObject.MouseMove += OnResizeMove;
            ResizedObject.LostMouseCapture += OnLostCaptureResize;
            ResizedObject.MouseUp += OnMouseUpResize;
            Mouse.Capture(ResizedObject);


        }



        void OnResizeMove(object sender, MouseEventArgs e)
        {
            UpdateResizePosition(e);
            //      UpdateVLCInnerPosition();
        }

        internal void Step(TimeSpan step)
        {
            VLC.Step(step);
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

        internal void SetZoom(Rect zoomedArea)
        {
            VLC.SetSoom(zoomedArea);
        }

        public Rect GetZoomedArea()
        {
           return VLC.GetZoomedArea();
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

        #region РЕСАЙЗ ВНУТРЕННЕГО ПРОИГРЫВАТЕЛЯ
        Thickness OldVLCMargin;
        Size OldVLCSize;

        public void DefineOldVLCInnerPosition()
        {
            OldVLCMargin = VLC.vlc.Margin;
            OldVLCSize = new Size(VLC.vlc.ActualWidth, VLC.vlc.ActualHeight);
        }


        Size OldContainerSize;

        public void DefineOldVLCInnerPosition2()
        {
            OldVLCMargin = VLC.vlc.Margin;
            OldVLCSize = new Size(VLC.vlc.ActualWidth, VLC.vlc.ActualHeight);
            OldContainerSize = new Size(this.Width, this.Height);
        }
        public void UpdateVLCInnerPosition2()
        {
            double kh, kw; //коэффициенты изменения размера по высоте и ширине
            double newWidth = this.Width;
            double newHeight = this.Height;
            kw = newWidth / OldContainerSize.Width;
            kh = newHeight / OldContainerSize.Height;
            Thickness newVLCMargin = new Thickness(OldVLCMargin.Left * kw, OldVLCMargin.Top * kh, OldVLCMargin.Right * kw, OldVLCMargin.Bottom * kh);
            Size newVLCSize = new Size(OldVLCSize.Width * kw, OldVLCSize.Height * kh);

            VLC.vlc.Margin = newVLCMargin;
            VLC.vlc.Height = newVLCSize.Height;
            VLC.vlc.Width = newVLCSize.Width;
        }

        public void UpdateVLCInnerPosition()
        {
            double kh, kw; //коэффициенты изменения размера по высоте и ширине
            double newWidth = this.Width;
            double newHeight = this.Height;
            kw = newWidth / oldWidth;
            kh = newHeight / oldHeight;
            Thickness newVLCMargin = new Thickness(OldVLCMargin.Left * kw, OldVLCMargin.Top * kh, OldVLCMargin.Right * kw, OldVLCMargin.Bottom * kh);
            Size newVLCSize = new Size(OldVLCSize.Width * kw, OldVLCSize.Height * kh);

            VLC.vlc.Margin = newVLCMargin;
            VLC.vlc.Height = newVLCSize.Height;
            VLC.vlc.Width = newVLCSize.Width;

            TextBlockDebug.Text = OldVLCSize + "Inner " + (int)VLC.vlc.Width + "х" + (int)VLC.vlc.Height + " Container" + (int)ActualWidth + "x" + (int)ActualHeight +
                " Container" + (int)(ActualWidth - VLC.vlc.Width) + "x" + (int)(ActualHeight - VLC.vlc.Height);

            VLC.OnResize();
        }
        #endregion

        #region Реализация Focused
        public event Action UpFocus;

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
            var res = MessageBox.Show("Вы уверены что хотите сменить плеер-лидер синхронизации?", "Смена плеера-лидера синхронизации", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (res == MessageBoxResult.OK)
                OnSyncLeaderSet(this, null);
        }
        #endregion


        #region Реализация ЗУМА
        public double OldZoomKoef = 1;
        public double CurZoomKoef = 1;

        private void VLC_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            double k = e.Delta > 0 ? 0.1 : -0.1;
            Zoom(k, VLC.vlc, e.GetPosition(VLC.MainGrid));
        }

        public void Zoom(double ZoomKoef, FrameworkElement ZoomedElement, Point ZoomCenterPositionInContainer)
        {

            double w = ZoomedElement.ActualWidth;
            double h = ZoomedElement.ActualHeight;

            double deltaX = ZoomKoef * w;
            double deltaY = ZoomKoef * h;

            double curX = ZoomCenterPositionInContainer.X;
            double curY = ZoomCenterPositionInContainer.Y;

            double ML = -ZoomedElement.Margin.Left;
            double MT = -ZoomedElement.Margin.Top;

            double wnew = w + deltaX;
            double hnew = h + deltaY;

            double a = ML + curX;
            double b = w - ML - curX;
            double tau = a / b;
            double MLnew = -(tau / (1 + tau)) * wnew + curX;

            double c = MT + curY;
            double d = h - MT - curY;
            double kappa = c / d;
            double MTnew = -(kappa / (1 + kappa)) * hnew + curY;


            if (MLnew > 0) MLnew = 0;
            if (wnew < VLC.MainGrid.ActualWidth) wnew = VLC.MainGrid.ActualWidth;
            if (MTnew > 0) MTnew = 0;
            if (hnew < VLC.MainGrid.ActualHeight) hnew = VLC.MainGrid.ActualHeight;

            ZoomedElement.Width = wnew;
            ZoomedElement.Height = hnew;
            ZoomedElement.Margin = new Thickness(MLnew, MTnew, ZoomedElement.Margin.Right, ZoomedElement.Margin.Bottom);
        }

        #endregion

        enum PanelsShowed
        {
            None,
            PlayOnly,
            SyncAndPlay,
            SyncOnly
        }

        PanelsShowed panelsShowed = PanelsShowed.PlayOnly;
        //скрываем панель инструментов синхронизации
        private void ButtonHideInstruments_Click(object sender, RoutedEventArgs e)
        {
            if (panelsShowed == PanelsShowed.SyncOnly) panelsShowed = PanelsShowed.None;
            else panelsShowed++;

            if (panelsShowed == PanelsShowed.PlayOnly || panelsShowed == PanelsShowed.SyncAndPlay) PlayPanelShowHide(true);
            else PlayPanelShowHide(false);

            if (panelsShowed == PanelsShowed.SyncOnly || panelsShowed == PanelsShowed.SyncAndPlay) SyncPanelShowHide(true);
            else SyncPanelShowHide(false);

        }

        private void SyncPanelShowHide(bool SetShow)
        {
            if (SetShow)
            {
                SyncronizationInstrumentsRow.Height = new GridLength(0);
                SyncronizationShiftViewer.Opacity = 0;
                //SyncInfoPanelRow.Height = new GridLength(0);
                //SyncInfoPanelView.Opacity = 0;

            }
            else
            {
                SyncronizationInstrumentsRow.Height = new GridLength(30);
                SyncronizationShiftViewer.Opacity = 1;
                //SyncInfoPanelRow.Height = new GridLength(30);
                //SyncInfoPanelView.Opacity = 1;

            }
        }
        
        private void PlayPanelShowHide(bool SetShow)
        {
            if (SetShow) { PlayerPanelRow.Height = new GridLength(0); PlayerPanelViewer.Opacity = 0; }
            else { PlayerPanelRow.Height = new GridLength(20); PlayerPanelViewer.Opacity = 1; }
        }


        internal System.Drawing.Bitmap GetSnapShot()
        {
            return VLC.GetSnapShot();
        }

    }
}
