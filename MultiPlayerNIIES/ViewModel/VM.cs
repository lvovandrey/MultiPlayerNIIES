using Meta.Vlc.Wpf;
using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Tools;
using MultiPlayerNIIES.Tools.Subtitles;
using MultiPlayerNIIES.View;
using MultiPlayerNIIES.View.Elements;
using MultiPlayerNIIES.View.TimeLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Excel = Microsoft.Office.Interop.Excel;

namespace MultiPlayerNIIES.ViewModel
{
    public class VM : INPCBase
    {
        #region Поля
        public List<VideoPlayerVM> videoPlayerVMs;
        Grid AreaVideoPlayersGrid;
        public MainWindow MainWindow;
        VideoPlayerVM focusedPlayer;
        Excel.Application excel;
        Excel.Workbooks ExcelBooks;
        Excel._Workbook ExcelBook;
        HwndSource sourceOfPostMessages;
        WaitProgressBar WaitIndicator;
        SettingsWindowView settingsWindowView;
        SettingsWindowVM settingsWindowVM;


        private System.Windows.Threading.DispatcherTimer MainTimer;
        private System.Windows.Threading.DispatcherTimer ExcelRefreshStateTimer;
        double oldMainWindowWidth, oldMainWindowHeight;//Блядь что за мусор? почему она тут валяется?


        private Queue<TimeSpan> SyncTitlesDeltasBuffer;
        private Queue<TimeSpan> SyncDeltasBuffer;


        #endregion

        #region Конструкторы и вспомогательные методы
        public VM(Grid areaVideoPlayersGrid, MainWindow mainWindow)
        {
            settingsWindowView = new SettingsWindowView();
            settingsWindowView.Visibility = Visibility.Hidden;
            settingsWindowVM = new SettingsWindowVM(settingsWindowView);


            videoPlayerVMs = new List<VideoPlayerVM>();
            AreaVideoPlayersGrid = areaVideoPlayersGrid;
            WaitIndicator = mainWindow.AreaVideoPlayers.WaitProgressBar1;
            MainWindow = mainWindow;
            MainWindow.SizeChanged += MainWindow_SizeChanged;
            oldMainWindowWidth = MainWindow.ActualWidth;
            oldMainWindowHeight = MainWindow.ActualHeight;


            MainTimer = new System.Windows.Threading.DispatcherTimer();
            MainTimer.Tick += new EventHandler(MainTimerTick);
            MainTimer.Interval = TimeSpan.FromSeconds(0.05);
            MainTimer.Start();

            ExcelRefreshStateTimer = new System.Windows.Threading.DispatcherTimer();
            ExcelRefreshStateTimer.Tick += new EventHandler(ExcelRefreshStateTimerTick);
            ExcelRefreshStateTimer.Interval = TimeSpan.FromSeconds(0.5);
            ExcelRefreshStateTimer.Start();


            Step = TimeSpan.FromMilliseconds(100);
            RateShift = 0.1;
            slowRate = 0.5;
            fastRate = 2;

            MainWindow.PreviewKeyDown += MainWindow_PreviewKeyDown;

            MaxSyncDelta = TimeSpan.FromSeconds(0.4);
            SyncDelta = TimeSpan.FromSeconds(0);
            SyncDeltasBuffer = new Queue<TimeSpan>();

            MaxSyncTitlesDelta = TimeSpan.FromSeconds(0.4);
            SyncTitlesDelta = TimeSpan.FromSeconds(0);
            SyncTitlesDeltasBuffer = new Queue<TimeSpan>();
            ToolsTimer.Delay(() =>
            {
                //подключаем обработку внешних сообщений PostMessage
                sourceOfPostMessages = HwndSource.FromHwnd(new WindowInteropHelper(MainWindow).Handle);
                sourceOfPostMessages.AddHook(new HwndSourceHook(PostMessagesRecieve));
            }, TimeSpan.FromSeconds(4));
        }



        private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                PlayPauseCommand.Execute(null); e.Handled = true;
            }
            if (e.Key == Key.Left && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                StepBackwardCommand.Execute(null); e.Handled = true;
            }
            if (e.Key == Key.Right && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                StepForwardCommand.Execute(null); e.Handled = true;
            }
            if (e.Key == Key.Up && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                RateIncreaceCommand.Execute(null); e.Handled = true;
            }
            if (e.Key == Key.Down && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                RateDecreaceCommand.Execute(null); e.Handled = true;
            }
            if (e.Key == Key.Q && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                SetRateCommand.Execute(SlowRate); e.Handled = true;
            }
            if (e.Key == Key.W && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                SetRateCommand.Execute(1.00); e.Handled = true;
            }
            if (e.Key == Key.E && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                SetRateCommand.Execute(FastRate); e.Handled = true;
            }

