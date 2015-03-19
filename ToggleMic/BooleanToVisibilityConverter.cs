using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace ToggleMic
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool)value;
            bool paramVal = true;

            if (parameter != null)
            {
                paramVal = System.Convert.ToBoolean(parameter);
            }

            return val == paramVal ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Visibility)value;

            bool paramVal = true;

            if (parameter != null)
            {
                paramVal = System.Convert.ToBoolean(parameter);
            }

            return val == Visibility.Visible && paramVal;
        }
    }
}
