using System;
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
using Order.SearchCriteria;

namespace Order.Model.Implementation
{
    public class OrderModel : Notifier, IOrderModel
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
            SearchCriteria = new OrderSearchCriteria(domainContext.ConvertService);
            Entities = new ObservableCollection<OrderItem>();
            SelectEntities();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        private IImageService ImageService => DomainContext?.ImageService;

        public OrderSearchCriteria SearchCriteria { get; }

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
            bool fromDateIsNull = !SearchCriteria.FromDateTime.HasValue;
            bool toDateIsNull = !SearchCriteria.ToDateTime.HasValue;
            DateTimeOffset fromDate = !fromDateIsNull ? SearchCriteria.FromDateTime.Value.Date : DateTimeOffset.MinValue;
            DateTimeOffset toDate = !toDateIsNull ? SearchCriteria.ToDateTime.Value.Date.AddDays(1) : DateTimeOffset.MinValue;

            DataService.Select<OrderEntity>()
                .Include(x => x.BasketItems)
                .Where(x => SearchCriteria.OrderStatus == OrderStatus.All || x.OrderStatus == SearchCriteria.OrderStatus)
                .Where(x => fromDateIsNull || (x.DateOfCreation >= fromDate))
                .Where(x => toDateIsNull || (x.DateOfCreation < toDate))
                .ToList()
                .ForEach(x => Entities.Add(new OrderItem(x, ImageService) { Position = position++ }));

            SelectedItem = Entities.FirstOrDefault(x => x.Id == orderId)
                ?? Entities.FirstOrDefault(x => x.Id == oldOrderId)
                ?? Entities.FirstOrDefault();

            oldSelectedItemId = -1L;
            SearchCriteria.SearchComplited();
        }

        public void Clear()
        {
            SearchCriteria.Clear();
            SelectEntities();
        }

        public void DeleteOrder()
        {
            DataService.DeleteEntities(SelectedItem.BasketItems);
            DataService.Delete(SelectedItem.Entity);
            SelectEntities();
        }

        public void SendOut()
        {
            DataService.SetOrderStatus(SelectedItem.Entity, OrderStatus.SentOut);
            SelectEntities();
        }

        public void Revert()
        {
            SelectedItem.BasketItems.ForEach(x => x.Order = null);
            DataService.Delete(SelectedItem.Entity);
            SelectEntities();
        }

        #endregion

        #region Events


        #endregion
    }
}
