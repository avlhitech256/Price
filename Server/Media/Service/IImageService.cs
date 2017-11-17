using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Media.Service
{
    public interface IImageService
    {
        BitmapSource ConvertToBitmapSource(Bitmap image);

        BitmapImage ConvertToBitmapImage(byte[] image);

        byte[] ConvertToByteArray(BitmapImage image);

        byte[] ConvertToByteArray(Image image);

        byte[] ConvertToByteArrayConvert2(Image image);

        ObservableCollection<BitmapSource> Assemble(IEnumerable<byte[]> photos);
    }
}
