using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class TimeSpanToTotalMiliseconds : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((TimeSpan)value).TotalMilliseconds.ToString(@"####");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (double.TryParse((string)value, out double d)) return TimeSpan.FromMilliseconds(d);
            else return DependencyProperty.UnsetValue;
        }

    }
}