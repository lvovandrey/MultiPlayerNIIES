using MultiPlayerNIIES.Abstract;
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

        public bool IsPlaying
        {
            get { return Body.IsPlaying; }
        }

        public double Rate
        {
            get { return Body.Rate; }
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
            { return (new Uri(Body.SourceFilename)).AbsolutePath; }
        }

        #region СИНХРОНИЗАЦИЯ и все что с ней связано
        private bool syncronizeLeader;
        public bool SyncronizeLeader
        {
            get { return syncronizeLeader; }
            set { syncronizeLeader = value; OnPropertyChanged("SyncronizeLeader"); }
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
            SyncronizeLeader = false;
            Body.subtitleProcessor = new SubtitleProcessor();
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

        private RelayCommand rateIncreaceCommand;
        public RelayCommand RateIncreaceCommand
        {
            get
            {
                return rateIncreaceCommand ??
                  (rateIncreaceCommand = new RelayCommand(obj =>
                  {
                      Body.RateIncreace();
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
                      Body.RateDecreace();
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

        internal void DefineOldVLCInnerPosition()
        {
            Body.DefineOldVLCInnerPosition2();
        }

        internal void UpdateVLCInnerPosition()
        {
            Body.UpdateVLCInnerPosition2();
        }





        #endregion
    }
}
