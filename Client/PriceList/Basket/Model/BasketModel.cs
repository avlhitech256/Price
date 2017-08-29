using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows.Documents;
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
            SelectedItem = null;
            long position = 1;

            List<BasketItemEntity> entityItems = DataService.Select<BasketItemEntity>()
                .Include(x => x.CatalogItem)
                .Include(x => x.Order)
                .Where(x => x.Order == null)
                .ToList();

            MergeItems(entityItems);

            entityItems.ForEach(x => Entities.Add(new BasketItem(x, DataService, ImageService) {Position = position++}));

            SelectedItem = Entities.FirstOrDefault(x => x.Id == basketId) 
                ?? Entities.FirstOrDefault(x => x.Id == oldBasketId)
                ?? Entities.FirstOrDefault();

            oldSelectedItem = null;
            oldSelectedItemId = -1L;
        }

        private void MergeItems(List<BasketItemEntity> items)
        {
            if (items != null && items.Any())
            {
                int index = 0;

                while (index < items.Count)
                {
                    BasketItemEntity item = items[index];

                    List<BasketItemEntity> dublicate = items.Where(t => t != item && t.CatalogItem.Id == item.CatalogItem.Id).ToList();

                    if (dublicate.Any())
                    {
                        decimal commonCount = item.Count + dublicate.Sum(c => c.Count);
                        item.Count = commonCount;
                        items.RemoveAll(x => dublicate.Contains(x));
                        dublicate.ForEach(d => DataService.Delete(d));
                    }

                    index++;
                }
            }
        }

        #endregion

        #region Events

        public event CountChangedEventHandler CountChanged;

        #endregion
    }
}
