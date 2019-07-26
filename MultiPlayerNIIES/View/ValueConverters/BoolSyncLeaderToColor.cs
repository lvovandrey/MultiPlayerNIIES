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
    class BoolSyncLeaderToColor: IValueConverter
    {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if ((bool)value == true)
                    return new SolidColorBrush(Colors.DodgerBlue);
                else
                    return new SolidColorBrush(Colors.Gray);

            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return DependencyProperty.UnsetValue;
            }
    }
}
