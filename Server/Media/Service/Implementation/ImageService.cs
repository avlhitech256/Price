using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Media.Service.Implementation
{
    public class ImageService : IImageService
    {
        public BitmapSource ConvertToBitmapSource(Bitmap imageData)
        {
            IntPtr hBitmap = imageData.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            BitmapSource image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                Int32Rect.Empty, sizeOptions);

            return image;
        }

        public BitmapImage ConvertToBitmapImage(byte[] image)
        {
            BitmapImage resultImage = null;

            if (image != null && image.Length > 0)
            {
                resultImage = new BitmapImage();

                using (var mem = new MemoryStream(image))
                {
                    mem.Position = 0;
                    resultImage.BeginInit();
                    resultImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    resultImage.CacheOption = BitmapCacheOption.OnLoad;
                    resultImage.UriSource = null;
                    resultImage.StreamSource = mem;
                    resultImage.EndInit();
                }

                resultImage.Freeze();
            }

            return resultImage;
        }

        public byte[] ConvertToByteArray(BitmapImage image)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        public byte[] ConvertToByteArray(Image image)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }

        public byte[] ConvertToByteArray(Bitmap image)
        {
            return ConvertToByteArray((Image) image);
        }

        public byte[] ConvertToByteArrayConvert2(Image image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public ObservableCollection<BitmapSource> Assemble(IEnumerable<byte[]> photos)
        {
            var result = new ObservableCollection<BitmapSource>();

            photos?.ToList().ForEach(x => result.Add(ConvertToBitmapImage(x)));

            return result;
        }
    }
}
