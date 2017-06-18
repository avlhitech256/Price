using System;
using System.Windows;
using System.Windows.Media;

namespace Media.Color
{
    public class ColorService : IColorService
    {
        #region Methods
        public LinearGradientBrush CreateBrush(byte r, byte g, byte b)
        {
            GradientStop gradientStop1 = CreateGradientStop(ConvertMiddleLightColorChanel(r),
                                                            ConvertMiddleLightColorChanel(g),
                                                            ConvertMiddleLightColorChanel(b), 0);
            GradientStop gradientStop2 = CreateGradientStop(r, g, b, 1);
            GradientStop gradientStop3 = CreateGradientStop(ConvertMaxLightColorChanel(r),
                                                            ConvertMaxLightColorChanel(g),
                                                            ConvertMaxLightColorChanel(b), 0.1);

            GradientStopCollection gradientStopCollection =
                new GradientStopCollection
                {
                    gradientStop1,
                    gradientStop2,
                    gradientStop3,
                };

            LinearGradientBrush brush =
                new LinearGradientBrush
                {
                    StartPoint = new Point(0.5, 0),
                    EndPoint = new Point(0.5, 1),
                    GradientStops = gradientStopCollection
                };

            return brush;
        }

        private byte ConvertMaxLightColorChanel(byte x)
        {
            int result = (int)((Math.Exp(128 * 0.28 / x) + 0.67) * x);
            return result > 0xFF ? (byte) 0xFF : (byte) result;
        }


        private byte ConvertMiddleLightColorChanel(byte x)
        {
            int result = (int)((Math.Exp(128 * 0.04 / x) - 1.52) * x / 1.67);
            return result > 0xFF ? (byte) 0xFF : (byte) result;
        }

        private GradientStop CreateGradientStop(System.Windows.Media.Color color, double? offset = null)
        {
            GradientStop gradientStop = offset.HasValue
                ? new GradientStop(color, offset.Value)
                : new GradientStop { Color = color };
            return gradientStop;
        }

        private GradientStop CreateGradientStop(byte r, byte g, byte b, double? offset = null)
        {
            var color = System.Windows.Media.Color.FromArgb(0xFF, r, g, b);
            GradientStop gradientStop = CreateGradientStop(color, offset);

            return gradientStop;
        }

        #endregion

    }
}
