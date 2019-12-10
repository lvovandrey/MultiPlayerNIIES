using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class TimeSpanHiConverter : IValueConverter //TODO: на самом деле этот конвертер для TimeBlock не нужен можно просто указать StringFormat='Длительность {0:hh\\:mm\\:ss\\.ff}'
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string s = "";
            if (parameter is string) s = (string)parameter;
            string sign = "";
            if ((TimeSpan)value < TimeSpan.Zero) sign = "-";

            return s + sign + ((TimeSpan)value).ToString(@"hh\:mm\:ss\.ff");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }

    class TimeSpanHiSignedConverter : IValueConverter //TODO: на самом деле этот конвертер для TimeBlock не нужен можно просто указать StringFormat='Длительность {0:hh\\:mm\\:ss\\.ff}'
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t = (TimeSpan)value;
            string s = "";
            if (parameter is string) s = (string)parameter;
            string sign = "+";
            if (t < TimeSpan.Zero) sign = "-";

            if (t.Duration() < TimeSpan.FromMinutes(1))
                return s + sign + (t).ToString(@"ss\.ff");
            if (t.Duration() < TimeSpan.FromHours(1))
                return s + sign + t.ToString(@"mm\:ss\.ff");
            else
                return s + sign + t.ToString(@"hh\:mm\:ss\.ff");

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

    }
}
