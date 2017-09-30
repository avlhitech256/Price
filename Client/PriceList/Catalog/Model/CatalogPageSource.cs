using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Holders;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;
using Media.Image;

namespace Catalog.Model
{
    public class CatalogPageSource
    {
        #region Members

        #endregion

        #region Constructors

        public CatalogPageSource()
        {
            SearchCriteria = new CatalogSearchCriteria();
            MaximumRows = 50;
            StartRowIndex = 0;
        }

        #endregion

        #region Properties

        public CatalogSearchCriteria SearchCriteria { get; set; }

        public IDomainContext DomainContext { get; set; }

        private IImageService ImageService => DomainContext?.ImageService;

        private IDataService DataService => DomainContext?.DataService;


        public int StartRowIndex { get; set; }

        public int MaximumRows { get; set; }

        #endregion

        #region Methods

        public IEnumerable<CatalogItem> GetCatalogItems(int maximumRows, int startRowIndex, string sort)
        {
            LongHolder position = new LongHolder();

            IEnumerable<CatalogItem> items = GetItems()
                .OrderBy(x => x.Name)
                .Skip(startRowIndex)
                .Take(maximumRows)
                .AsEnumerable()
                .Select(x => new CatalogItem(x, DataService, ImageService)
                        {
                            Position = ++position.Value
                        });

            return items;
        }

        public int Count()
        {
            int count = GetItems().Count();
            return count;
        }

        private IQueryable<CatalogItemEntity> GetItems()
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
                    .Include(x => x.Brand)
                    .Where(x => !codes.Any() || codes.Contains(x.Code))
                    .Where(n => !lexemes.Any() || lexemes.All(s => n.Name.Contains(s)))
                    .Where(x => !articles.Any() || articles.Contains(x.Article))
                    .Where(x => !SearchCriteria.IsNew || (x.Status == CatalogItemStatus.New && x.DateOfCreation <= dateForNew))
                    .Where(
                        x =>
                            !SearchCriteria.PriceIsDown ||
                            (x.Status == CatalogItemStatus.PriceIsDown && x.LastUpdatedStatus <= dateForPrice))
                    .Where(
                        x => !SearchCriteria.PriceIsUp || (x.Status == CatalogItemStatus.PriceIsUp && x.LastUpdatedStatus <= dateForPrice))
                    .Where(x => SearchCriteria.BrandId <= SearchCriteria.FirstBrandItemEntity.Id ||
                                x.Brand.Id == SearchCriteria.BrandId);
            }

            return items;
        }

        #endregion
    }
}
