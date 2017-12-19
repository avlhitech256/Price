using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using DataBase.Service;
using Option.Service;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    public class ShapingProductDirections : IShapingProductDirections
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public ShapingProductDirections(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
        }

        #endregion

        #region Methods

        public ProductDirectionInfo GetItem(long id)
        {
            return Assemble(dataService.DataBaseContext.ProductDirectionEntities.Find(id));
        }

        public ProductDirections GetItems(string login, DateTimeOffset lastUpdate)
        {
            long count = RemainderToUpdate(login);

            if (count == 0L)
            {
                count = PrepareToUpdate(login, lastUpdate);
            }

            ProductDirections result = new ProductDirections
            {
                Count = count,
                Items = GetProductDirectionInfos(login)
            };

            return result;
        }

        public void ConfirmUpdate(string login, List<long> itemIds)
        {
            List<SendItemsEntity> photosToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.ProductDirectionEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();

            dataService.DeleteEntities(photosToDelete);
        }

        private List<ProductDirectionInfo> GetProductDirectionInfos(string login)
        {
            List<long> productDirectionIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.ProductDirectionEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<ProductDirectionInfo> result =
                dataService.Select<ProductDirectionEntity>()
                    .Where(x => productDirectionIds.Contains(x.Id))
                    .Select(Assemble)
                    .ToList();

            return result;
        }

        private ProductDirectionInfo Assemble(ProductDirectionEntity item)
        {
            ProductDirectionInfo result = null;

            if (item != null)
            {
                result = new ProductDirectionInfo
                {
                    Id = item.Id,
                    Direction = item.Direction,
                    DirectoryId = item.Directory.Id,
                    DateOfCreation = item.DateOfCreation,
                    ForceUpdated = item.ForceUpdated,
                    LastUpdated = item.LastUpdated
                };
            }

            return result;
        }

        public long PrepareToUpdate(string login, DateTimeOffset lastUpdate)
        {
            var loginParametr = new SqlParameter();
            loginParametr.ParameterName = "@login";
            loginParametr.SqlDbType = SqlDbType.NVarChar;
            loginParametr.Value = login;
            loginParametr.Direction = ParameterDirection.Input;

            var lastUpdateParametr = new SqlParameter();
            lastUpdateParametr.ParameterName = "@lastUpdate";
            lastUpdateParametr.SqlDbType = SqlDbType.DateTimeOffset;
            lastUpdateParametr.Value = lastUpdate;
            lastUpdateParametr.Direction = ParameterDirection.Input;

            int count = 0;
            var countToUpdateParametr = new SqlParameter();
            countToUpdateParametr.ParameterName = "@countToUpdate";
            countToUpdateParametr.SqlDbType = SqlDbType.BigInt;
            countToUpdateParametr.Direction = ParameterDirection.Output;
            countToUpdateParametr.Value = count;

            dataService.DataBaseContext.Database
                .ExecuteSqlCommand("PrepareToUpdateProductDirections @login, @lastUpdate, @countToUpdate",
                                   loginParametr, lastUpdateParametr, countToUpdateParametr);

            count = dataService.DataBaseContext.SendItemsEntities
                .Count(x => x.EntityName == EntityName.ProductDirectionEntity && x.Login == login);

            return count;
        }

        private long RemainderToUpdate(string login)
        {
            return dataService.Select<SendItemsEntity>()
                .Count(x => x.Login == login && x.EntityName == EntityName.ProductDirectionEntity);
        }

        #endregion
    }
}
