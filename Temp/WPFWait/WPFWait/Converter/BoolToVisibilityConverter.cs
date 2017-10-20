using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFWait.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result = Visibility.Hidden;

            if (value is bool)
            {
                result = (bool) value ? Visibility.Visible : Visibility.Hidden;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
