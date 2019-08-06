﻿using MultiPlayerNIIES.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{
    public class SyncronizationShiftVM : INPCBase
    {
        private double sliderPosition;
        public double SliderPosition
        {
            get { return sliderPosition; }
            set { sliderPosition = value; OnPropertyChanged("SliderPosition"); OnPropertyChanged("ShiftTime"); }
        }

        private double sliderMaxPosition = 1000;
        public double SliderMaxPosition
        {
            get { return sliderMaxPosition; }
            set { sliderMaxPosition = value; OnPropertyChanged("SliderMaxPosition"); OnPropertyChanged("SliderMinPosition"); }
        }
        public double SliderMinPosition
        {
            get { return -sliderMaxPosition; }
            set { sliderMaxPosition = -value; OnPropertyChanged("SliderMaxPosition"); OnPropertyChanged("SliderMinPosition"); }
        }


//        private TimeSpan shiftTime;
        public TimeSpan ShiftTime
        {
            get { return TimeSpan.FromSeconds((SliderPosition/SliderMaxPosition)*ShiftMaxTime.TotalSeconds); }
            set
            {
                sliderPosition = (value.TotalSeconds/ShiftMaxTime.TotalSeconds)*SliderMaxPosition;
                if (sliderPosition > SliderMaxPosition) sliderPosition = SliderMaxPosition;
                if (sliderPosition < SliderMinPosition) sliderPosition = SliderMinPosition;
                OnPropertyChanged("SliderPosition"); OnPropertyChanged("ShiftTime"); }
        }

       


        private TimeSpan shiftMaxTime;
        public TimeSpan ShiftMaxTime
        {
            get { return shiftMaxTime; }
            set { shiftMaxTime = value; OnPropertyChanged("ShiftMaxTime"); OnPropertyChanged("ShiftMinTime"); OnPropertyChanged("SliderPosition"); OnPropertyChanged("ShiftTime"); }
        }
        public TimeSpan ShiftMinTime
        {
            get { return -shiftMaxTime; }
            set { shiftMaxTime = -value; OnPropertyChanged("ShiftMinTime"); OnPropertyChanged("ShiftMaxTime"); OnPropertyChanged("SliderPosition"); OnPropertyChanged("ShiftTime"); }
        }

        #region КОМАНДЫ
        private RelayCommand incMaxTimeCommand;
        public RelayCommand IncMaxTimeCommand
        {
            get
            {
                return incMaxTimeCommand ??
                  (incMaxTimeCommand = new RelayCommand(obj =>
                  {
                      ShiftMaxTime += TimeSpan.FromSeconds(1);
                  }));
            }
        }

        private RelayCommand decMaxTimeCommand;
        public RelayCommand DecMaxTimeCommand
        {
            get
            {
                return decMaxTimeCommand ??
                  (decMaxTimeCommand = new RelayCommand(obj =>
                  {
                      if(ShiftMaxTime>TimeSpan.FromSeconds(1))
                      ShiftMaxTime -= TimeSpan.FromSeconds(1);
                  }));
            }
        }
        #endregion


    }
}