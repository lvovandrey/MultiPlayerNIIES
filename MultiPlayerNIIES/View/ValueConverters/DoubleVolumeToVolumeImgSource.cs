

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class DoubleVolumeToVolumeImgSource : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "Images/PNG/Cross.png";
            if (!(value is double)) return "Images/PNG/Cross.png";


            if ((double)value == 0 )
                return "Images/PNG/Gnome-Audio-Volume-Muted-64.png";
            else if ((double)value > 0 && (double)value < 100)
                return "Images/PNG/Gnome-Audio-Volume-Low-64.png";
            else if ((double)value >= 100 && (double)value < 400)
                return "Images/PNG/Gnome-Audio-Volume-Medium-64.png";
            else if ((double)value >= 400 && (double)value < 600)
                return "Images/PNG/Gnome-Audio-Volume-High-64.png";
            else
                return "Images/PNG/Gnome-Audio-Volume-Muted-64.png";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
