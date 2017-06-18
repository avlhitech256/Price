using System.Windows.Media;

namespace Media.Color
{
    public interface IColorService
    {
        LinearGradientBrush CreateBrush(byte r, byte g, byte b);
    }
}
