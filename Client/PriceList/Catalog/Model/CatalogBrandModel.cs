using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        private BrandItemEntity selectedItem;
        private List<BrandItemEntity> entities;

        #endregion

        #region Constructors

        public CatalogBrandModel(IDomainContext domainContext, CatalogSearchCriteria searchCriteria)
        {
            DomainContext = domainContext;
            SearchCriteria = searchCriteria;
            Entities = null; //new IQueryable<BrandItemEntity>;
        }

        #endregion

        #region Properties

        IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        private CatalogSearchCriteria SearchCriteria { get; }

        public BrandItemEntity SelectedItem
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

        public List<BrandItemEntity> Entities
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
                //Entities.Clear();
                //GetItems().ForEach(x => Entities.Add(x));
                //IQueryable<BrandItemEntity> items = GetItems();
                //Entities = new List<BrandItemEntity>(items);

                //List<BrandItemEntity> 
                Entities = GetItems().ToList();
                OnPropertyChanged(nameof(Entities));
                SelectedItem = Entities.FirstOrDefault(x => x.Id == brandId) ?? Entities.FirstOrDefault();
            }
        }

        private IQueryable<BrandItemEntity> GetItems()
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
                LongHolder position = new LongHolder { Value = 0 };

                items = DataService.Select<BrandItemEntity>()
                    .Include(x => x.CatalogItems)
                    .Where(x => !codes.Any() || x.CatalogItems.Any(c => codes.Contains(c.Code)))
                    .Where(x => !lexemes.Any() || x.CatalogItems.Any(c => lexemes.All(s => c.Name.Contains(s))))
                    .Where(x => !articles.Any() || x.CatalogItems.Any(c => articles.Contains(c.Article)))
                    .Where(x => (!SearchCriteria.IsNew && !SearchCriteria.PriceIsDown && !SearchCriteria.PriceIsUp) ||
                                (SearchCriteria.IsNew &&
                                 x.CatalogItems.Any(
                                     c => c.Status == CatalogItemStatus.New && c.DateOfCreation >= dateForNew)) ||
                                (SearchCriteria.PriceIsDown &&
                                 x.CatalogItems.Any(
                                     c =>
                                         c.Status == CatalogItemStatus.PriceIsDown &&
                                         c.LastUpdatedStatus >= dateForPrice)) ||
                                (SearchCriteria.PriceIsUp &&
                                 x.CatalogItems.Any(
                                     c => c.Status == CatalogItemStatus.PriceIsUp && c.LastUpdatedStatus >= dateForPrice)))
                    .Where(x => SearchCriteria.BrandId <= SearchCriteria.FirstBrandItemEntity.Id ||
                                x.CatalogItems.Any(c => c.Brand.Id == SearchCriteria.BrandId))
                    .OrderBy(x => x.Name);
                //items.Load();

                //.Select(x => new BrandItem(x)
                //{
                //    Position = ++position.Value
                //});
                //.ToList(); ;
            }

            return items;
        }


        #endregion

    }
}
