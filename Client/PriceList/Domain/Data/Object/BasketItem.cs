using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Properties;
using Domain.Service.Precision;
using Media.Image;

namespace Domain.Data.Object
{
    public class BasketItem : Notifier
    {
        private BasketItemEntity entity;
        private BrandItem brand;
        private List<byte[]> photos;
        private string count;
        private decimal countValue;
        private readonly IDataService databaseService;
        private IImageService imageService;
        private readonly IPrecisionService precisionService;

        public BasketItem(BasketItemEntity entity, IDataService databaseService, IPrecisionService precisionService, IImageService imageService)
        {
            Entity = entity;
            this.databaseService = databaseService;
            this.imageService = imageService;
            this.precisionService = precisionService;
            CountValue = 0;
        }

        public BasketItemEntity Entity
        {
            get
            {
                return entity;
            }
            set
            {
                if (entity != value)
                {
                    entity = value;
                    OnPropertyChanged();
                }
            }
        }

        public long Id => Entity.Id;

        public long Position { get; set; }

        public bool Selected { get; set; }

        public string Code => Entity.CatalogItem.Code;

        public string Article => Entity.CatalogItem.Article;

        public string Name => Entity.CatalogItem.Name;

        public BrandItem Brand => brand ?? (brand = new BrandItem(Entity.CatalogItem.Brand));

        public string Unit => Entity.CatalogItem.Unit;

        public string EnterpriceNormPack => Entity.CatalogItem.EnterpriceNormPack;

        public string Balance => Entity.CatalogItem.Balance;

        public decimal Price => Entity.CatalogItem.Price;

        public string Currency => Entity.CatalogItem.Currency;

        public List<byte[]> Photos => GetPhotos();

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

        private List<byte[]> GetPhotos()
        {
            if (photos != null)
            {
                databaseService.LoadPhotos(Entity);
                photos = Entity.CatalogItem.Photos.Select(x => x.Photo).ToList();
            }

            return photos;
        }
    }
}
