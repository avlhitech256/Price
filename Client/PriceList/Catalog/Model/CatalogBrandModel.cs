using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Holders;
using Common.Data.Notifier;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Catalog.Model
{
    public class CatalogBrandModel : Notifier
    {
        #region Members

        private BrandItem selectedItem;
        private ObservableCollection<BrandItem> entities;

        #endregion

        #region Constructors

        public CatalogBrandModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            DomainContext = domainContext;
            SearchCriteria = searchCriteria;
            Entities = new ObservableCollection<BrandItem>();
        }

        #endregion

        #region Properties

        IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        private CatalogSearchCriteria SearchCriteria { get; }

        public BrandItem SelectedItem
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

        public ObservableCollection<BrandItem> Entities
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
            if (SearchCriteria != null)
            {
                long brandId = SelectedItem?.Id ?? -1L;
                Entities.Clear();
                LongHolder position = new LongHolder { Value = 0 };

                GetItems().ToList().ForEach(x => Entities.Add(new BrandItem(x) {Position = position.Value++}));
                OnPropertyChanged(nameof(Entities));
                SelectedItem = Entities.FirstOrDefault(x => x.Id == brandId) ?? Entities.FirstOrDefault();
            }
        }

        private IQueryable<BrandItemEntity> GetItems()
        {

            return SearchCriteria != null && SearchCriteria.EnabledEdvanceSearch ? GetEdvanceItems() : GetStandardItems();
        }

        private IQueryable<BrandItemEntity> GetStandardItems()
        {
            IQueryable<BrandItemEntity> items = null;

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

                items = DataService.Select<BrandItemEntity>()
                    .SelectMany(x => x.CatalogItems)
                    .Distinct()
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(x => !lexemes.Any() || lexemes.All(s => x.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
                                (SearchCriteria.IsNew &&
                                 x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
                                (SearchCriteria.PriceIsDown &&
                                 x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
                                (SearchCriteria.PriceIsUp &&
                                 x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
                    .Select(x => x.Brand)
                    .Distinct()
                    .OrderBy(x => x.Name);
            }

            return items;
        }

        private IQueryable<BrandItemEntity> GetEdvanceItems()
        {
            IQueryable<BrandItemEntity> items = null;

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
                List<Guid> commodityDirectionCriteria = SearchCriteria?.GetCommodityDirectionCriteria() ?? new List<Guid>();

                items = DataService.Select<BrandItemEntity>()
                    .SelectMany(x => x.CatalogItems)
                    .Distinct()
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(x => !lexemes.Any() || lexemes.All(s => x.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
                                (SearchCriteria.IsNew &&
                                 x.Status == CatalogItemStatus.New && x.DateOfCreation >= dateForNew) ||
                                (SearchCriteria.PriceIsDown &&
                                 x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus >= dateForPrice) ||
                                (SearchCriteria.PriceIsUp &&
                                 x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus >= dateForPrice))
                    .Where(x => !commodityDirectionCriteria.Any() || 
                                x.CommodityDirection.Any(c => commodityDirectionCriteria.Contains(c.Code)))
                    .Select(x => x.Brand)
                    .Distinct()
                    .OrderBy(x => x.Name);
            }

            return items;
        }

        #endregion

    }
}
