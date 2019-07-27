﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MultiPlayerNIIES.View.ValueConverters
{
    class TimeSpanHiConverter : IValueConverter
    {
        
            public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                return ((TimeSpan)value).ToString(@"hh\:mm\:ss\.ff");
            }

            public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        
    }
}
