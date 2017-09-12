using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Media.Image;

namespace Order.Model.Implementation
{
    public class DetailOrderModel : Notifier, IDetailOrderModel
    {
        #region Members

        private OrderItem currentOrder;
        private BasketItem selectedItem;
        private long oldSelectedItemId;
        private ObservableCollection<BasketItem> entities;
        private string capture;

        #endregion

        #region Constructors

        public DetailOrderModel(IDomainContext domainContext)
        {
            entities = new ObservableCollection<BasketItem>();
            DomainContext = domainContext;
            CurrentOrder = null;
            Entities = new ObservableCollection<BasketItem>();
            SelectEntities();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        private IImageService ImageService => DomainContext?.ImageService;

        public OrderItem CurrentOrder
        {
            get
            {
                return currentOrder;
            }
            set
            {
                if (currentOrder != value)
                {
                    currentOrder = value;
                    SelectEntities();
                    Capture = "Заказ № " + value.OrderNumber;
                    OnPropertyChanged();
                }
            }
        }

        public string Capture
        {
            get
            {
                return capture;
            }
            private set
            {
                if (capture != value)
                {
                    capture = value;
                    OnPropertyChanged();
                }
            }

        }

        public BasketItem SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<BasketItem> Entities
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
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public void SelectEntities()
        {
            long oldBasketId = oldSelectedItemId;
            long basketId = SelectedItem?.Id ?? oldBasketId;

            Entities.Clear();
            SelectedItem = null;
            long position = 1L;

            if (CurrentOrder != null)
            {
                DataService.Select<BasketItemEntity>()
                    .Include(x => x.CatalogItem)
                    .Where(x => x.Order.Id == CurrentOrder.Entity.Id)
                    .ToList()
                    .ForEach(x => Entities.Add(new BasketItem(x, DataService, ImageService) {Position = position++}));
            }

            SelectedItem = Entities.FirstOrDefault(x => x.Id == basketId)
                ?? Entities.FirstOrDefault(x => x.Id == oldBasketId)
                ?? Entities.FirstOrDefault();

            oldSelectedItemId = -1L;
        }

        public void Clear()
        {
            CurrentOrder = null;
            SelectEntities();
        }

        #endregion
    }
}
