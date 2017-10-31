using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Holders;
using Common.Data.Notifier;
using Common.Event;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Media.Image;
using Options.Service;

namespace Catalog.Model
{
    public class CatalogModel : Notifier
    {
        #region Members

        private CatalogItem selectedItem;
        private CatalogItem oldSelectedItem;
        private List<CatalogItem> entities;
        private List<BrandItem> brandItems;
        private BrandItem externalBrandItem;
        private BrandItem oldExternalBrandItem;
        private decimal amount;
        private int startRowIndex;
        private int oldMaximumRows;
        private int count;
        private bool needToUpdateCount;
        private readonly Dictionary<int, List<CatalogItem>> cacheCatalogItems;


        #endregion

        #region Constructors

        public CatalogModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Amount = 0;
            StartRowIndex = 0;
            oldMaximumRows = MaximumRows;
            needToUpdateCount = true;
            oldSelectedItem = null;
            Entities = new List<CatalogItem>();
            cacheCatalogItems = new Dictionary<int, List<CatalogItem>>();
            BrandItems = new List<BrandItem>();
            SearchCriteria = new CatalogSearchCriteria();
            SubscribeEvents();
            SelectBrandPopupItems();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IImageService ImageService => DomainContext?.ImageService;

        private IDataService DataService => DomainContext?.DataService;

        public CatalogSearchCriteria SearchCriteria { get; }

