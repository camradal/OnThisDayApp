using System;
using System.Globalization;
using System.Windows.Data;
using OnThisDayApp.Resources;

namespace Utilities
{
    public sealed class SortOrderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? 1 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value == 1;
        }
    }
}
