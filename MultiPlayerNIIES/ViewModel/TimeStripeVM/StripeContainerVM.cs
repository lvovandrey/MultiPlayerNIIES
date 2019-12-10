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
            {
                stripeVMs.Add(new StripeVM(v));
            }
            List<Stripe> stripes = new List<Stripe>();
            foreach (var v in stripeVMs)
            {
                Stripe s = new Stripe();
                s.DataContext = v;
                stripes.Add(s);
            }
            Body.FillStripes(stripes);
        }
        

        #endregion

        #region Свойства

        public ObservableCollection<VideoPlayerVM> videoPlayerVMs
        {
            get { return VM.videoPlayerVMs; }
        }
       
        public void Refresh()
        {
            OnPropertyChanged("IsSyncronizeLeader");
            OnPropertyChanged("FilenameForTitle");
            OnPropertyChanged("TimeShift");
            OnPropertyChanged("Margin");
        }


        #endregion

        #region Комманды


        #endregion

    }
}
