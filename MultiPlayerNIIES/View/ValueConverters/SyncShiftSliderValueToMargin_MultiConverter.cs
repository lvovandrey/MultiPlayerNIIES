using System;
using System.Windows;
using System.Windows.Data;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class SyncShiftSliderValueToMargin_MultiConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = (double)values[0];
            var maxval = (double)values[1];
            var minval = (double)values[2];
            var w = (double)values[3];

            double center = w * (-minval / (maxval - minval));

            Thickness marg = new Thickness();
            double k = val / (maxval - minval);
            double pos = k * w;
            if (val > 0)
                marg = new Thickness(center, 0, w - center - pos + 5, 0);
            else
                marg = new Thickness(center + pos + 5, 0, w - center, 0);

            return marg;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
