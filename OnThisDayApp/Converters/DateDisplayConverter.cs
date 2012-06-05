using System;
using System.Globalization;
using System.Windows.Data;

namespace Utilities
{
    public sealed class DateDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                DateTime dateTime = (DateTime)value;
                return dateTime.ToString("M", CultureInfo.CurrentUICulture);
            }
            catch (Exception)
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
