using MultiPlayerNIIES.ViewModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class VideoPlayerWindowStateToImgSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "/Images/PNG/MaximizeO2.png";
            if (!(value is VideoPlayerWindowState)) return "/Images/PNG/MaximizeO2.png";


            if ((VideoPlayerWindowState)value == VideoPlayerWindowState.Maximized)
                return "/Images/PNG/RestoreO2.png";
            else 
                return "/Images/PNG/MaximizeO2.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
