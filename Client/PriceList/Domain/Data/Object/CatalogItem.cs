using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Resources;
using System.Windows;
using System.Windows.Media.Imaging;
using Domain.Properties;

namespace Domain.Data.Object
{
    public class CatalogItem
    {
        public long Id { get; set; }

        public long Position { get; set; }

        public string Code { get; set; }

        public string Article { get; set; }

        public string Name { get; set; }

        public BrandItem Brand { get; set; }

        public string Unit { get; set; }

        public string EnterpriceNormPack { get; set; }

        public string Balance { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public byte[] Photo { get; set; }

        public BitmapSource PhotoIcon => GetImage(Resources.Camera);

        public string FullPrice => Price.ToString(CultureInfo.InvariantCulture) + " " + Currency;

        private BitmapSource GetImage(Bitmap imageData)
        {
            IntPtr hBitmap = imageData.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            BitmapSource image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                Int32Rect.Empty, sizeOptions);

            return image;
        }

    }

}
