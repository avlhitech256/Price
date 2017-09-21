using System;
using System.Globalization;
using System.Windows.Data;

namespace Common.Convert.Implementation
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result;

            if (value is bool)
            {
                result = !(bool) value;
            }
            else
            {
                throw new NotSupportedException();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
