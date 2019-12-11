using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.View.TimeStripes;
using MultiPlayerNIIES.ViewModel.TimeDiffVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel.TimeStripeVM
{
    class StripeContainerVM : INPCBase
    {

        public event Action Refreshed;

        #region Поля 
        public VM VM;
        public ObservableCollection<StripeVM> stripeVMs;
        public StripesContainer Body;
        #endregion

        #region Конструкторы
        public StripeContainerVM(VM vm, StripesContainer body)
        {
            VM = vm;
            Body = body;
            Body.DataContext = this;
            VM.videoPlayerVMs.CollectionChanged += VideoPlayerVMs_CollectionChanged;
            stripeVMs = new ObservableCollection<StripeVM>();
            FillStripes();
            VM.PropertyChanged += VideoPlayerVM_PropertyChanged;
        }

        private void VideoPlayerVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SyncLeadPlayer")
            {
                foreach (var v in stripeVMs) v.Refresh();
            }
        }

        private void VideoPlayerVMs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("videoPlayerVMs");
            FillStripes();
        }


        #endregion

        #region Методы
        public void FillStripes()
        {
            stripeVMs.Clear();
            foreach (var v in videoPlayerVMs)
                stripeVMs.Add(new StripeVM(v, this, new Stripe()));
            List<Stripe> stripes = new List<Stripe>();
            foreach (var v in stripeVMs)
            {
                v.Body.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                stripes.Add(v.Body);
                Refreshed += v.Refresh;
            }
            Body.FillStripes(stripes);
        }
        public void Refresh()
        {
            OnPropertyChanged("IsSyncronizeLeader");
            OnPropertyChanged("FilenameForTitle");
            OnPropertyChanged("TimeShift");
            OnPropertyChanged("Margin");
            OnPropertyChanged("SyncLeadBodyWidth");
            if (Refreshed != null) Refreshed();
        }


        #endregion

        #region Свойства

        public ObservableCollection<VideoPlayerVM> videoPlayerVMs
        {
            get { return VM.videoPlayerVMs; }
        }

        public double SyncLeadBodyWidth
        {
            get
            {
                return VM.MainWindow.TimeLine1.ActualWidth;
            }
        }

        public TimeSpan SyncLeadDuration
        {
            get
            {
                if (VM.SyncLeadPlayer == null) return TimeSpan.Zero;
                else return VM.SyncLeadPlayer.Duration;
            }
        }

        #endregion

        #region Комманды


        #endregion

    }
}
