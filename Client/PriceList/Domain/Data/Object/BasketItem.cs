using System;
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
    public class BasketItem : Notifier
    {
        #region Members

        private BrandItem brand;
        private List<byte[]> photos;
        private string count;
        private decimal sum;
        private readonly IDataService databaseService;
        private IImageService imageService;

        #endregion

        #region Constructors

        public BasketItem(BasketItemEntity entity, 
                          IDataService databaseService, 
                          IImageService imageService)
        {
            Entity = entity;
            this.databaseService = databaseService;
            this.imageService = imageService;
            CalculateSum();
        }

        #endregion

        #region Properties

        public BasketItemEntity Entity { get; }

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

        public BitmapSource DeleteIcon => imageService?.ConvertToBitmapSource(Resources.delete);

        public string FullPrice => Price.ToString(CultureInfo.InvariantCulture) + " " + Currency;

        public decimal Count
        {
            get
            {
                return Entity.Count;
            }
            set
            {
                decimal oldValue = Entity.Count;
                long id = Entity.CatalogItem.Id;

                if (Entity.Count != value)
                {
                    Entity.Count = value;
                    CalculateSum();
                    OnPropertyChanged();

                    if (value == 0)
                    {
                        OrderEntity order = Entity.Order;
                        databaseService.Delete(Entity);

                        if (order != null && order.Sum == 0)
                        {
                            databaseService.Delete(order);
                            OnDeleteOrder();
                        }

                        OnCountChanged(id, oldValue, value);
                        OnDeletedItem();
                    }
                    else
                    {
                        OnCountChanged(id, oldValue, value);
                    }
                }
            }
        }

        public decimal Sum
        {
            get
            {
                return sum;
            }
            set
            {
                if (sum != value)
                {
                    sum = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AllowChangeCount
        {
            get
            {
                bool result = Entity.Order == null || Entity.Order.OrderStatus == OrderStatus.New; 
                return result;
            }
        }

        #endregion

        #region Methods

        private List<byte[]> GetPhotos()
        {
            if (photos == null)
            {
                databaseService.LoadPhotos(Entity.CatalogItem);
                photos = Entity.CatalogItem.Photos.Select(x => x.Photo).ToList();
            }

            return photos;
        }

        private void CalculateSum()
        {
            Sum = Price * Count;
            CalculateOrderSum();
        }

        private void CalculateOrderSum()
        {
            databaseService.CalculateOrderSum(Entity);
        }

        public void Refresh()
        {
            CalculateSum();
        }

        private void OnCountChanged(long id, decimal oldValue, decimal newValue)
        {
            CountChanged?.Invoke(this, new DecimalValueChangedEventArgs(id, 
                                                                        oldValue, 
                                                                        newValue, 
                                                                        MenuItemName.Basket));
        }

        private void OnDeletedItem()
        {
            DeletedItem?.Invoke(this, new EventArgs());
        }

        private void OnDeleteOrder()
        {
            DeletedOrder?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Events

        public event CountChangedEventHandler CountChanged;

        public event EventHandler DeletedItem;

        public event EventHandler DeletedOrder;

        #endregion
    }
}
