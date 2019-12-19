using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class TimeShiftToColor : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t = ((TimeSpan)value).Duration(); 
            if (t > TimeSpan.FromMilliseconds(0) && t < TimeSpan.FromMilliseconds(200)) return new SolidColorBrush(Colors.Green);
            else if (t >= TimeSpan.FromMilliseconds(200) && t < TimeSpan.FromMilliseconds(300)) return new SolidColorBrush(Colors.Yellow);
            else if (t >= TimeSpan.FromMilliseconds(300)) return new SolidColorBrush(Colors.Red);
            else return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }

    class TimeShiftToColorBack : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t = ((TimeSpan)value).Duration();
            if (t > TimeSpan.FromMilliseconds(0) && t < TimeSpan.FromMilliseconds(200)) return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            else if (t >= TimeSpan.FromMilliseconds(200) && t < TimeSpan.FromMilliseconds(300)) return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            else if (t >= TimeSpan.FromMilliseconds(300)) return new SolidColorBrush(Colors.Yellow);
            else return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
