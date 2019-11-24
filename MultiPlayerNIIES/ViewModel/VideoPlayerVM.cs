using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Model;
using MultiPlayerNIIES.Tools.Subtitles;
using MultiPlayerNIIES.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MultiPlayerNIIES.ViewModel
{
    public delegate void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);
    public class VideoPlayerVM : INPCBase
    {
        public VideoPlayerView Body; //Ну это не настоящая VM
        Grid Container;
        public VM VM;
        public SyncronizationShiftVM syncronizationShiftVM;
        PlayerPanelVM playerPanelVM;
        public VideoPlayerVM(Grid container, VM vm, Rect AreaForPlacementInContainer)
        {
            Container = container;
            Body = new VideoPlayerView();
            Body.DataContext = this;
            VM = vm;
            container.Children.Add(Body);
            Body.DragDropSwitchOn(Container, Body.Dragger);
            Body.ResizeSwitchOn(Container);
            Body.HorizontalAlignment = HorizontalAlignment.Left;
            Body.VerticalAlignment = VerticalAlignment.Top;
            Replace(AreaForPlacementInContainer);
            Body.UpFocus += UpFocusX;
            Body.OnSyncLeaderSet += Body_OnSyncLeaderSet;
            IsSyncronizeLeader = false;
            SyncronizationShiftVM = new SyncronizationShiftVM(this) { ShiftMaxTime = TimeSpan.FromSeconds(10) };
            Body.subtitleProcessor = new SubtitleProcessor();
            PlayerPanelVM = new PlayerPanelVM(Body, this);

            Settings.SettingsChanged += Settings_SettingsChanged;
            Body.SizeChanged += (s, e) => { UpdateVLCInnerPosition(); };

            Body.VLC.OnVolumeChanged += (d, e) => { OnPropertyChanged("Volume"); };
        }


        private bool outOfSyncronization = false;
        private bool outOfSyncronizationTitles = false;
        public bool OutOfSyncronization
        {
            get { return outOfSyncronization; }
            set { outOfSyncronization = value; OnPropertyChanged("OutOfSyncronization");
            }
        }

        public bool OutOfSyncronizationTitles
        {
            get { return outOfSyncronizationTitles; }
            set
            {
                outOfSyncronizationTitles = value; OnPropertyChanged("OutOfSyncronization");
            }
        }

        public SyncronizationShiftVM SyncronizationShiftVM
        {
            get
            { return syncronizationShiftVM; }
            set
            { syncronizationShiftVM = value; OnPropertyChanged("SyncronizationShiftVM"); }
        }

        public TimeSpan CurShiftTime 
        {
            get
            { return SyncronizationShiftVM.CurrentShiftTime-SyncronizationShiftVM.ShiftTime; }
            set
            { CurShiftTime = value; OnPropertyChanged("CurShiftTime"); }
        }

        public void OnPropertyChangedCurShiftTime() { OnPropertyChanged("CurShiftTime"); }


        public PlayerPanelVM PlayerPanelVM
        {
            get
            { return playerPanelVM; }
            set
            { playerPanelVM = value; OnPropertyChanged("PlayerPanelVM"); }
        }

        public bool IsPlaying
        {
            get { return Body.IsPlaying; }
        }

        public double Rate
        {
            get { return Body.Rate; }
            set { Body.Rate = value; OnPropertyChanged("Rate"); }
        }

        public TimeSpan CurTime
        {
            get { return Body.VLC.CurTimeEx; }
            set { Body.SetPosition(value); OnPropertyChanged("CurTime"); OnPropertyChanged("SliderPosition"); }
        }

        public double SliderPosition
        {
            get { return Body.VLC.Position; }
            set { Body.SetSliderPosition(value); OnPropertyChanged("CurTime"); OnPropertyChanged("SliderPosition"); }
        }

        private double volume;
        public double Volume
        {
            get
            {
                volume = Body.VLC.Volume;
                return volume;
            }
            set
            {
                volume = value;
                if (volume < 0) Body.VLC.Volume = 0;
                else if (volume > 100) Body.VLC.Volume = 100;
                else Body.VLC.Volume = volume;
                OnPropertyChanged("Volume"); OnPropertyChanged("SelfVolume"); Console.WriteLine("V=" + Volume);
            }
        }

        private double shiftVolume = 100;
        public double ShiftVolume
        {
            get { return shiftVolume; }
            set { shiftVolume = value; Volume = (shiftVolume / 100) * selfVolume; OnPropertyChanged("Volume"); OnPropertyChanged("SelfVolume"); OnPropertyChanged("ShiftVolume"); Console.WriteLine("shiftV=" + shiftVolume); }
        }

        private double selfVolume = Settings.DefaultVolume;
        public double SelfVolume
        {
            get { return selfVolume; }
            set { selfVolume = value; Volume = (shiftVolume / 100) * selfVolume; OnPropertyChanged("Volume"); OnPropertyChanged("SelfVolume"); Console.WriteLine("selfV=" + selfVolume); }
        }


        public TimeSpan Duration
        {
            get { return Body.VLC.Duration; }
        }

        public TimeSpan SyncLeaderCurTime
        {
            get { return VM.TimeSyncLead; }
        }

        public SubtitleProcessor GetSubtitleProcessor()
        {
            return Body.subtitleProcessor;
        }

        public string SubtitlesFilename
        {
            get { return System.IO.Path.ChangeExtension(SourceFilename, "srt"); }

        }

        public bool HaveSubtitles
        {
            get { return GetSubtitleProcessor().Ready; }

        }


        public string SourceFilename
        {
            get
            {
                return (new Uri(Body.SourceFilename)).LocalPath;
            }
        }
        public string FilenameForTitle
        {
            get
            {
                if (Settings.ShowFullNameInPlayerHeader)
                    return SourceFilename;
                else
                    return Path.GetFileName(SourceFilename);
            }
        }

        private System.Drawing.Bitmap snapShotTimeDiff1;
        private System.Drawing.Bitmap snapShotTimeDiff2;

        public System.Drawing.Bitmap SnapShotTimeDiff1
        {
            get { return snapShotTimeDiff1; }
            set { snapShotTimeDiff1 = value;  OnPropertyChanged("SnapShotTimeDiff1"); }
        }
        public System.Drawing.Bitmap SnapShotTimeDiff2
        {
            get { return snapShotTimeDiff2; }
            set { snapShotTimeDiff2 = value;  OnPropertyChanged("SnapShotTimeDiff2"); }
        }



        #region СИНХРОНИЗАЦИЯ и все что с ней связано
        private bool isSyncronizeLeader;
        public bool IsSyncronizeLeader
        {
            get { return isSyncronizeLeader; }
            set { isSyncronizeLeader = value; OnPropertyChanged("IsSyncronizeLeader"); }
        }        //данный плеер - ведущий в синхронизации. С ним синхронизируются все остальные
        public event EventHandler OnSyncLeaderSet;
        private void Body_OnSyncLeaderSet(object sender, EventArgs e)
        {
            OnSyncLeaderSet(this, null);
        }

        internal TimeSpan GetSyncTimeFromTitles(TimeSpan SyncTime)
        {
            return Body.subtitleProcessor.GetSubtitle(SyncTime).TimeFromTextBegin;
        }

        internal TimeSpan GetSmartSyncTime(TimeSpan SyncTime, TimeSpan SyncTitlesTime, VideoPlayerVM SyncLeadVideoPlayerVM)
        {
            int t = SearchAndTools.SmartSearchRecord(SyncTime, SyncLeadVideoPlayerVM.GetSubtitleProcessor(), GetSubtitleProcessor());
            TimeSpan SyncTime2 = GetSubtitleProcessor().GetSyncTime(SyncTitlesTime, t);
            return SyncTime2;
        }

        #endregion

        #region Methods


        Rect restoreWindowRect = new Rect(0, 0, 500, 350);
        bool IsAlreadyMax = false;
        bool IsAlreadyMin = false;
        public void SetCurrentSizeForRestore()
        {
            Rect newR = new Rect(Body.Margin.Left, Body.Margin.Top, Body.ActualWidth, Body.ActualHeight);
            if (!IsAlreadyMax && !IsAlreadyMin)
                restoreWindowRect = newR;
        }

        public void OnClose()
        {
            Body.VLC.OnClosing();
        }
        public void LoadFile(string filename)
        {
            if (File.Exists(filename))
            {
                Body.Load(filename);
                OnPropertyChanged("SourceFilename");
            }
            else
            {
                MessageBox.Show("Файл " + filename + " не найден.");
            }
        }

        public event EventHandler UpFocus;
        public void UpFocusX()
        {
            UpFocus(this, null);
        }

        bool focus;
        public bool Focus
        {
            get { return focus; }
            set { focus = value; OnPropertyChanged("Focus"); }
        }

        public int ZIndex
        {
            set
            {
                Canvas.SetZIndex(Body, value);
            }
            get
            {
                return Canvas.GetZIndex(Body);
            }
        }

        public Rect GetArea()
        {
            Rect r = new Rect(Body.Margin.Left, Body.Margin.Top, Body.ActualWidth, Body.ActualHeight);
            return r;
        }

        public void Replace(Rect AreaForPlacementInContainer)
        {
            Body.Margin = new Thickness(AreaForPlacementInContainer.Left, AreaForPlacementInContainer.Top, 0, 0);
            Body.Width = AreaForPlacementInContainer.Width;
            Body.Height = AreaForPlacementInContainer.Height;
        }

        public void SetZoom(Rect ZoomedArea)
        {
            Body.SetZoom(ZoomedArea);
        }

        public Rect GetZoomedArea()
        {
            return Body.GetZoomedArea();
        }

        internal void Play()
        {
            Body.Play();
        }

        internal void Pause()
        {
            Body.Pause();
        }

        private void Settings_SettingsChanged()
        {
            OnPropertyChanged("FilenameForTitle");
        }

        public System.Drawing.Bitmap GetSnapShot()
        {
            return Body.VLC.GetSnapShot();
        }

        #endregion

        #region КОМАНДЫ
        private RelayCommand playCommand;
        public RelayCommand PlayCommand
        {
            get
            {
                return playCommand ??
                  (playCommand = new RelayCommand(obj =>
                  {
                      Body.Play();
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
                      Body.Stop();
                  }));
            }
        }
        private RelayCommand pauseCommand;
        public RelayCommand PauseCommand
        {
            get
            {
                return pauseCommand ??
                  (pauseCommand = new RelayCommand(obj =>
                  {
                      Body.Pause();
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
                      Rate = (double)obj;
                      OnPropertyChanged("Rate");
                  }));
            }
        }

        private RelayCommand readSubtitlesCommand;
        public RelayCommand ReadSubtitlesCommand
        {
            get
            {
                return readSubtitlesCommand ??
                  (readSubtitlesCommand = new RelayCommand(obj =>
                  {
                      GetSubtitleProcessor().LoadSubtitles(SubtitlesFilename);
                  }));
            }
        }


        private RelayCommand stepCommand;
        public RelayCommand StepCommand
        {
            get
            {
                return stepCommand ??
                  (stepCommand = new RelayCommand(obj =>
                  {
                      if (!(obj is TimeSpan)) return;
                      TimeSpan Step = (TimeSpan)obj;
                      Body.Step(Step);
                  }));
            }
        }

        private RelayCommand closeCommand;
        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand ??
                  (closeCommand = new RelayCommand(obj =>
                  {
                      Container.Children.Remove(Body);

                      VM.ClosePlayer(this);
                      OnClose();
                  }));
            }
        }



        private RelayCommand maximizeCommand;
        public RelayCommand MaximizeCommand
        {
            get
            {
                return maximizeCommand ??
                  (maximizeCommand = new RelayCommand(obj =>
                  {
                      SetCurrentSizeForRestore();
                      VM.MaximizePlayer(this);
                      IsAlreadyMax = true;
                      IsAlreadyMin = false;
                  }));
            }
        }

        private RelayCommand minimizeCommand;
        public RelayCommand MinimizeCommand
        {
            get
            {
                return minimizeCommand ??
                  (minimizeCommand = new RelayCommand(obj =>
                  {
                      SetCurrentSizeForRestore();
                      VM.MinimizePlayer(this);
                      IsAlreadyMax = false;
                      IsAlreadyMin = true;
                  }));
            }
        }
        private RelayCommand restoreCommand;
        public RelayCommand RestoreCommand
        {
            get
            {
                return restoreCommand ??
                  (restoreCommand = new RelayCommand(obj =>
                  {
                      IsAlreadyMax = false;
                      IsAlreadyMin = false;
                      Replace(restoreWindowRect);
                  }));
            }
        }


        internal void DefineOldVLCInnerPosition()
        {
            // Body.DefineOldVLCInnerPosition2();
        }

        internal void UpdateVLCInnerPosition()
        {
            //  Body.UpdateVLCInnerPosition2();
        }






        #endregion
    }
}
