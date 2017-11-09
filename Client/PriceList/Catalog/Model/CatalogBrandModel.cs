using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        private List<BrandItem> entities;

        #endregion

        #region Constructors

        public CatalogBrandModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            DomainContext = domainContext;
            SearchCriteria = searchCriteria;
            Entities = new List<BrandItem>();
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

        public List<BrandItem> Entities
        {
            get
            {
                return entities;
            }
            private set
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
                try
                {
                    long brandId = SelectedItem?.Id ?? -1L;
                    Entities.Clear();
                    LongHolder position = new LongHolder { Value = 0 };

                    List<BrandItem> loadedEntities = 
                        GetItems().Select(x => new BrandItem(x) { Position = position.Value++ }).ToList();

                    if (SearchCriteria != null)
                    {
                        loadedEntities.ForEach(
                            x => x.Selected = SearchCriteria.SelectedBrandItems.Any(s => s.Id == x.Id && s.Selected));

                        SearchCriteria.BrandItems = loadedEntities;
                        SearchCriteria.SelectedBrandItems.Clear();
                        Entities.Where(x => x.Selected).ToList().ForEach(x => SearchCriteria.SelectedBrandItems.Add(x));
                    }

                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            Entities.AddRange(loadedEntities);
                            OnPropertyChanged(nameof(Entities));
                            SelectedItem = Entities.FirstOrDefault(x => x.Id == brandId) ?? Entities.FirstOrDefault();
                            SearchCriteria.BrandComplited();
                        });
                }
                catch (Exception e)
                {
                    ;
                    //throw;
                }
            }
        }

        private List<BrandItemEntity> GetItems()
        {
            List<BrandItemEntity> items = new List<BrandItemEntity>();

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
                List<long> selectedDirectoryIds = SearchCriteria?.GetSelectedDirectoryIds() ?? new List<long>();
                List<long> directoryIds = SearchCriteria?.GetDirectoryIds() ?? new List<long>();

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
                    .Where(x => (!selectedDirectoryIds.Any() && directoryIds.Contains(x.Directory.Id)) || selectedDirectoryIds.Contains(x.Directory.Id))
                    .Select(x => x.Brand)
                    .Distinct()
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            return items;
        }

        #endregion
    }
}
