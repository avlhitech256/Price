using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using DataBase.Objects;
using DataBase.Service;
using Option.Service;
using Price.Service;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    public class ShapingCatalogs : IShapingCatalogs
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;
        private readonly IPriceService priceService;

        #endregion

        #region Constructors

        public ShapingCatalogs(IDataService dataService, IOptionService optionService, IPriceService priceService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
            this.priceService = priceService;
        }

        #endregion

        #region Methods

        public CatalogInfo GetItem(string login, long id)
        {
            return Assemble(dataService.DataBaseContext.CatalogItemEntities.Find(id), login);
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
                Items = GetCatalogInfos(login)
            };

            return result;
        }

        public void ConfirmUpdate(string login, List<long> itemIds)
        {
            List<SendItemsEntity> brandsToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.CatalogItemEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();
            dataService.DeleteEntities(brandsToDelete);
        }

        private List<CatalogInfo> GetCatalogInfos(string login)
        {
            List<long> catalogIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.CatalogItemEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<CatalogInfo> result =
                dataService.Select<CatalogItemEntity>()
                    .Include(x => x.Discounts)
                    .Include(x => x.PriceGroup)
                    .Include(x => x.NomenclatureGroup)
                    .Where(x => catalogIds.Contains(x.Id))
                    .ToList()
                    .Select(x => Assemble(x, login))
                    .ToList();

            return result;
        }

        private CatalogInfo Assemble(CatalogItemEntity item, string login)
        {
            CatalogInfo result = null;

            if (item != null)
            {
                PriceInfo priceInfo = priceService.GetPrice(item, login);

                result = new CatalogInfo
                {
                    Id = item.Id,
                    UID = item.UID,
                    Code = item.Code,
                    Article = item.Article,
                    Name = item.Name,
                    BrandId = item.Brand?.Id,
                    BrandName = item.BrandName,
                    Unit = item.Unit,
                    EnterpriceNormPack = item.EnterpriceNormPack,
                    BatchOfSales = item.BatchOfSales,
                    Balance = item.Balance,
                    Price = priceInfo.Price,
                    Currency = priceInfo.Currency,
                    Multiplicity = item.Multiplicity,
                    HasPhotos = item.HasPhotos,
                    Photos = item.Photos?.Select(x => x.Id).ToList() ?? new List<long>(),
                    DateOfCreation = item.DateOfCreation,
                    LastUpdated = item.LastUpdated,
                    ForceUpdated = item.ForceUpdated,
                    Status = item.Status,
                    LastUpdatedStatus = item.LastUpdatedStatus,
                    DirectoryId = item.Directory?.Id
                };
            }

            return result;
        }

        public long PrepareToUpdate(string login, DateTimeOffset lastUpdate)
        {
            var loginParametr = new SqlParameter
            {
                ParameterName = "@login",
                SqlDbType = SqlDbType.NVarChar,
                Value = login,
                Direction = ParameterDirection.Input
            };

            var lastUpdateParametr = new SqlParameter
            {
                ParameterName = "@lastUpdate",
                SqlDbType = SqlDbType.DateTimeOffset,
                Value = lastUpdate,
                Direction = ParameterDirection.Input
            };

            int count = 0;
            var countToUpdateParametr = new SqlParameter
            {
                ParameterName = "@countToUpdate",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Output,
                Value = count
            };

            try
            {
                dataService.DataBaseContext.Database
                    .ExecuteSqlCommand("PrepareToUpdateCatalogs @login, @lastUpdate, @countToUpdate",
                        loginParametr, lastUpdateParametr, countToUpdateParametr);
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }

            try
            {
                count = dataService.DataBaseContext.SendItemsEntities
                    .Count(x => x.EntityName == EntityName.CatalogItemEntity && x.Login == login);
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }


            return count;
        }

        private long RemainderToUpdate(string login)
        {
            return dataService.Select<SendItemsEntity>()
                .Count(x => x.Login == login && x.EntityName == EntityName.CatalogItemEntity);
        }

        #endregion
    }
}