        private IOptionService OptionService => DomainContext?.OptionService;


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
                    OnChangeSelectedItem(oldValue, value);
                    OnPropertyChanged();
                }
            }
        }

        public List<CatalogItem> Entities
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

        public List<BrandItem> BrandItems
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

        public int StartRowIndex
        {
            get
            {
                return startRowIndex;
            }
            set
            {
                if (startRowIndex != value)
                {
                    startRowIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaximumRows
        {
            get
            {
                return OptionService?.CatalogMaximumRows ?? 13;
            }
            set
            {
                if (OptionService != null && OptionService.CatalogMaximumRows != value)
                {
                    OptionService.CatalogMaximumRows = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Count
        {
            get
            {
                if (needToUpdateCount)
                {
                    needToUpdateCount = false;
                    count = GetCount();
                }

                return count;
            }
        }

        public BrandItem ExternalBrandItem
        {
            get
            {
                return externalBrandItem;
            }
            set
            {
                if (externalBrandItem != value)
                {
                    externalBrandItem = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            SearchCriteria.SearchCriteriaChanged += SearchCriteria_SearchCriteriaChanged;
        }

        private void SearchCriteria_SearchCriteriaChanged(object sender, EventArgs e)
        {
            needToUpdateCount = needToUpdateCount || SearchCriteria.IsModified;
        }

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

        public void SelectBrandPopupItems()
        {
            BrandItems.Clear();
            BrandItems.Add(new BrandItem(SearchCriteria.FirstBrandItemEntity));

            DataService.Select<BrandItemEntity>()
                .OrderBy(x => x.Name)
                .ToList()
                .ForEach(x => BrandItems.Add(new BrandItem(x)));

            if (SearchCriteria != null)
            {
                BrandItem selectBrandItem = BrandItems.FirstOrDefault(x => x.Id == SearchCriteria.BrandId) ??
                                            BrandItems.FirstOrDefault();

                SearchCriteria.BrandId = selectBrandItem?.Id ?? SearchCriteria.FirstBrandItemEntity.Id;
                SearchCriteria.BrandName = selectBrandItem?.Name ?? SearchCriteria.FirstBrandItemEntity.Name;
            }
        }

        private bool ExternalBrandItemIsChanged()
        {
            bool result = SearchCriteria.EnabledEdvanceSearch &&
                          (oldExternalBrandItem != null && ExternalBrandItem == null) ||
                          (oldExternalBrandItem == null && ExternalBrandItem != null) ||
                          (oldExternalBrandItem != null && ExternalBrandItem != null && 
                           oldExternalBrandItem.Id != ExternalBrandItem.Id);
            return result;
        }

        public void RefreshEntities()
        {
            OnPropertyChanged(nameof(Entities));
        }

        public void SelectEntities()
        {
            try
            {
                if (SearchCriteria != null)
                {
                    long catalogId = SelectedItem?.Id ?? -1L;
                    Entities.Clear();

                    if (SearchCriteria.IsModified || SearchCriteria.BrandItemIdsChanged || SearchCriteria.DirectoryItemIdsChanged)
                    {
                        count = GetCount();
                        needToUpdateCount = false;
                        oldExternalBrandItem = ExternalBrandItem;
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                StartRowIndex = 0;
                                OnPropertyChanged(nameof(Count));
                            });
                    }

                    if (StartRowIndex < Count)
                    {
                        GetItems(StartRowIndex, MaximumRows).ForEach(x => Entities.Add(x));
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                OnPropertyChanged(nameof(Entities));
                                SelectedItem = Entities.FirstOrDefault(x => x.Id == catalogId) ?? Entities.FirstOrDefault();
                                SearchCriteria.SearchComplited();
                            });
                        
                    }
                }
            }
            catch (Exception e)
            {
                ; //TODO Вывести Exception в стилизированное окно ошибок
            }
        }

        private List<CatalogItem> GetItems(int startRow, int maxRows)
        {
            if (oldMaximumRows != MaximumRows || SearchCriteria.IsModified || SearchCriteria.EnabledEdvanceSearch)
            {
                cacheCatalogItems.Clear();
                oldMaximumRows = MaximumRows;
            }

            List<CatalogItem> items;

            if (!cacheCatalogItems.TryGetValue(startRow, out items))
            {
                LongHolder position = new LongHolder {Value = startRow};

                List<CatalogItemEntity> entityItems = GetItems()
                    .OrderBy(x => x.Name)
                    .Skip(startRow)
                    .Take(maxRows)
                    .ToList();


                items = entityItems
                    .Select(x => new CatalogItem(x, DataService, ImageService)
                            {
                                Position = ++position.Value
                            })
                    .ToList();

                if (cacheCatalogItems.Count > 50)
                {
                    var firstItems = cacheCatalogItems.FirstOrDefault();
                    cacheCatalogItems.Remove(firstItems.Key);
                }

                cacheCatalogItems.Add(startRow, items);
            }

            return items;
        }

        private IQueryable<CatalogItemEntity> GetItems()
        {

            return SearchCriteria != null && SearchCriteria.EnabledEdvanceSearch ? GetEdvanceItems() : GetStandardItems();
        }

        private IQueryable<CatalogItemEntity> GetStandardItems()
        {
            IQueryable<CatalogItemEntity> items = null;

            if (SearchCriteria != null)
            {
                Func<string, string[]> prepareArray =
                    x =>
                    {
                        List<string> results = new List<string>();

                        if (!string.IsNullOrWhiteSpace(x))
                        {
                            results = x.Split(',', ' ').ToList();
                            results.RemoveAll(string.IsNullOrWhiteSpace);
                        }

                        return results.ToArray();
                    };

                string[] codes = prepareArray(SearchCriteria.Code);
                string[] lexemes = prepareArray(SearchCriteria.Name);
                string[] articles = prepareArray(SearchCriteria.Article);
                DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
                DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);

                items = DataService.Select<CatalogItemEntity>()
                    .Include(x => x.BasketItems)
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(n => !lexemes.Any() || lexemes.All(s => n.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) || 
                                 (SearchCriteria.IsNew && x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
                                 (SearchCriteria.PriceIsDown && x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) || 
                                 (SearchCriteria.PriceIsUp && x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
                    .Where(x => SearchCriteria.BrandId <= SearchCriteria.FirstBrandItemEntity.Id ||
                                x.Brand.Id == SearchCriteria.BrandId);
            }

            return items;
        }

        private IQueryable<CatalogItemEntity> GetEdvanceItems()
        {
            IQueryable<CatalogItemEntity> items = null;

            if (SearchCriteria != null)
            {
                Func<string, string[]> prepareArray =
                    x =>
                    {
                        List<string> results = new List<string>();

                        if (!string.IsNullOrWhiteSpace(x))
                        {
                            results = x.Split(',', ' ').ToList();
                            results.RemoveAll(string.IsNullOrWhiteSpace);
                        }

                        return results.ToArray();
                    };

                string[] codes = prepareArray(SearchCriteria.Code);
                string[] lexemes = prepareArray(SearchCriteria.Name);
                string[] articles = prepareArray(SearchCriteria.Article);
                DateTimeOffset dateForNew = DateTimeOffset.Now.AddDays(-14);
                DateTimeOffset dateForPrice = DateTimeOffset.Now.AddDays(-7);
                List<long> directoryIds = SearchCriteria?.GetDirectoryIds().ToList();
                List<long> brandIds = SearchCriteria?.GetBrandIds() ?? new List<long>();

                items = DataService.Select<CatalogItemEntity>()
                    .Include(x => x.BasketItems)
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(n => !lexemes.Any() || lexemes.All(s => n.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
                                 (SearchCriteria.IsNew && x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
                                 (SearchCriteria.PriceIsDown && x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
                                 (SearchCriteria.PriceIsUp && x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
                    .Where(x => !directoryIds.Any() || directoryIds.Contains(x.Directory.Id))
                    .Where(x => !brandIds.Any() || brandIds.Contains(x.Brand.Id))
                    .Distinct();
            }

            return items;
        }

        private int GetCount()
        {
            return GetItems().Count();
        }

        #endregion

        #region Events

        public event CountChangedEventHandler CountChanged;

        #endregion
    }
}
