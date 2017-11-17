using System;
using System.Globalization;
using System.Windows.Data;

namespace Common.Service.Implementation
{
    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? result;

            try
            {
                DateTimeOffset? dto = value as DateTimeOffset?;
                result = dto?.DateTime;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTimeOffset? result;

            try
            {
                DateTime? date = value as DateTime?;
                result = date != null ? new DateTimeOffset(date.Value) : (DateTimeOffset?)null;
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
    }
}
