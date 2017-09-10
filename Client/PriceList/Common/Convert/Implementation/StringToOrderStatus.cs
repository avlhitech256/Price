using System;
using System.Globalization;
using System.Windows.Data;
using Common.Data.Enum;

namespace Common.Convert.Implementation
{
    public class StringToOrderStatus : IValueConverter
    {
        private IConvertService convertService;

        public StringToOrderStatus()
        {
            convertService = new ConvertService();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;

            if (value != null && value is OrderStatus)
            {
                result = convertService.Convert((OrderStatus) value);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OrderStatus result;

            string status = value as string;

            if (status != null)
            {
                result = convertService.Convert(status);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}
