using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using DataBase.Service;
using Option.Service;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    public class ShapingBrands : IShapingBrands
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public ShapingBrands(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
        }

        #endregion

        #region Methods

        public Brands GetItems(string login, DateTimeOffset lastUpdate)
        {
            ShapingBrandsList(login, lastUpdate);

            Brands result = new Brands
            {
                Count = GetBrandInfosCount(lastUpdate),
                Items = GetBrandInfos(lastUpdate)
            };

            return result;
        }

        public void ConfirmUpdateBrands(string login, DateTimeOffset lastUpdate, List<long> itemIds)
        {
            IEnumerable<SendItemsEntity> brandsToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.RequestDate == lastUpdate)
                .Where(x => itemIds.Contains(x.Id))
                .AsEnumerable();
            dataService.DeleteEntities(brandsToDelete);
        }

        private long GetBrandInfosCount(DateTimeOffset lastUpdate)
        {
            long result = dataService.Select<BrandItemEntity>().Count(x => x.LastUpdated > lastUpdate);
            return result;
        }

        private List<BrandInfo> GetBrandInfos(DateTimeOffset lastUpdate)
        {
            List<BrandInfo> result =
                dataService.Select<BrandItemEntity>()
                    .Where(x => x.LastUpdated > lastUpdate)
                    .Take(optionService.CountSendItems)
                    .Select(Assemble)
                    .ToList();

            return result;
        }

        private BrandInfo Assemble(BrandItemEntity item)
        {
            BrandInfo result = new BrandInfo
            {
                Id = item.Id,
                Code = item.Code,
                Name = item.Name,
                CatalogId = item.CatalogItems.Select(x => x.Id).ToList()
            };

            return result;
        }

        private void ShapingBrandsList(string login, DateTimeOffset lastUpdate)
        {
            if (dataService.Select<SendItemsEntity>().All(x => x.RequestDate != lastUpdate))
            {
                int count = 0;
                dataService.Select<BrandItemEntity>().Where(x => x.LastUpdated > lastUpdate).ToList().ForEach(
                    x =>
                    {
                        SendItemsEntity item = new SendItemsEntity
                        {
                            Login = login,
                            EntityName = EntityName.BrandItemEntity,
                            EntityId = x.Id,
                            RequestDate = lastUpdate
                        };

                        dataService.DataBaseContext.SendItemsEntities.Add(item);
                        count++;

                        if (count == 100)
                        {
                            dataService.DataBaseContext.SaveChanges();
                            count = 0;
                        }
                    });

                dataService.DataBaseContext.SaveChanges();
            }
        }

        #endregion
    }
}
