using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media.Imaging;
using Common.Data.Notifier;
using Common.Event;
using Domain.Properties;
using Domain.Service.Precision;
using Media.Image;

namespace Domain.Data.Object
{
    public class CatalogItem : Notifier
    {
        private string count;
        private decimal countValue;
        private IImageService imageService;
        private readonly IPrecisionService precisionService;

        public event CountChangedEventHandler CountChanged;

        public CatalogItem(IPrecisionService precisionService, IImageService imageService)
        {
            this.imageService = imageService;
            this.precisionService = precisionService;
            CountValue = 0;
        }
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

        public List<byte[]> Photo { get; set; }

        public BitmapSource PhotoIcon => imageService?.ConvertToBitmapSource(Resources.Camera);

        public string FullPrice => Price.ToString(CultureInfo.InvariantCulture) + " " + Currency;

        public string Count
        {
            get
            {
                return count;
            }
            set
            {
                string stringValue = precisionService?.NormalizeValue(value);

                if (count != stringValue)
                {
                    count = stringValue;

                    if (precisionService != null)
                    {
                        CountValue = precisionService.Convert(Count);
                    }

                    OnPropertyChanged();
                }
            }
        }

        public decimal CountValue
        {
            get
            {
                return countValue;
            }
            set
            {
                if (countValue != value)
                {
                    countValue = value;
                    Count = precisionService?.Convert(CountValue);
                    OnPropertyChanged();
                }
            }
        }

        #region Methods

        public void OnCountChanged(decimal oldValue, decimal newValue)
        {
            CountChanged?.Invoke(this, new DecimalValueChangedEventArgs(Id, oldValue, newValue));
        }

        #endregion
    }
}
