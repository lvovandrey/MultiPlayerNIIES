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
    class BoolFocusToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            LinearGradientBrush linGrBrush = new LinearGradientBrush(
                                            Color.FromArgb(255, 248, 210, 159),   
                                            Color.FromArgb(255, 212, 163, 98), 
                                            new Point(0.5, 0.9),
                                            new Point(0.5, 1));
            LinearGradientBrush linGrBrushUnfocus = new LinearGradientBrush(
                                            Colors.DimGray,
                                            Color.FromArgb(255, 60, 60, 60),
                                            new Point(0.5, 0.9),
                                            new Point(0.5, 1));
            if ((bool)value == true)
                return linGrBrush;
            else
                return linGrBrushUnfocus; 

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
