using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Media.Imaging;
using Common.Data.Enum;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using Domain.Properties;
using Media.Image;

namespace Domain.Data.Object
{
    public class OrderItem : Notifier
    {
        #region Members

        private BrandItem brand;
        private List<byte[]> photos;
        private string count;
        private decimal sum;
        private IImageService imageService;


        #endregion

        #region Constructors

        public OrderItem(OrderEntity entity, IImageService imageService)
        {
            Entity = entity;
            this.imageService = imageService;
        }

        #endregion

        #region Properties

        public OrderEntity Entity { get; }

        public long Id => Entity.Id;

        public long Position { get; set; }

        public bool Selected { get; set; }

        public string OrderNumber => Entity.OrderNumber;

        public string Comment
        {
            get
            {
                return Entity.Comment;
            }
            set
            {
                if (Entity.Comment != value)
                {
                    Entity.Comment = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<BasketItemEntity> BasketItems => Entity.BasketItems;

        public DateTimeOffset DateOfCreation => Entity.DateOfCreation;

        public OrderStatus OrderStatus
        {
            get
            {
                return Entity.OrderStatus;
            }
            set
            {
                if (Entity.OrderStatus != value)
                {
                    Entity.OrderStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Sum => Entity.Sum.ToString(CultureInfo.InvariantCulture) + " " + Currency;

        public string Currency => Entity.BasketItems.FirstOrDefault()?.CatalogItem.Currency;

        public BitmapSource DeleteIcon => imageService?.ConvertToBitmapSource(Resources.delete);

        #endregion
    }
}
