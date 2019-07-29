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
    public class SyncronizationShiftToSliderValue : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TimeSpan timeInterval = TimeSpan.FromMinutes(10);//(TimeSpan)values[0];
            double Max = (double)values[1];
            double Min = (double)values[2];
            TimeSpan shift = TimeSpan.FromMinutes(1);//(TimeSpan)values[3];

            double val = (Max - Min)* shift.TotalSeconds / timeInterval.TotalSeconds ;

            return val;
        }



        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue, DependencyProperty.UnsetValue };
        }
    }



}
