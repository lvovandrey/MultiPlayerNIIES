using MultiPlayerNIIES.Abstract;
using MultiPlayerNIIES.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiPlayerNIIES.ViewModel
{
    public class MainSliderVM : INPCBase
    {
        private MainSlider mainSlider;

        public MainSliderVM(MainSlider mainSlider)
        {
            this.mainSlider = mainSlider;
            mainSlider.OnPositionChanged += (d, e) => { OnPropertyChanged("Position"); };
        }
    }
}
