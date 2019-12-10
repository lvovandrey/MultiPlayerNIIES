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
                //var SyncLeadStripes = (from v in stripeVMs
                //                       where v.IsSyncronizeLeader
                //                       select v.Body).ToList();

                //if (SyncLeadStripes.Count() > 0 && SyncLeadStripes.First().MainGrid.ActualWidth > 100)
                    return Body.ActualWidth-400;
           //     else return 100;
            }
        }


        #endregion

        #region Комманды


        #endregion

    }
}
