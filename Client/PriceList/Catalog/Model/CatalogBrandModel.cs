using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
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
            AutoEntities = new List<BrandItem>();
            OtherEntities = new List<BrandItem>();
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

        public List<BrandItem> AutoEntities { get; private set; }

        public List<BrandItem> OtherEntities { get; private set; }

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
                    List<BrandItemEntity> avtoEntities;
                    List<BrandItemEntity> otherEntities;

                    List<BrandItem> loadedEntities = GetItems(out avtoEntities, out otherEntities)
                        .Select(x => new BrandItem(x) { Position = position.Value++ })
                        .ToList();

                    AutoEntities = loadedEntities.Where(x => avtoEntities.Contains(x.Entity)).ToList();
                    OtherEntities = loadedEntities.Where(x => otherEntities.Contains(x.Entity)).ToList();

                    loadedEntities.ForEach(
                        x => x.Selected = SearchCriteria.SelectedAvtoBrandItems.Any(s => s.Id == x.Id && s.Selected) ||
                                          SearchCriteria.SelectedOtherBrandItems.Any(o => o.Id == x.Id && o.Selected));

                    SearchCriteria.SelectedAvtoBrandItems.Clear();
                    AutoEntities.Where(x => x.Selected).ToList().ForEach(x => SearchCriteria.SelectedAvtoBrandItems.Add(x));

                    SearchCriteria.SelectedOtherBrandItems.Clear();
                    OtherEntities.Where(x => x.Selected).ToList().ForEach(x => SearchCriteria.SelectedOtherBrandItems.Add(x)); 

                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            Entities = loadedEntities;
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

        private List<BrandItemEntity> GetItems(out List<BrandItemEntity> avtoEntities, out List<BrandItemEntity> otherEntities)
        {
            List<BrandItemEntity> items = new List<BrandItemEntity>();
            avtoEntities = new List<BrandItemEntity>();
            otherEntities = new List<BrandItemEntity>();

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
                List<Guid> commodityDirectionAvtoCriteria = 
                    SearchCriteria?.GetCommodityDirectionCriteria(CommodityDirectionType.Avto) ?? new List<Guid>();
                List<Guid> commodityDirectionOtherCriteria = 
                    SearchCriteria?.GetCommodityDirectionCriteria(CommodityDirectionType.Other) ?? new List<Guid>();
                List<long> directoryIds = SearchCriteria?.GetDirectoryIds() ?? new List<long>();

                avtoEntities = DataService.Select<BrandItemEntity>()
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
                    .Where(x => commodityDirectionAvtoCriteria.Any() &&
                                x.CommodityDirection.Any(c => commodityDirectionAvtoCriteria.Contains(c.Code)))
                    .Where(x => !directoryIds.Any() || directoryIds.Any(id => id == -2))
                    .Select(x => x.Brand)
                    .Distinct()
                    .OrderBy(x => x.Name)
                    .ToList();

                otherEntities = DataService.Select<BrandItemEntity>()
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
                    .Where(x => commodityDirectionOtherCriteria.Any() &&
                                x.CommodityDirection.Any(c => commodityDirectionOtherCriteria.Contains(c.Code)))
                    .Where(x => !directoryIds.Any() || directoryIds.Contains(x.Directory.Id))
                    .Select(x => x.Brand)
                    .Distinct()
                    .OrderBy(x => x.Name)
                    .ToList();

                items = avtoEntities.Union(otherEntities).Distinct().OrderBy(x => x.Name).ToList();
            }

            return items;
        }

        #endregion
    }
}
