using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.Model;
using MultiPlayerNIIES.View.TimeStripes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MultiPlayerNIIES.ViewModel.TimeStripeVM
{
    class StripeVM : INPCBase
    {

        #region Поля 
        public VideoPlayerVM VideoPlayerVM;
        public Stripe Body;
        public StripeContainerVM StripeContainerVM;
        #endregion

        #region Конструкторы
        public StripeVM(VideoPlayerVM videoPlayerVM, StripeContainerVM stripeContainerVM, Stripe body)
        {
            VideoPlayerVM = videoPlayerVM;
            Body = body;
            StripeContainerVM = stripeContainerVM;
            Body.DataContext = this;
            VideoPlayerVM.SyncronizationShiftVM.PropertyChanged += SyncronizationShiftVM_PropertyChanged;
        }


        private void SyncronizationShiftVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShiftTime")
            {
                Refresh();
            }
        }
        #endregion

        #region Методы



        #endregion

        #region Свойства

        public bool IsSyncronizeLeader
        {
            get { return VideoPlayerVM.IsSyncronizeLeader; }
        }
        public string FilenameForTitle
        {
            get { return VideoPlayerVM.FilenameForTitle; }
        }

        public TimeSpan TimeShift
        {
            get { return VideoPlayerVM.SyncronizationShiftVM.ShiftTime; }
        }

        public TimeSpan Duration
        {
            get { return VideoPlayerVM.Duration; }
        }

        public Thickness Margin
        {
            get
            { 
                if (IsSyncronizeLeader) return new Thickness(0, 0, 0, 0);
                else return new Thickness(TimeShift.TotalSeconds, 0, 0, 0);
            }
        }

        public double Width
        {
            get
            {
                if (VideoPlayerVM.IsSyncronizeLeader)
                    return StripeContainerVM.SyncLeadBodyWidth;
                else
                {
                    double k = Duration.TotalSeconds / StripeContainerVM.SyncLeadDuration.TotalSeconds;

                    return StripeContainerVM.SyncLeadBodyWidth*k;
                }
            }
        }
        public void Refresh()
        {
            OnPropertyChanged("IsSyncronizeLeader");
            OnPropertyChanged("FilenameForTitle");
            OnPropertyChanged("TimeShift");
            OnPropertyChanged("Margin");
            OnPropertyChanged("Width");
        }


        #endregion

        #region Комманды


        #endregion

    }
}
