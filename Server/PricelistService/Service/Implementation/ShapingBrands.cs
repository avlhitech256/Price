﻿using System;
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

        public BrandInfo GetItem(long id)
        {
            return Assemble(dataService.DataBaseContext.BrandItemEntities.Find(id));
        }

        public Brands GetItems(string login, DateTimeOffset lastUpdate)
        {
            long count = RemainderToUpdate(login);

            if (count == 0L)
            {
                count = PrepareToUpdate(login, lastUpdate);
            }

            Brands result = new Brands
            {
                Count = count,
                Items = GetBrandInfos(login)
            };

            return result;
        }

        public void ConfirmUpdate(string login, List<long> itemIds)
        {
            IEnumerable<SendItemsEntity> brandsToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.BrandItemEntity)
                .Where(x => itemIds.Contains(x.Id))
                .AsEnumerable();
            dataService.DeleteEntities(brandsToDelete);
        }

        private List<BrandInfo> GetBrandInfos(string login)
        {
            List<long> brandIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.BrandItemEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<BrandInfo> result =
                dataService.Select<BrandItemEntity>()
                    .Where(x => brandIds.Contains(x.Id))
                    .Select(Assemble)
                    .ToList();

            return result;
        }

        private BrandInfo Assemble(BrandItemEntity item)
        {
            BrandInfo result = null;

            if (item != null)
            {
                result = new BrandInfo
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    CatalogId = item.CatalogItems.Select(x => x.Id).ToList()
                };
            }

            return result;
        }

        private void ShapingBrandsList(string login, DateTimeOffset lastUpdate)
        {
            if (dataService.Select<SendItemsEntity>().All(x => x.RequestDate != lastUpdate))
            {
                int count = 0;
                List<long> brandIds = dataService.Select<SendItemsEntity>()
                    .Where(x => x.Login == login)
                    .Where(x => x.EntityName == EntityName.BrandItemEntity)
                    .Select(x => x.Id)
                    .ToList();

                dataService.Select<BrandItemEntity>()
                    .Where(x => x.LastUpdated > lastUpdate)
                    .Where(x => !brandIds.Contains(x.Id))
                    .ToList()
                    .ForEach(
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

        public long PrepareToUpdate(string login, DateTimeOffset lastUpdate)
        {
            ShapingBrandsList(login, lastUpdate);
            return RemainderToUpdate(login);
        }

        private long RemainderToUpdate(string login)
        {
            return dataService.Select<SendItemsEntity>()
                .Count(x => x.Login == login && x.EntityName == EntityName.BrandItemEntity);
        }

        #endregion
    }
}
