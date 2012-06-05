using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Utilities
{
    public sealed class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0)
                return Application.Current.Resources["TextBlockStyleNormal"];
            return Application.Current.Resources["TextBlockStyleLarge"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == Application.Current.Resources["TextBlockStyleNormal"])
                return 0;
            return 1;
        }
    }
}
