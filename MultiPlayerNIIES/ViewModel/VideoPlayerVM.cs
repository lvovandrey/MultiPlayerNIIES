﻿using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Tools.Subtitles;
using MultiPlayerNIIES.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MultiPlayerNIIES.ViewModel
{
    public class VideoPlayerVM : INPCBase
    {
        VideoPlayerView Body; //Ну это не настоящая VM
        Grid Container;
        VM VM;
        SyncronizationShiftVM syncronizationShiftVM;
        PlayerPanelVM playerPanelVM;

        public SyncronizationShiftVM SyncronizationShiftVM
        {
            get
            { return syncronizationShiftVM; }
            set
            { syncronizationShiftVM = value; OnPropertyChanged("SyncronizationShiftVM"); }
        }

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
            set { Body.SetPosition(value); OnPropertyChanged("CurTime"); }
        }

        public SubtitleProcessor GetSubtitleProcessor()
        {
            return Body.subtitleProcessor;
        }

        public string SubtitlesFilename
        {
            get { return System.IO.Path.ChangeExtension(SourceFilename, "srt"); }
                
        }
        public string SourceFilename
        {
            get
            { return (new Uri(Body.SourceFilename)).LocalPath; }
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
            SyncronizationShiftVM = new SyncronizationShiftVM() { ShiftMaxTime=TimeSpan.FromSeconds(10) };
            Body.subtitleProcessor = new SubtitleProcessor();
            PlayerPanelVM = new PlayerPanelVM(Body);
        }




        #region Methods
        public void LoadFile(string filename)
        {
            if (File.Exists(filename))
            {
                Body.Load(filename);
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

        internal void Play()
        {
            Body.Play();
        }

        internal void Pause()
        {
            Body.Pause();
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
                return stopCommand ??
                  (stopCommand = new RelayCommand(obj =>
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

        internal void DefineOldVLCInnerPosition()
        {
            Body.DefineOldVLCInnerPosition2();
        }

        internal void UpdateVLCInnerPosition()
        {
            Body.UpdateVLCInnerPosition2();
        }

        internal void LoadWebcam()
        {
            string destination = @"C:\tmp\22.avi";
            //     string source = @"C:\tmp\444.avi";
            string source = @"rtsp://192.168.0.1:5000/qwe";
       //      string source = @"screen://";
            Uri u = new Uri(source);
            var mediaOptions = new[]
            {
                ":sout=file{dst=" + destination + "}",
                ":sout-keep"
            };
            var mediaOptions2 = new[] {

                ":sout=#transcode{}:standard{access=file,mux=ts,dst=" + destination + "}"
                , ":sout-keep"
            };
//            Body.VLC.vlc.LoadMediaWithOptions(u, mediaOptions2);
            Body.VLC.vlc.LoadMedia(u);
            Body.VLC.vlc.AddOption(mediaOptions2);
            Body.VLC.vlc.Play();


            //string lowQualityDestination = @"C:\tmp\17.avi";

            //var lowQualityMediaOptions = new[]
            //{
            //":sout=#transcode{}:std{access=file,mux=ts,dst=" + lowQualityDestination + "}",
            //":sout-all"
            //};
            ////string destination = @"C:\tmp\444.avi";






            //string[] options = new string[3];

            //options[0] = "config=\"~/Application Data/vlc/vlcrc\"";
            //options[1] = ":file-caching=300 :sout-udp-caching=200";
            //options[2] = ":sout =#transcode{vcodec=mp4v,scale=1}:duplicate{dst=display,dst=std{access=file,mux=ts,dst=\"" + lowQualityDestination + "\"}}";



        }






        #endregion
    }
}
