using System.Collections.ObjectModel;
using System.Linq;
using Common.Data.Notifier;
using Common.Messenger;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.Service.Precision;

namespace Basket.Model
{
    public class BasketModel : Notifier
    {
        #region Members

        private CatalogItem selectedItem;
        private CatalogItem oldSelectedItem;
        private ObservableCollection<CatalogItem> entities;

        #endregion

        #region Constructors

        public BasketModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Entities = new ObservableCollection<CatalogItem>();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;

        private IPrecisionService PrecisionService => DomainContext?.PrecisionService;

        public CatalogItem SelectedItem
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

        public ObservableCollection<CatalogItem> Entities
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
    }
}
