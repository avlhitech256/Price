using System.Drawing;
using System.Windows.Media.Imaging;

namespace Media.Image
{
    public interface IImageService
    {
        BitmapSource ConvertToBitmapSource(Bitmap image);

        BitmapImage ConvertToBitmapImage(byte[] image);

        byte[] ConvertToByteArray(BitmapImage image);

        byte[] ConvertToByteArray(System.Drawing.Image image);

        byte[] ConvertToByteArrayConvert2(System.Drawing.Image image);
    }
}