            if (e.Key == Key.S && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                SyncronizationTitleCommand.Execute(FastRate); e.Handled = true;
            }
            if (e.Key == Key.S && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                SyncronizationShiftCommand.Execute(FastRate); e.Handled = true;
            }

        }

        #endregion

        #region СВОЙСТВА 

        public string ExcelBookFilename
        {
            get
            {
                if (IsExcelConnected && ExcelBook != null) return ExcelBook.FullName;
                else return "";
            }
        }

        public string PlayerState
        {
            get
            {
                string playerState = "No video";
                if (videoPlayerVMs.Count > 0)
                {
                    playerState = "Paused";
                    foreach (VideoPlayerVM v in videoPlayerVMs)
                        if (v.IsPlaying)
                        {
                            playerState = "Play";
                            break;
                        }
                }
                return playerState;
            }
        }

        private bool isOnAutoSyncroinization;
        public bool IsOnAutoSyncronization
        {
            get { return isOnAutoSyncroinization; }
            set
            {
                isOnAutoSyncroinization = value;
                OnPropertyChanged("IsOnAutoSyncronization");
            }
        }

        private TimeSpan maxSyncDelta;
        public TimeSpan MaxSyncDelta
        {
            get { return maxSyncDelta; }
            set
            {
                maxSyncDelta = value;
                OnPropertyChanged("MaxSyncDelta");
                OnPropertyChanged("SyncDeltaPercentage");
            }
        }

        private TimeSpan syncDelta;
        public TimeSpan SyncDelta
        {
            get { return syncDelta; }
            set
            {
                syncDelta = value;
                OnPropertyChanged("SyncDelta");
                OnPropertyChanged("SyncDeltaTotalMiliseconds");
                OnPropertyChanged("SyncDeltaPercentage");

            }
        }

        public double SyncDeltaTotalMiliseconds { get { return SyncDelta.TotalMilliseconds; } }

        public double SyncDeltaPercentage { get { return 100 * (SyncDelta.TotalSeconds / MaxSyncDelta.TotalSeconds); } }

        private int syncErrorsCount = 0;
        public int SyncErrorsCount { get { return syncErrorsCount; } set { syncErrorsCount = value; } }
        public int SyncErrorsMaxCount { get; private set; } = 5;


        private bool isOnAutoSyncroinizationTitles;
        public bool IsOnAutoSyncronizationTitles
        {
            get { return isOnAutoSyncroinizationTitles; }
            set
            {
                isOnAutoSyncroinizationTitles = value;
                OnPropertyChanged("IsOnAutoSyncronizationTitles");
            }
        }

        private bool IsSyncInProcess = false;

        private TimeSpan maxSyncTitlesDelta;
        public TimeSpan MaxSyncTitlesDelta
        {
            get { return maxSyncTitlesDelta; }
            set
            {
                maxSyncTitlesDelta = value;
                OnPropertyChanged("MaxSyncTitlesDelta");
                OnPropertyChanged("SyncTitlesDeltaPercentage");
            }
        }

        private TimeSpan syncTitlesDelta;
        public TimeSpan SyncTitlesDelta
        {
            get { return syncTitlesDelta; }
            set
            {
                syncTitlesDelta = value;
                OnPropertyChanged("SyncTitlesDelta");
                OnPropertyChanged("SyncTitlesDeltaTotalMiliseconds");
                OnPropertyChanged("SyncTitlesDeltaPercentage");

            }
        }




        public double SyncTitlesDeltaTotalMiliseconds { get { return SyncTitlesDelta.TotalMilliseconds; } }

        public double SyncTitlesDeltaPercentage { get { return 100 * (SyncTitlesDelta.TotalSeconds / MaxSyncTitlesDelta.TotalSeconds); } }



        public VideoPlayerVM FocusedPlayer
        {
            get { return focusedPlayer; }
            set
            {
                focusedPlayer = value;
                OnPropertyChanged("FocusedPlayer");
                OnPropertyChanged("Rate");
            }
        }
        public VideoPlayerVM SyncLeadPlayer
        {
            get
            {
                VideoPlayerVM SyncLead = null;
                var SyncLeads = videoPlayerVMs.Where(v => v.IsSyncronizeLeader == true);
                if (videoPlayerVMs.Count > 0)
                    if (SyncLeads.Count() > 0)
                        SyncLead = SyncLeads.First();
                    else
                    {
                        SyncLeadPlayer = videoPlayerVMs.First();
                        SyncLead = videoPlayerVMs.First();
                    }
                return SyncLead;
            }
            set
            {
                if (value == null) return;

                foreach (VideoPlayerVM v in videoPlayerVMs)
                    v.IsSyncronizeLeader = false;
                value.IsSyncronizeLeader = true;
                OnPropertyChanged("SyncLeadPlayer");
                RefreshTimeLineView();
            }
        }

        public TimeSpan CurTime
        {
            get
            {
                if (SyncLeadPlayer != null)
                    return SyncLeadPlayer.CurTime;
                else return TimeSpan.Zero;
            }
            set
            {
                if (SyncLeadPlayer != null)
                {
                    SyncLeadPlayer.CurTime = value;
                    OnPropertyChanged("CurTime");
                }
            }
        }
        public double SyncLeadSliderPosition
        {
            get
            {

                if (SyncLeadPlayer != null)
                {
                  //  Debug.WriteLine("SyncLeadPlayer.SliderPosition PROP=" + SyncLeadPlayer.SliderPosition.ToString());
                    return SyncLeadPlayer.SliderPosition;
                }
                else return 0;
            }
            set
            {
                if (SyncLeadPlayer != null)
                {
                    SyncLeadPlayer.SliderPosition = value;
                    OnPropertyChanged("SyncLeadSliderPosition");
                }
            }
        }

        public TimeSpan SyncLeadSliderDuration
        {
            get
            {
                if (SyncLeadPlayer != null)
                    return SyncLeadPlayer.Duration;
                else return TimeSpan.Zero;
            }
        }




        public double Rate
        {
            get
            {
                if (FocusedPlayer != null) return FocusedPlayer.Rate; else return 0;
            }
        }
        private double rateShift;
        public double RateShift
        {
            get { return rateShift; }
            set
            {
                if (value < 0) throw new Exception("Изменение скорости воспроизведения должно указываться как положительная величина");
                rateShift = value;
                OnPropertyChanged("RateShift");
            }
        }

        private double slowRate;
        public double SlowRate
        {
            get { return slowRate; }
            set
            {
                if (value >= 1 || value <= 0) throw new Exception("Медленная скорость должна быть от 0 до 1");
                slowRate = value;
                OnPropertyChanged("SlowRate");
            }
        }

        private double fastRate;
        public double FastRate
        {
            get { return fastRate; }
            set
            {
                if (value <= 1) throw new Exception("Быстрая скорость должна быть более 1");
                fastRate = value;
                OnPropertyChanged("FastRate");
            }
        }

        public TimeSpan TimeSyncLead
        {
            get { if (SyncLeadPlayer != null) return SyncLeadPlayer.CurTime; else return TimeSpan.Zero; }
        }



        private TimeSpan step;
        public TimeSpan Step
        {
            get
            {
                return step;
            }
            set
            {
                step = value;
                OnPropertyChanged("Step");
            }
        }

        private bool isExcelConnected;
        public bool IsExcelConnected
        {
            get
            {
                return isExcelConnected;
            }
            set
            {
                isExcelConnected = value;
                OnPropertyChanged("IsExcelConnected");
            }
        }
        #endregion

        #region Methods

        private void RefreshTimeLineView()
        {
            if (SyncLeadPlayer != null)
            {
                MainWindow.TimeLine1.FullTime = SyncLeadPlayer.Duration;
                SyncLeadSliderPosition = SyncLeadPlayer.SliderPosition;
                MainWindow.TimeLine1.POS = SyncLeadPlayer.SliderPosition;
                MainWindow.TimeLine1.Cursor1.CRPosition = SyncLeadPlayer.SliderPosition;
                OnPropertyChanged("SyncLeadSliderDuration");
                OnPropertyChanged("SyncLeadSliderPosition");

            }
            else
            {
                SyncLeadSliderPosition = 0;
                MainWindow.TimeLine1.POS = 0;
                MainWindow.TimeLine1.Cursor1.CRPosition = 0;
                OnPropertyChanged("SyncLeadSliderDuration");
                OnPropertyChanged("SyncLeadSliderPosition");

            }


        }

        //Обработчик сообщений окна Windows (сюда можно принять сообщение от PostMessage)
        private IntPtr PostMessagesRecieve(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //  do stuff
            try
            {
                if (msg == 1025)
                {
                    switch ((int)lParam)
                    {
                        case 34: PlayPauseCommand.Execute(null); break;
                        case 37: RateDecreaceCommand.Execute(null); break;
                        case 38: RateIncreaceCommand.Execute(null); break;
                        case 39: StepBackwardCommand.Execute(null); break;
                        case 40: StepForwardCommand.Execute(null); break;
                        case 41: OpenCommand.Execute(null); break;
                        case 42: SendToExcelTime1Command.Execute(null); break;
                        case 43: SendToExcelTime2Command.Execute(null); break;
                        case 44: StepValueIncreaceCommand.Execute(null); break;
                        case 45: StepValueIncreaceCommand.Execute(null); break;
                        case 46: OnExcelClosing(); break;
                        case 47:
                            {
                                TimeSpan time = TimeSpan.Zero;
                                int milliseconds = wParam.ToInt32();
                                if (milliseconds >= 0)
                                {
                                    time = TimeSpan.FromMilliseconds((double)milliseconds);
                                    foreach (VideoPlayerVM v in videoPlayerVMs) v.PauseCommand.Execute(null);
                                    SyncLeadPlayer.CurTime = time;
                                }
                                break;
                            }
                        default: break;
                    }
                }
                //   Txt.Text += msg.ToString() + "/" + lParam.ToInt32().ToString() + "/" + wParam.ToInt32().ToString() + " - ";
            }
            catch
            {

            }
            //        Thread.Sleep(50);
            return IntPtr.Zero;
        }

        private void OnExcelClosing()
        {
            //Не совсем понял а что тут надо делать
            System.Windows.MessageBox.Show("Закрыт связанный файл Excel");
            IsExcelConnected = false;
        }

        private void MainTimerTick(object sender, EventArgs e)
        {
            OnPropertyChanged("CurTime");
            OnPropertyChanged("SyncLeadSliderPosition");
            if (SyncLeadPlayer != null)
            {
                SyncLeadSliderPosition = SyncLeadPlayer.SliderPosition;
            }
            else
                SyncLeadSliderPosition = 0;
            SyncDelta = CalcSyncDelta();
            SyncTitlesDelta = CalcSyncTitlesDelta();
            if (IsOnAutoSyncronization) AutoSyncronization();
            else if (IsOnAutoSyncronizationTitles) AutoSyncronizationTitles();
        }

        private void ExcelRefreshStateTimerTick(object sender, EventArgs e)
        {
            try
            {
                if (IsExcelConnected)
                {
                    excel.Run((object)"CurModeReceive", (object)PlayerState);
                    excel.Run((object)"CurTimeReceive", (object)TimeSyncLead.ToString(@"hh\:mm\:ss\,ff"));
                    excel.Run((object)"CurTimePitchReceive", (object)Step.ToString(@"s\,ff"));
                    excel.Run((object)"CurSpeedReceive", (object)Rate.ToString("F2").Replace(@".", @","));
                }
            }
            catch
            {

            }
        }

        private void SendTime1ToExcel()
        {
            try
            {
                if (excel != null && IsExcelConnected && ExcelBook != null)
                {
                    excel.Run((object)"WriteTime", (object)(TimeSyncLead.ToString(@"h\:mm\:ss\.fff")));
                }
            }
            catch
            {

            }
        }
        private void SendTime2ToExcel()
        {
            try
            {
                if (excel != null && IsExcelConnected && ExcelBook != null)
                {
                    excel.Run((object)"WriteTime2", (object)(TimeSyncLead.ToString(@"h\:mm\:ss\.fff")));
                }
            }
            catch
            {

            }
        }

        private void AutoSyncronizationTitles()
        {
            if (SyncTitlesDelta > MaxSyncTitlesDelta && !IsSyncInProcess) SyncronizationTitleCommand.Execute(null);
        }

        private void AutoSyncronization()
        {
            if (SyncDelta > MaxSyncDelta && !IsSyncInProcess) SyncronizationShiftCommand.Execute(null);
        }

        private TimeSpan CalcSyncDelta()
        {
            if (videoPlayerVMs.Count < 2) return TimeSpan.Zero;
            List<TimeSpan> deltas = new List<TimeSpan>();
            foreach (VideoPlayerVM v in videoPlayerVMs)
            {
                TimeSpan dt = (TimeSyncLead - v.CurTime + v.SyncronizationShiftVM.ShiftTime);
                if (dt < TimeSpan.Zero) dt = -dt;
                deltas.Add(dt);
            }


            SyncDeltasBuffer.Enqueue(deltas.Max());
            if (SyncDeltasBuffer.Count > 10) SyncDeltasBuffer.Dequeue();

            return SyncDeltasBuffer.Max();
        }

        private TimeSpan CalcSyncTitlesDelta()
        {


            if (videoPlayerVMs.Count < 2) return TimeSpan.Zero;

            List<VideoPlayerVM> videoPlayers = new List<VideoPlayerVM>();
            foreach (VideoPlayerVM v in videoPlayerVMs)
            {
                if (v.HaveSubtitles) videoPlayers.Add(v);
            }
            if (videoPlayers.Count < 2) return TimeSpan.Zero;
            List<TimeSpan> deltas = new List<TimeSpan>();
            TimeSpan TL =  videoPlayers[0].GetSyncTimeFromTitles(TimeSyncLead); 
            foreach (VideoPlayerVM v in videoPlayers)
            {
                TimeSpan t = v.GetSyncTimeFromTitles(v.CurTime);
                TimeSpan dt = (t - TL);
                if (dt < TimeSpan.Zero) dt = -dt;
                deltas.Add(dt);
            }


            SyncTitlesDeltasBuffer.Enqueue(deltas.Max());
            if (SyncTitlesDeltasBuffer.Count > 10) SyncTitlesDeltasBuffer.Dequeue();

            return SyncTitlesDeltasBuffer.Max();
        }

        public void OpenVideos(string[] fileNames)
        {
            Queue<Rect> AreasForPlacement = CalcAreasForPlacementVideoplayers(fileNames.Count() + videoPlayerVMs.Count);
            foreach (var v in videoPlayerVMs)
            {
                v.Replace(AreasForPlacement.Dequeue());
            }

            foreach (String file in fileNames)
            {
                var v = AddVideoPlayer(AreasForPlacement.Dequeue());
                v.LoadFile(file);
                OnPropertyChanged("Rate"); //Ты же знаешь это ужасно! 
            }

            ReadSubitilesCommand.Execute(null);
            RefreshTimeLineView();

        }



        public void OpenVideos(string[] fileNames, Rect[] AreasForPlacement)
        {
            for (int i = 0; i < fileNames.Count(); i++)
            {
                var v = AddVideoPlayer(AreasForPlacement[i]);
                v.LoadFile(fileNames[i]);
                OnPropertyChanged("Rate"); //Ты же знаешь это ужасно! 
            }
            ReadSubitilesCommand.Execute(null);
        }

        private VideoPlayerVM AddVideoPlayer(Rect AreaForPlacement)
        {
            VideoPlayerVM videoPlayerVM = new VideoPlayerVM(AreaVideoPlayersGrid, this, AreaForPlacement);
            videoPlayerVMs.Add(videoPlayerVM);
            videoPlayerVM.UpFocus += UpFocus;
            UpFocus(videoPlayerVM, null);
            videoPlayerVM.OnSyncLeaderSet += VideoPlayerVM_OnSyncLeaderSet;
            if (videoPlayerVMs.Count == 1) VideoPlayerVM_OnSyncLeaderSet(videoPlayerVM, null);
            ToolsTimer.Delay(() =>
            {
                videoPlayerVM.SetCurrentSizeForRestore();
            }, TimeSpan.FromSeconds(3));
            return videoPlayerVM;
        }

        private void VideoPlayerVM_OnSyncLeaderSet(object sender, EventArgs e)
        {
            VideoPlayerVM videoPlayerVM = (VideoPlayerVM)sender;

            SyncLeadPlayer = videoPlayerVM;

        }

        /// <summary>
        /// Вычисляем расположение видеоплееров по их количеству
        /// </summary>
        /// <param name="playersCount"></param>
        /// <returns></returns>
        private Queue<Rect> CalcAreasForPlacementVideoplayers(int playersCount)
        {
            Queue<Rect> rects = new Queue<Rect>();

            double w = AreaVideoPlayersGrid.ActualWidth;
            double h = AreaVideoPlayersGrid.ActualHeight;

            if (playersCount == 1)
            {
                rects.Enqueue(new Rect(0, 0, w, h));
            }
            if (playersCount == 2)
            {
                rects.Enqueue(new Rect(0, 0, w / 2, h));
                rects.Enqueue(new Rect(w / 2, 0, w / 2, h));
            }
            if (playersCount == 3)
            {
                rects.Enqueue(new Rect(0, 0, w / 2, h / 2));
                rects.Enqueue(new Rect(w / 2, 0, w / 2, h / 2));
                rects.Enqueue(new Rect(0, h / 2, w / 2, h / 2));

            }
            if (playersCount == 4)
            {
                rects.Enqueue(new Rect(0, 0, w / 2, h / 2));
                rects.Enqueue(new Rect(w / 2, 0, w / 2, h / 2));
                rects.Enqueue(new Rect(0, h / 2, w / 2, h / 2));
                rects.Enqueue(new Rect(w / 2, h / 2, w / 2, h / 2));
            }
            if (playersCount == 5)
            {
                rects.Enqueue(new Rect(0, 0, w / 3, h / 2));
                rects.Enqueue(new Rect(w / 3, 0, w / 3, h / 2));
                rects.Enqueue(new Rect(2 * w / 3, 0, w / 3, h / 2));

                rects.Enqueue(new Rect(0, h / 2, w / 3, h / 2));
                rects.Enqueue(new Rect(w / 3, h / 2, w / 3, h / 2));
            }
            if (playersCount == 6)
            {
                rects.Enqueue(new Rect(0, 0, w / 3, h / 2));
                rects.Enqueue(new Rect(w / 3, 0, w / 3, h / 2));
                rects.Enqueue(new Rect(2 * w / 3, 0, w / 3, h / 2));

                rects.Enqueue(new Rect(0, h / 2, w / 3, h / 2));
                rects.Enqueue(new Rect(w / 3, h / 2, w / 3, h / 2));
                rects.Enqueue(new Rect(2 * w / 3, h / 2, w / 3, h / 2));
            }
            if (playersCount == 7)
            {
                rects.Enqueue(new Rect(0, 0, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, 0, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, 0, w / 3, h / 3));

                rects.Enqueue(new Rect(0, h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, h / 3, w / 3, h / 3));

                rects.Enqueue(new Rect(0, 2 * h / 3, w / 3, h / 3));

            }
            if (playersCount == 8)
            {
                rects.Enqueue(new Rect(0, 0, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, 0, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, 0, w / 3, h / 3));

                rects.Enqueue(new Rect(0, h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, h / 3, w / 3, h / 3));

                rects.Enqueue(new Rect(0, 2 * h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, 2 * h / 3, w / 3, h / 3));
            }
            if (playersCount == 9)
            {
                rects.Enqueue(new Rect(0, 0, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, 0, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, 0, w / 3, h / 3));

                rects.Enqueue(new Rect(0, h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, h / 3, w / 3, h / 3));

                rects.Enqueue(new Rect(0, 2 * h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(w / 3, 2 * h / 3, w / 3, h / 3));
                rects.Enqueue(new Rect(2 * w / 3, 2 * h / 3, w / 3, h / 3));
            }
            if (playersCount > 9)
            {
                for (int i = 1; i <= playersCount; i++)
                    rects.Enqueue(new Rect(30 * i, 30 * i, 300, 300));
            }

            return rects;
        }

        private void UpFocus(object sender, EventArgs e)
        {
            VideoPlayerVM videoPlayerVM = (VideoPlayerVM)sender;
            //сначала разбираемяся с Z-индексами
            List<int> Zindexes = new List<int>();
            foreach (VideoPlayerVM v in videoPlayerVMs)
                Zindexes.Add(v.ZIndex);
            int Min = Zindexes.Min();
            if (Min > 0)
                foreach (VideoPlayerVM v in videoPlayerVMs)
                    v.ZIndex -= Min;
            Zindexes.Clear();
            foreach (VideoPlayerVM v in videoPlayerVMs)
                Zindexes.Add(v.ZIndex);
            int Max = Zindexes.Max();
            videoPlayerVM.ZIndex = Max + 1;
            //Теперь проставляем фокус
            foreach (VideoPlayerVM v in videoPlayerVMs)
                v.Focus = false;
            videoPlayerVM.Focus = true;

            focusedPlayer = videoPlayerVM;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double ScaleWidth = MainWindow.ActualWidth / oldMainWindowWidth;
            double ScaleHeight = MainWindow.ActualHeight / oldMainWindowHeight;
            AllVideoPlayersResize(ScaleWidth, ScaleHeight);
            oldMainWindowWidth = MainWindow.ActualWidth;
            oldMainWindowHeight = MainWindow.ActualHeight;
        }

        private void AllVideoPlayersResize(double ScaleWidth, double ScaleHeight)
        {
            foreach (VideoPlayerVM v in videoPlayerVMs)
            {
                v.DefineOldVLCInnerPosition();
                Rect oldArea = v.GetArea();
                Rect newArea = new Rect(oldArea.Left * ScaleWidth,
                                        oldArea.Top * ScaleHeight,
                                        oldArea.Width * ScaleWidth,
                                        oldArea.Height * ScaleHeight);
                v.Replace(newArea);
            }
            Thread.Sleep(50);
            foreach (VideoPlayerVM v in videoPlayerVMs)
            {
                v.UpdateVLCInnerPosition();
                ToolsTimer.Delay(() => { v.SetCurrentSizeForRestore(); }, TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// Закрывает указанный плеер
        /// </summary>
        /// <param name="videoPlayerVM"></param>
        internal void ClosePlayer(VideoPlayerVM videoPlayerVM)
        {
            videoPlayerVMs.Remove(videoPlayerVM);
            if (videoPlayerVM.IsSyncronizeLeader && videoPlayerVMs.Count > 1)
                VideoPlayerVM_OnSyncLeaderSet(videoPlayerVMs.Where(v => v.IsSyncronizeLeader != true).First(), null);

            Queue<Rect> AreasForPlacement = CalcAreasForPlacementVideoplayers(videoPlayerVMs.Count);

            foreach (var v in videoPlayerVMs)
                v.Replace(AreasForPlacement.Dequeue());
            videoPlayerVM = null;
            if (videoPlayerVMs.Count > 0) videoPlayerVMs[0].UpFocusX();
        }

        internal void MaximizePlayer(VideoPlayerVM videoPlayerVM)
        {
            Rect r = new Rect(0, 0, AreaVideoPlayersGrid.ActualWidth, AreaVideoPlayersGrid.ActualHeight);
            videoPlayerVM.Replace(r);
        }

        internal void MinimizePlayer(VideoPlayerVM videoPlayerVM)
        {
            Rect r = new Rect(0, 0, 250, 100);
            videoPlayerVM.Replace(r);
        }
        #endregion


        #region КОМАНДЫ

        private RelayCommand saveStateCommand;
        public RelayCommand SaveStateCommand
        {
            get
            {
                return saveStateCommand ??
                  (saveStateCommand = new RelayCommand(obj =>
                  {

                      SaveFileDialog saveFileDialog = new SaveFileDialog
                      {
                          Filter = "Файлы проектов xml (*.xml)|*.xml"
                      };
                      if (saveFileDialog.ShowDialog() != DialogResult.OK) return;
                      if (File.Exists(@saveFileDialog.FileName))
                      {
                         MessageBoxResult res =  System.Windows.MessageBox.Show("Перезаписать файл?", "Файл существует", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                          if (res == MessageBoxResult.No) return;
                      }
                      Tools.Serialization.StateSaver.Save(@saveFileDialog.FileName, this);
                  }));
            }
        }

        private RelayCommand openStateCommand;
        public RelayCommand OpenStateCommand
        {
            get
            {
                return openStateCommand ??
                  (openStateCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog
                      {
                          Multiselect = false,
                          Filter = "Файлы проектов xml (*.xml)|*.xml"
                      };
                      if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                      try
                      {
                          Tools.Serialization.StateSaver.Restore(@openFileDialog.FileName, this);
                      }
                      catch
                      {
                          System.Windows.MessageBox.Show("Ошибка открытия файла проекта");
                      }
                  }));
            }
        }

        private RelayCommand closeAllCommand;
        public RelayCommand СloseAllCommand
        {
            get
            {
                return closeAllCommand ??
                  (closeAllCommand = new RelayCommand(obj =>
                  {
                      foreach (var v in videoPlayerVMs)
                          v.CloseCommand.Execute(null);
                  }));
            }
        }


        private RelayCommand autoSyncTitlesOnOffCommand;
        public RelayCommand AutoSyncTitlesOnOffCommand
        {
            get
            {
                return autoSyncTitlesOnOffCommand ??
                  (autoSyncTitlesOnOffCommand = new RelayCommand(obj =>
                  {
                      IsOnAutoSyncronizationTitles = !IsOnAutoSyncronizationTitles;
                  }));
            }
        }

        private RelayCommand autoSyncOnOffCommand;
        public RelayCommand AutoSyncOnOffCommand
        {
            get
            {
                return autoSyncOnOffCommand ??
                  (autoSyncOnOffCommand = new RelayCommand(obj =>
                  {
                      IsOnAutoSyncronization = !IsOnAutoSyncronization;
                  }));
            }
        }
        private RelayCommand playPauseCommand;
        public RelayCommand PlayPauseCommand
        {
            get
            {
                return playPauseCommand ??
                  (playPauseCommand = new RelayCommand(obj =>
                   {
                       if (FocusedPlayer == null) return;
                       if (FocusedPlayer.IsPlaying)
                       {
                          List<Task> tasks = new List<Task>();

                          foreach (VideoPlayerVM player in videoPlayerVMs)
                          {
                               // player.PauseCommand.Execute(null);
                               tasks.Add(new Task(() => { player.PauseCommand.Execute(null); })); 
                           }

                           ToolsTimer.Delay(() =>
                           {
                               foreach (var t in tasks) t.Start();
                               if (IsOnAutoSyncronization && SyncDeltaPercentage > 60) SyncronizationShiftCommand.Execute(null);
                               if (IsOnAutoSyncronizationTitles && SyncTitlesDeltaPercentage > 60) SyncronizationTitleCommand.Execute(null);
                           }, TimeSpan.FromSeconds(0.1));
                       }
                       else
                       {
                           //foreach (VideoPlayerVM player in videoPlayerVMs)
                           //{
                           //    player.PlayCommand.Execute(null);
                           //}
                           List<Task> tasks = new List<Task>();

                           foreach (VideoPlayerVM player in videoPlayerVMs)
                               tasks.Add(new Task(() => { player.PlayCommand.Execute(null); }));
                       
                           ToolsTimer.Delay(() =>
                           {
                               foreach (var t in tasks) t.Start();
                           }, TimeSpan.FromSeconds(0.1));


                       }

                   }));
            }
        }

        private RelayCommand allPauseCommand;
        public RelayCommand AllPauseCommand
        {
            get
            {
                return allPauseCommand ??
                  (allPauseCommand = new RelayCommand(obj =>
                  {
                          List<Task> tasks = new List<Task>();
                          foreach (VideoPlayerVM player in videoPlayerVMs)
                              tasks.Add(new Task(() => { player.Body.VLC.pause(); })); //TODO:НАРУШЕН ПРИНЦИП ИНКАПСУЛЯЦИИ
                          ToolsTimer.Delay(() =>
                          {
                              foreach (var t in tasks) t.Start();
                          }, TimeSpan.FromSeconds(0.1));

                  }));
            }
        }

        private RelayCommand allPlayCommand;
        public RelayCommand AllPlayCommand
        {
            get
            {
                return allPlayCommand ??
                  (allPlayCommand = new RelayCommand(obj =>
                  {
                      List<Task> tasks = new List<Task>();
                      foreach (VideoPlayerVM player in videoPlayerVMs)
                          tasks.Add(new Task(() => { player.PlayCommand.Execute(null); }));
                      ToolsTimer.Delay(() =>
                      {
                          foreach (var t in tasks) t.Start();
                      }, TimeSpan.FromSeconds(0.1));
                  }));
            }
        }


        private RelayCommand stopCommand;
        public RelayCommand StopCommand
        {
            get
            {
                return stopCommand ??
                  (stopCommand = new RelayCommand(obj =>
                  {
                      foreach (VideoPlayerVM player in videoPlayerVMs)
                      {
                          player.StopCommand.Execute(null);
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog { Multiselect = true };
                      if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                      OpenVideos(openFileDialog.FileNames);
                  }));
            }
        }



        private RelayCommand rateIncreaceCommand;
        public RelayCommand RateIncreaceCommand
        {
            get
            {
                return rateIncreaceCommand ??
                  (rateIncreaceCommand = new RelayCommand(obj =>
                  {
                       SetRateCommand.Execute(Rate + RateShift);
                  }));
            }
        }

        private RelayCommand rateDecreaceCommand;
        public RelayCommand RateDecreaceCommand
        {
            get
            {
                return rateDecreaceCommand ??
                  (rateDecreaceCommand = new RelayCommand(obj =>
                  {
                      SetRateCommand.Execute(Rate - RateShift);
                  }));
            }
        }


        private RelayCommand setRateCommand;
        public RelayCommand SetRateCommand
        {
            get
            {
                return setRateCommand ??
                  (setRateCommand = new RelayCommand(obj =>
                  {
                      if (!(obj is double)) return;
                      Dictionary<VideoPlayerVM, bool> WasPlay = new Dictionary<VideoPlayerVM, bool>();
                      foreach (VideoPlayerVM player in videoPlayerVMs)
                      {
                          WasPlay.Add(player, player.IsPlaying);
                          player.Body.VLC.pause();// 
                      }
                      ToolsTimer.Delay(() =>
                      {
                          foreach (VideoPlayerVM player in videoPlayerVMs)
                              player.SetRateCommand.Execute((double)obj);
                          ToolsTimer.Delay(() =>
                          {
                              foreach (var PlayingStates in WasPlay)
                                  if (PlayingStates.Value) PlayingStates.Key.PlayCommand.Execute(null);

                              OnPropertyChanged("Rate");
                          }, TimeSpan.FromSeconds(0.1));
                      }, TimeSpan.FromSeconds(0.1));

                     
                  }));
            }
        }

        private RelayCommand stepBackwardCommand;
        public RelayCommand StepBackwardCommand
        {
            get
            {
                return stepBackwardCommand ??
                  (stepBackwardCommand = new RelayCommand(obj =>
                  {
                      Dictionary<VideoPlayerVM, bool> PlayersStates = new Dictionary<VideoPlayerVM, bool>();
                      foreach (VideoPlayerVM v in videoPlayerVMs)
                      {
                          PlayersStates.Add(v, v.IsPlaying);
                          v.Pause();
                      }
                      ToolsTimer.Delay(() =>
                      {
                          foreach (VideoPlayerVM player in videoPlayerVMs)
                              player.StepCommand.Execute(-Step);
                      }, TimeSpan.FromSeconds(0.05));
                  }));
            }
        }

        private RelayCommand stepForwardCommand;
        public RelayCommand StepForwardCommand
        {
            get
            {
                return stepForwardCommand ??
                  (stepForwardCommand = new RelayCommand(obj =>
                  {
                      foreach (VideoPlayerVM player in videoPlayerVMs)
                          player.StepCommand.Execute(Step);
                  }));
            }
        }

        private RelayCommand stepValueIncreaceCommand;
        public RelayCommand StepValueIncreaceCommand
        {
            get
            {
                return stepValueIncreaceCommand ??
                  (stepValueIncreaceCommand = new RelayCommand(obj =>
                  {
                      Step += TimeSpan.FromMilliseconds(10);
                  }));
            }
        }

        private RelayCommand stepValueDecreaceCommand;
        public RelayCommand StepValueDecreaceCommand
        {
            get
            {
                return stepValueDecreaceCommand ??
                  (stepValueDecreaceCommand = new RelayCommand(obj =>
                  {
                      Step -= TimeSpan.FromMilliseconds(10);
                  }));
            }
        }

        private RelayCommand excelOpenCommand;
        public RelayCommand ExcelOpenCommand
        {
            get
            {
                return excelOpenCommand ??
                  (excelOpenCommand = new RelayCommand(obj =>
                  {
                      OpenFileDialog openFileDialog = new OpenFileDialog
                      {
                          Multiselect = false,
                          Filter = "Файлы Excel c поддержкой макросов (*.xlsm)|*.xlsm"
                      };
                      if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                      try
                      {
                          excel = new Excel.Application();
                          excel.Visible = true;
                          ExcelBooks = excel.Workbooks;
                          ExcelBook = null;
                          ExcelBook = ExcelBooks.Open(@openFileDialog.FileName);
                          IsExcelConnected = true;
                      }
                      catch (Exception e)
                      {
                          System.Windows.MessageBox.Show("Ошибка при открытии Excel-файла: " + e.Message);
                      }

                  }));
            }
        }

        private RelayCommand sendToExcelTime1Command;
        public RelayCommand SendToExcelTime1Command
        {
            get
            {
                return sendToExcelTime1Command ??
                  (sendToExcelTime1Command = new RelayCommand(obj =>
                  {
                      SendTime1ToExcel();
                  }));
            }
        }

        private RelayCommand sendToExcelTime2Command;
        public RelayCommand SendToExcelTime2Command
        {
            get
            {
                return sendToExcelTime2Command ??
                  (sendToExcelTime2Command = new RelayCommand(obj =>
                  {
                      SendTime2ToExcel();
                  }));
            }
        }

        private RelayCommand settingsOpenCommand;
        public RelayCommand SettingsOpenCommand
        {
            get
            {
                return settingsOpenCommand ??
                  (settingsOpenCommand = new RelayCommand(obj =>
                  {
                      settingsWindowView.Show();
                  }));
            }
        }

        private RelayCommand copyTimeToClipBoardCommand;
        public RelayCommand CopyTimeToClipBoardCommand
        {
            get
            {
                return copyTimeToClipBoardCommand ??
                  (copyTimeToClipBoardCommand = new RelayCommand(obj =>
                  {
                      System.Windows.Forms.Clipboard.SetText((string)obj);
                  }));
            }
        }


        private RelayCommand readSubitilesCommand;
        public RelayCommand ReadSubitilesCommand
        {
            get
            {
                return readSubitilesCommand ??
                  (readSubitilesCommand = new RelayCommand(obj =>
                  {
                      foreach (VideoPlayerVM v in videoPlayerVMs)
                      {
                          v.ReadSubtitlesCommand.Execute(null);
                      }
                  }));
            }
        }


        private RelayCommand syncronizationTitleCommand;
        public RelayCommand SyncronizationTitleCommand
        {
            get
            {
                return syncronizationTitleCommand ??
                  (syncronizationTitleCommand = new RelayCommand(obj =>
                  {
                      IsSyncInProcess = true;
                      ToolsTimer.Delay(() => { IsSyncInProcess = false; }, TimeSpan.FromSeconds(3));//TODO: Очень бы тут пригодилась многопоточность

                      WaitIndicator.ShowMe("Синхронизация по титрам", TimeSpan.FromSeconds(1));
                      if (videoPlayerVMs.Count < 2) return;
                      foreach (VideoPlayerVM v in videoPlayerVMs)
                      {
                          v.PauseCommand.Execute(null);
                      }

                      try
                      {
                          TimeSpan SyncTitlesTime = SyncLeadPlayer.GetSyncTimeFromTitles(TimeSyncLead);

                          Dictionary<VideoPlayerVM, TimeSpan> SyncDictionary = new Dictionary<VideoPlayerVM, TimeSpan>();
                          foreach (VideoPlayerVM v in videoPlayerVMs)
                          {

                              if (v.HaveSubtitles)
                              {
                                  SyncDictionary.Add(v, v.GetSmartSyncTime(TimeSyncLead, SyncTitlesTime, SyncLeadPlayer));
                              }
                          }

                          foreach (KeyValuePair<VideoPlayerVM, TimeSpan> v in SyncDictionary)
                              v.Key.CurTime = v.Value;
                          SyncErrorsCount = 0;
                      }
                      catch (SyncException e)
                      {
                          
                          IsOnAutoSyncronizationTitles = false;
                          IsSyncInProcess = false;
                          if (SyncErrorsCount < SyncErrorsMaxCount)
                          {
                              StepForwardCommand.Execute(null);
                              ToolsTimer.Delay(() => { SyncronizationTitleCommand.Execute(null); }, TimeSpan.FromSeconds(1));
                              SyncErrorsCount++;
                          }
                          else
                          {
                              System.Windows.MessageBox.Show(e.Message);
                              SyncErrorsCount = 0;
                          }
                      }
                  }));
            }
        }

        private RelayCommand syncronizationShiftCommand;
        public RelayCommand SyncronizationShiftCommand
        {
            get
            {
                return syncronizationShiftCommand ??
                  (syncronizationShiftCommand = new RelayCommand(obj =>
                  {
                      IsSyncInProcess = true;
                      ToolsTimer.Delay(() => { IsSyncInProcess = false; }, TimeSpan.FromSeconds(2.2));
                      WaitIndicator.ShowMe("Синхронизация по смещению", TimeSpan.FromSeconds(1));
                      if (videoPlayerVMs.Count < 2) return;



                      Dictionary<VideoPlayerVM, bool> PlayersStates = new Dictionary<VideoPlayerVM, bool>();
                      foreach (VideoPlayerVM v in videoPlayerVMs)
                      {
                          PlayersStates.Add(v, v.IsPlaying);
                      }

                      foreach (VideoPlayerVM v in videoPlayerVMs)
                      {
                          v.PauseCommand.Execute(null);
                      }

                      foreach (VideoPlayerVM v in videoPlayerVMs)
                          if (!v.IsSyncronizeLeader) v.CurTime = v.SyncronizationShiftVM.ShiftTime + TimeSyncLead;
                          else v.CurTime = TimeSyncLead;



                      System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
                      Timer.Tick += (s, _) =>
                      {
                          bool IsAllSyncronized = true;
                          foreach (VideoPlayerVM v in videoPlayerVMs)
                              if (!v.IsSyncronizeLeader)
                                  if (v.CurTime != v.SyncronizationShiftVM.ShiftTime + TimeSyncLead) { IsAllSyncronized = false; break; }

                          if (IsAllSyncronized)
                          {
                              Timer.Stop();
                              if(IsAllPlayerStatesEquals(PlayersStates))
                                ToolsTimer.Delay(() =>
                                {
                                    foreach (KeyValuePair<VideoPlayerVM, bool> pair in PlayersStates)
                                        if (pair.Value) pair.Key.Play();
                                }, TimeSpan.FromSeconds(1));
                          }
                      };
                      Timer.Interval = TimeSpan.FromSeconds(0.1);
                      Timer.Start();


                  }));

            }
        }

        private bool IsAllPlayerStatesEquals(Dictionary<VideoPlayerVM, bool> PlayersStates)
        {
            bool flag = true;
            bool firstval = PlayersStates.First().Value;
            foreach (KeyValuePair<VideoPlayerVM, bool> pair in PlayersStates)
                if (firstval != pair.Value) flag = false;
            return flag;
        }



        private RelayCommand setCurrencyShiftsOfSyncronizationCommand;
        public RelayCommand SetCurrencyShiftsOfSyncronizationCommand
        {
            get
            {
                return setCurrencyShiftsOfSyncronizationCommand ??
                  (setCurrencyShiftsOfSyncronizationCommand = new RelayCommand(obj =>
                  {
                      if (System.Windows.MessageBox.Show("Все отрегулированные Вами ранее смещения синхронизации будут заменены на текущие смещения. Продолжить?", "Замена смещений на текущие", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel) return;
                      foreach (VideoPlayerVM v in videoPlayerVMs)
                          if (!v.IsSyncronizeLeader)
                              v.SyncronizationShiftVM.ShiftTime = v.CurTime - TimeSyncLead;

                  }));
            }
        }

        private RelayCommand closeAppCommand;


        public RelayCommand CloseAppCommand
        {
            get
            {
                return closeAppCommand ??
                  (closeAppCommand = new RelayCommand(obj =>
                  {
                      foreach (var v in videoPlayerVMs)
                          v.OnClose();
                     ApiManager.ReleaseAll(); 

                  }));
            }
        }


        #endregion
    }
}
