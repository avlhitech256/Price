using System.Windows.Media;

namespace Media.Service
{
    public interface IColorService
    {
        LinearGradientBrush CreateBrush(byte r, byte g, byte b);
    }
}
