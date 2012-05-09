using System;
using System.Globalization;
using System.Windows.Data;
using OnThisDayApp.Resources;
using System.Windows;

namespace Utilities
{
    public sealed class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
                return Application.Current.Resources["PhoneFontSizeNormal"];
            else
                return Application.Current.Resources["PhoneFontSizeMedium"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == Application.Current.Resources["PhoneFontSizeNormal"])
                return 0;
            else
                return 1;
        }
    }
}
