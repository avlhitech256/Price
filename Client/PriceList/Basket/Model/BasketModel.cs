using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Common.Data.Notifier;
using Common.Messenger;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.Service.Precision;
using Media.Image;

namespace Basket.Model
{
    public class BasketModel : Notifier
    {
        #region Members

        private BasketItem selectedItem;
        private BasketItem oldSelectedItem;
        private ObservableCollection<BasketItem> entities;

        #endregion

        #region Constructors

        public BasketModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Entities = new ObservableCollection<BasketItem>();
            SelectEntities();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;

        private IPrecisionService PrecisionService => DomainContext?.PrecisionService;

        private IImageService ImageService => DomainContext?.ImageService;

        private IDataService DataService => DomainContext?.DataService;

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
                    SelectedItem = entities.FirstOrDefault();
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        public void SelectEntities()
        {
            long basketId = SelectedItem?.Id ?? -1L;

            Entities.Clear();

            DataService.Select<BasketItemEntity>()
                .Include(x => x.CatalogItem)
                .Include(x => x.OrderItem)
                .Where(x => x.OrderItem == null)
                .ToList()
                .ForEach(x => Entities.Add(new BasketItem(x, DataService, PrecisionService, ImageService)));

            SelectedItem = Entities.FirstOrDefault(x => x.Id == basketId) ?? Entities.FirstOrDefault();
        }


        #endregion#
    }
}
