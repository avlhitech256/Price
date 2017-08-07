using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using Catalog.SearchCriteria;
using Common.Data.Holders;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.Service.Precision;
using Media.Image;

namespace Catalog.Model
{
    public class CatalogModel : Notifier
    {
        #region Members

        private CatalogItem selectedItem;
        private CatalogItem oldSelectedItem;
        private ObservableCollection<CatalogItem> entities;
        private ObservableCollection<BrandItem> brandItems;
        private decimal amount;
        public event CountChangedEventHandler CountChanged;

        #endregion

        #region Constructors

        public CatalogModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Amount = 0;
            Entities = new ObservableCollection<CatalogItem>();
            BrandItems = new ObservableCollection<BrandItem>();
            SearchCriteria = new CatalogSearchCriteria(Messenger);
            SelectBrandPopupItems();
            SelectEntities();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;

        private IImageService ImageService => DomainContext?.ImageService;

        private IPrecisionService PrecisionService => DomainContext?.PrecisionService;

        private IDataService DataServise => DomainContext.DataService;

        public CatalogSearchCriteria SearchCriteria { get; }

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
                    CatalogItem oldValue = SelectedItem;
                    selectedItem = value;
                    OnPropertyChanged();
                    OnChangeSelectedItem(oldValue, value);
                    SendSetImageMessage();
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
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<BrandItem> BrandItems
        {
            get
            {
                return brandItems;
            }
            set
            {
                if (brandItems != value)
                {
                    brandItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount != value)
                {
                    amount = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void OnChangeSelectedItem(CatalogItem oldItem, CatalogItem newItem)
        {
            UnsubscribeSelectedItemEvents(oldItem);
            SubscribeSelectedItemEvents(newItem);
        }

        private void UnsubscribeSelectedItemEvents(CatalogItem oldItem)
        {
            if (oldItem != null)
            {
                oldItem.CountChanged -= OnAmountChanged;
            }
        }

        private void SubscribeSelectedItemEvents(CatalogItem newItem)
        {
            if (newItem != null)
            {
                newItem.CountChanged += OnAmountChanged;
            }
        }

        private void OnAmountChanged(object sender, DecimalValueChangedEventArgs e)
        {
            Amount = Amount - e.OldValue + e.NewValue;
            CountChanged?.Invoke(this, e);
        }

        private void SendSetImageMessage()
        {
            Messenger.Send(CommandName.SetImage, SelectedItem);
        }

        public void SelectBrandPopupItems()
        {
            BrandItems.Clear();
            BrandItems.Add(new BrandItem(SearchCriteria.FirstBrandItemEntity));

            DataServise.Select<BrandItemEntity>()
                .OrderBy(x => x.Name)
                .ToList()
                .ForEach(x => BrandItems.Add(new BrandItem(x)));

            if (SearchCriteria != null)
            {
                BrandItem selectBrandItem = BrandItems.FirstOrDefault(x => x.Id == SearchCriteria.BrandId);

                if (selectBrandItem == null)
                {
                    selectBrandItem = BrandItems.FirstOrDefault();
                }

                SearchCriteria.BrandId = selectBrandItem?.Id ?? SearchCriteria.FirstBrandItemEntity.Id;
                SearchCriteria.BrandName = selectBrandItem?.Name ?? SearchCriteria.FirstBrandItemEntity.Name;
            }
        }

        public void SelectEntities()
        {
            Func<string, string[]> prepareArray =
                x =>
                {
                    List<string> results = x.Split(',', ' ').ToList();
                    results.RemoveAll(string.IsNullOrWhiteSpace);
                    return results.ToArray();
                };

            Entities.Clear();

            string[] codes = prepareArray(SearchCriteria.Code);
            string[] lexemes = prepareArray(SearchCriteria.Name);
            string[] articles = prepareArray(SearchCriteria.Article);
            LongHolder position = new LongHolder();
            DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
            DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);

            DataServise.Select<CatalogItemEntity>()
                .Include(x => x.Brand)
                .Where(x => !codes.Any() || codes.Contains(x.Code))
                .Where(n => !lexemes.Any() || lexemes.Any(s => n.Name.Contains(s)))
                .Where(x => !articles.Any() || articles.Contains(x.Article))
                .Where(x => !SearchCriteria.IsNew || (x.IsNew && x.LastUpdated <= dateForNew))
                .Where(
                    x =>
                        !SearchCriteria.PriceIsDown ||
                        (x.PriceIsDown && x.LastUpdated <= dateForPrice))
                .Where(
                    x => !SearchCriteria.PriceIsUp || (x.PriceIsUp && x.LastUpdated <= dateForPrice))
                .Where(x => SearchCriteria.BrandId <= SearchCriteria.FirstBrandItemEntity.Id || 
                                                      x.Brand.Id == SearchCriteria.BrandId)
                .ToList()
                .ForEach(
                    x =>
                        Entities.Add(new CatalogItem(x, DataServise, PrecisionService, ImageService)
                        {
                            Position = ++position.Value
                        }));
            SelectedItem = Entities.FirstOrDefault();
            SearchCriteria.SearchComplited();
        }

        #endregion
    }
}
