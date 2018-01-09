using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Media.Color;
using Media.Color.Implementation;

namespace Media.Converter
{
    public class BoolToAccessInternetBackgroundConverter : IValueConverter
    {
        private readonly IColorService colorService;

        public BoolToAccessInternetBackgroundConverter()
        {
            colorService = new ColorService();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush = colorService.CreateBrush(0xFF, 0x10, 0x10);

            if (value is bool)
            {
                brush = (bool) value
                    ? colorService.CreateBrush(0x10, 0xFF, 0x10)
                    : colorService.CreateBrush(0xFF, 0x10, 0x10);
            }

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
