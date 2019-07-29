using MultiPlayerNIIES.Abstract;
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
            set { sliderPosition = value; OnPropertyChanged("SliderPosition"); }
        }

        private TimeSpan shiftTime;
        public TimeSpan ShiftTime
        {
            get { return shiftTime; }
            set { shiftTime = value; OnPropertyChanged("ShiftTime"); }
        }

    }
}
