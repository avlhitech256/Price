using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;
using Common.Data.Notifier;
using Common.Event;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Properties;
using Domain.Service.Precision;
using Media.Image;

namespace Domain.Data.Object
{
    public class CatalogItem : Notifier
    {
        #region Members

        private CatalogItemEntity entity;
        private BrandItem brand;
        private List<byte[]> photos;
        private long position;
        private string count;
        private decimal countValue;
        private IDataService databaseService;
        private IImageService imageService;
        private readonly IPrecisionService precisionService;

        #endregion

        #region Constructors

        public CatalogItem(CatalogItemEntity entity, 
                           IDataService databaseService, 
                           IPrecisionService precisionService, 
                           IImageService imageService)
        {
            Entity = entity;
            this.databaseService = databaseService;
            this.imageService = imageService;
            this.precisionService = precisionService;
            Brand = new BrandItem(entity.Brand);
            Position = 1L;
            CountValue = 0;
            photos = new List<byte[]>();
        }
        #endregion

        #region Properties

        public CatalogItemEntity Entity
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

        public long Id
        {
            get
            {
                return Entity.Id;
            }
            set
            {
                if (Entity.Id != value)
                {
                    Entity.Id = value;
                    OnPropertyChanged();
                }
            }
        }

        public long Position
        {
            get
            {
                return position;
            }
            set
            {
                if (position != value)
                {
                    position = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Code
        {
            get
            {
                return Entity.Code;
            }
            set
            {
                if (Entity.Code != value)
                {
                    Entity.Code = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Article
        {
            get
            {
                return Entity.Article;
            }
            set
            {
                if (Entity.Article != value)
                {
                    Entity.Article = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get
            {
                return entity.Name;
            }
            set
            {
                if (entity.Name != value)
                {
                    entity.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public BrandItem Brand
        {
            get
            {
                return brand;
            }
            set
            {
                if (brand != value)
                {
                    brand = value;
                    OnPropertyChanged();
                    entity.Brand = value.Entity;
                }
            }
        }

        public string Unit
        {
            get
            {
                return Entity.Unit;
            }
            set
            {
                if (Entity.Unit != value)
                {
                    Entity.Unit = value;
                    OnPropertyChanged();
                }
            }
        }

        public string EnterpriceNormPack
        {
            get
            {
                return Entity.EnterpriceNormPack;
            }
            set
            {
                if (Entity.EnterpriceNormPack != value)
                {
                    Entity.EnterpriceNormPack = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Balance
        {
            get
            {
                return Entity.Balance;
            }
            set
            {
                if (Entity.Balance != value)
                {
                    Entity.Balance = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get
            {
                return Entity.Price;
            }
            set
            {
                if (Entity.Price != value)
                {
                    Entity.Price = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Currency
        {
            get
            {
                return Entity.Currency;
            }
            set
            {
                if (Entity.Currency != value)
                {
                    Entity.Currency = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<byte[]> Photos
        {
            get
            {
                databaseService.LoadPhotos(Entity);
                return Entity.Photos.Select(x => x.Photo).ToList();
            }
            set
            {
                databaseService.LoadPhotos(Entity);
                if (value.Any(x => !Entity.Photos.Select(p => p.Photo).Contains(x)))
                {
                    value
                        .Where(x => !Entity.Photos.Select(p => p.Photo).Contains(x))
                        .ToList()
                        .ForEach(v => databaseService.AddPhoto(Entity, v));
                    OnPropertyChanged();
                }
            }
        }



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

        #endregion

        #region Methods

        public void OnCountChanged(decimal oldValue, decimal newValue)
        {
            CountChanged?.Invoke(this, new DecimalValueChangedEventArgs(Id, oldValue, newValue));
        }

        #endregion

        #region Events

        public event CountChangedEventHandler CountChanged;

        #endregion
    }
}
