using System;
using System.Collections.Generic;
using System.Linq;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using DataBase.Service;
using Option.Service;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    public class ShapingCatalogs : IShapingCatalogs
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public ShapingCatalogs(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
        }

        #endregion

        #region Methods

        public CatalogInfo GetItem(long id)
        {
            return Assemble(dataService.DataBaseContext.CatalogItemEntities.Find(id));
        }

        public Catalogs GetItems(string login, DateTimeOffset lastUpdate)
        {
            long count = RemainderToUpdate(login);

            if (count == 0L)
            {
                count = PrepareToUpdate(login, lastUpdate);
            }

            Catalogs result = new Catalogs
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
                .Where(x => x.EntityName == EntityName.CatalogItemEntity)
                .Where(x => itemIds.Contains(x.Id))
                .AsEnumerable();
            dataService.DeleteEntities(brandsToDelete);
        }

        private List<CatalogInfo> GetBrandInfos(string login)
        {
            List<long> catalogIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.CatalogItemEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<CatalogInfo> result =
                dataService.Select<CatalogItemEntity>()
                    .Where(x => catalogIds.Contains(x.Id))
                    .Select(Assemble)
                    .ToList();

            return result;
        }

        private CatalogInfo Assemble(CatalogItemEntity item)
        {
            CatalogInfo result = null;

            if (item != null)
            {
                result = new CatalogInfo
                {
                    Id = item.Id,
                    UID = item.UID,
                    Code = item.Code,
                    Article = item.Article,
                    Name = item.Name,
                    BrandId = item.Brand.Id,
                    BrandName = item.BrandName,
                    Unit = item.Unit,
                    EnterpriceNormPack = item.EnterpriceNormPack,
                    BatchOfSales = item.BatchOfSales,
                    Balance = item.Balance,
                    Price = item.Price,
                    Currency = item.Currency,
                    Multiplicity = item.Multiplicity,
                    HasPhotos = item.HasPhotos,
                    Photos = item.Photos != null ? item.Photos.Select(x => x.Id).ToList() : new List<long>(),
                    DateOfCreation = item.DateOfCreation,
                    LastUpdated = item.LastUpdated,
                    ForceUpdated = item.ForceUpdated,
                    Status = item.Status,
                    LastUpdatedStatus = item.LastUpdatedStatus,
                    DirectoryId = item.Directory.Id
                };
            }

            return result;
        }

        private void ShapingBrandsList(string login, DateTimeOffset lastUpdate)
        {
            if (!dataService.Select<SendItemsEntity>()
                    .Any(x => x.RequestDate >= lastUpdate &&
                              x.Login == login &&
                              x.EntityName == EntityName.CatalogItemEntity))
            {
                int count = 0;
                List<long> catalogIds = dataService.Select<SendItemsEntity>()
                    .Where(x => x.Login == login)
                    .Where(x => x.EntityName == EntityName.CatalogItemEntity)
                    .Select(x => x.Id)
                    .ToList();

                dataService.Select<CatalogItemEntity>()
                    .Where(x => x.LastUpdated > lastUpdate)
                    .Where(x => !catalogIds.Contains(x.Id))
                    .ToList()
                    .ForEach(
                        x =>
                        {
                            SendItemsEntity item = new SendItemsEntity
                            {
                                Login = login,
                                EntityName = EntityName.CatalogItemEntity,
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
                .Count(x => x.Login == login && x.EntityName == EntityName.CatalogItemEntity);
        }

        #endregion
    }
}
