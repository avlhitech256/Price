using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Common.Data.Enum;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Media.Image;
using Options.Service;

namespace Basket.Model
{
    public class OrderModel : Notifier
    {
        #region Members

        private OrderItem selectedItem;
        private long oldSelectedItemId;
        private ObservableCollection<OrderItem> entities;

        #endregion

        #region Constructors

        public OrderModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Entities = new ObservableCollection<OrderItem>();
            oldSelectedItemId = -1L;
            SelectEntities();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        private IOptionService OptionService => DomainContext?.OptionService;

        private IImageService ImageService => DomainContext?.ImageService;

        public OrderItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                oldSelectedItemId = SelectedItem?.Id ?? -1L;

                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OrderItem> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                if (entities != value)
                {
                    entities = value;
                    SelectedItem = entities.FirstOrDefault();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public void SelectEntities()
        {
            long oldOrderId = oldSelectedItemId;
            long orderId = SelectedItem?.Id ?? oldOrderId;

            Entities.Clear();
            SelectedItem = null;
            long position = 1L;

            DataService.Select<OrderEntity>()
                .Include(x => x.BasketItems)
                .Where(x => x.OrderStatus == OrderStatus.New)
                .ToList()
                .ForEach(x => Entities.Add(new OrderItem(x, ImageService) {Position = position++}));

            SelectedItem = Entities.FirstOrDefault(x => x.Id == orderId)
                ?? Entities.FirstOrDefault(x => x.Id == oldOrderId)
                ?? Entities.FirstOrDefault();

            oldSelectedItemId = -1L;
        }

        public void CreateOrder(IEnumerable<BasketItem> basketItems, string comment)
        {
            OrderEntity order = new OrderEntity
            {
                BasketItems = basketItems.Select(x => x.Entity).ToList(),
                Comment = comment,
                DateOfCreation = DateTimeOffset.Now,
                OrderStatus = OrderStatus.New,
                OrderNumber = OptionService.Login + "-" + OptionService.LastOrderNumber++,
                Sum = DataService.GetSumBasket()
            };

            DataService.Insert(order);
        }

        public void DeleteOrder()
        {
            DataService.DeleteEntities(SelectedItem.BasketItems);
            DataService.Delete(SelectedItem.Entity);
            SelectEntities();
        }

        public void SendOut()
        {
            SelectedItem.OrderStatus = OrderStatus.SentOut;
            DataService.DataBaseContext.SaveChanges();
            SelectEntities();
        }

        public void Revert()
        {
            SelectedItem.BasketItems.ForEach(x => x.Order = null);
            DataService.Delete(SelectedItem.Entity);
            SelectEntities();
        }

        #endregion
    }
}
