using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class TimeShiftToVisibility : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t = ((TimeSpan)value).Duration();
            if (t > TimeSpan.FromMilliseconds(10)) return Visibility.Visible;
            else return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
