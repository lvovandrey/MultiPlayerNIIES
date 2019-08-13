using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiPlayerNIIES.View.ValueConverters
{
    public class DoubleSyncDeltaToColor: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) return new SolidColorBrush(Colors.Gray);
            double v = (double)value;
            if (v < 50) return new SolidColorBrush(Colors.Green);
            else if (v < 75) return new SolidColorBrush(Colors.Yellow);
            else if (v < 95) return new SolidColorBrush(Colors.OrangeRed);
            else return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}

