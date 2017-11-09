using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Properties;
using Media.Image;

namespace Domain.Data.Object
{
    public class CatalogItem : Notifier
    {
        #region Members

        private CatalogItemEntity entity;
        private BrandItem brand;
        private long position;
        private decimal count;
        private readonly IDataService databaseService;
        private readonly IImageService imageService;

        #endregion

        #region Constructors

        public CatalogItem(CatalogItemEntity entity, 
                           IDataService databaseService, 
                           IImageService imageService)
        {
            Entity = entity;
            this.databaseService = databaseService;
            this.imageService = imageService;
            Position = 1L;
            Refresh();
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

        public string Brand
        {
            get
            {
                return entity.BrandName;
            }
            set
            {
                if (entity.BrandName != value)
                {
                    entity.BrandName = value;
                    OnPropertyChanged();
                    BrandItemEntity brandItem =
                        databaseService.Select<BrandItemEntity>().FirstOrDefault(x => x.Name == Brand);
                    entity.Brand = brandItem;
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

        public decimal BatchOfSales
        {
            get
            {
                return Entity.BatchOfSales;
            }
            set
            {
                if (Entity.BatchOfSales != value)
                {
                    Entity.BatchOfSales = value;
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

        public CatalogItemStatus Status => Entity.Status;

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

        public bool HasPhotos => Entity.HasPhotos;

        public BitmapSource PhotoIcon
            =>
                HasPhotos
                    ? imageService?.ConvertToBitmapSource(Resources.Camera)
                    : imageService?.ConvertToBitmapSource(Resources.disableCameraDarkGrey);
        
        public string FullPrice => Price.ToString(CultureInfo.InvariantCulture) + " " + Currency;

        public decimal Count
        {
            get
            {
                return count;
            }
            set
            {
                decimal oldValue = count;

                if (count != value)
                {
                    count = value;
                    databaseService.SetCount(Entity, value);
                    OnCountChanged(oldValue, value);
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void OnCountChanged(decimal oldValue, decimal newValue)
        {
            CountChanged?.Invoke(this, new DecimalValueChangedEventArgs(Id, oldValue, newValue, MenuItemName.PriceList));
        }

        public void Refresh()
        {
            Count = databaseService.GetCount(Entity);  //TODO Необходимо будет сделать загрузку в фоне
        }

        #endregion

        #region Events

        public event CountChangedEventHandler CountChanged;

        #endregion
    }
}
