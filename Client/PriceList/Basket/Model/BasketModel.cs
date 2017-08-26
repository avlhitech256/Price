using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Common.Data.Notifier;
using Common.Event;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Media.Image;

namespace Basket.Model
{
    public class BasketModel : Notifier
    {
        #region Members

        private BasketItem selectedItem;
        private BasketItem oldSelectedItem;
        private long oldSelectedItemId;
        private ObservableCollection<BasketItem> entities;

        #endregion

        #region Constructors

        public BasketModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Entities = new ObservableCollection<BasketItem>();
            oldSelectedItem = null;
            oldSelectedItemId = -1L;
            SelectEntities();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

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
                oldSelectedItem = SelectedItem;
                oldSelectedItemId = SelectedItem?.Id ?? -1L;

                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnChangeSelectedItem(oldSelectedItem, value);
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

        private void OnChangeSelectedItem(BasketItem oldItem, BasketItem newItem)
        {
            UnsubscribeSelectedItemEvents(oldItem);
            SubscribeSelectedItemEvents(newItem);
        }

        private void UnsubscribeSelectedItemEvents(BasketItem oldItem)
        {
            if (oldItem != null)
            {
                oldItem.CountChanged -= OnCountChanged;
                oldItem.DeletedItem -= OnDeletedItem;
            }
        }

        private void SubscribeSelectedItemEvents(BasketItem newItem)
        {
            if (newItem != null)
            {
                newItem.CountChanged += OnCountChanged;
                newItem.DeletedItem += OnDeletedItem;
            }
        }

        private void OnCountChanged(object sender, DecimalValueChangedEventArgs e)
        {
            CountChanged?.Invoke(this, e);
        }

        private void OnDeletedItem(object sender, EventArgs eventArgs)
        {
            SelectEntities();
        }

        public void SelectEntities()
        {
            long oldBasketId = oldSelectedItemId;
            long basketId = SelectedItem?.Id ?? oldBasketId;

            Entities.Clear();

            DataService.Select<BasketItemEntity>()
                .Include(x => x.CatalogItem)
                .Include(x => x.Order)
                .Where(x => x.Order == null)
                .ToList()
                .ForEach(x => Entities.Add(new BasketItem(x, DataService, ImageService)));

            SelectedItem = Entities.FirstOrDefault(x => x.Id == basketId) 
                ?? Entities.FirstOrDefault(x => x.Id == oldBasketId)
                ?? Entities.FirstOrDefault();

            oldSelectedItem = null;
            oldSelectedItemId = -1L;
        }

        #endregion

        #region Events

        public event CountChangedEventHandler CountChanged;

        #endregion
    }
}
