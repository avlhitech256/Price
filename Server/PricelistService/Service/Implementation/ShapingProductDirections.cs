using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using DataBase.Service;
using Option.Service;
using PricelistService.Service.Contract;
using PricelistService.Service.Objects;

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
                .Include(x => x.Contragent)
                .Where(x => x.Contragent.Login == login)
                .Where(x => x.EntityName == EntityName.ProductDirectionEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();

            dataService.DeleteEntities(photosToDelete);
        }

        private List<ProductDirectionInfo> GetProductDirectionInfos(string login)
        {
            //List<long> productDirectionIds = dataService.Select<SendItemsEntity>()
            //    .Include(x => x.Contragent)
            //    .Where(x => x.Contragent.Login == login)
            //    .Where(x => x.EntityName == EntityName.ProductDirectionEntity)
            //    .Take(optionService.CountSendItems)
            //    .Select(x => x.EntityId)
            //    .ToList();

            //List<ProductDirectionInfo> result =
            //    dataService.Select<ProductDirectionEntity>()
            //        .Where(x => productDirectionIds.Contains(x.Id))
            //        .Select(Assemble)
            //        .ToList();

            //return result;

            var loginParametr = new SqlParameter();
            loginParametr.ParameterName = "@login";
            loginParametr.SqlDbType = SqlDbType.NVarChar;
            loginParametr.Value = login;
            loginParametr.Direction = ParameterDirection.Input;

            var countToUpdateParametr = new SqlParameter();
            countToUpdateParametr.ParameterName = "@countToUpdate";
            countToUpdateParametr.SqlDbType = SqlDbType.BigInt;
            countToUpdateParametr.Direction = ParameterDirection.Input;
            countToUpdateParametr.Value = optionService.CountSendItems;

            List<DbProductDirectionInfo> productDirectionInfos = null;

            try
            {
                productDirectionInfos = dataService.DataBaseContext.Database
                    .SqlQuery<DbProductDirectionInfo>("GetProductDirectionItems @login, @countToUpdate",
                        loginParametr, countToUpdateParametr).ToList();
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }

            var result = new List<ProductDirectionInfo>();

            if (productDirectionInfos != null && productDirectionInfos.Any())
            {
                productDirectionInfos.ForEach(
                    x =>
                    {
                        ProductDirectionInfo productDirectionInfo = Assemble(x);

                        if (productDirectionInfo != null)
                        {
                            result.Add(productDirectionInfo);
                        }
                    });
            }

            return result;

        }

        private ProductDirectionInfo Assemble(DbProductDirectionInfo item)
        {
            ProductDirectionInfo result = null;

            if (item != null)
            {
                result = new ProductDirectionInfo
                {
                    Id = item.Id,
                    Direction = item.Direction,
                    DirectoryId = item.DirectoryId,
                    DateOfCreation = item.DateOfCreation,
                    LastUpdated = item.LastUpdated,
                    ForceUpdated = item.ForceUpdated
                };
            }

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

            try
            {
                dataService.DataBaseContext.Database
                    .ExecuteSqlCommand("PrepareToUpdateProductDirections @login, @lastUpdate, @countToUpdate",
                        loginParametr, lastUpdateParametr, countToUpdateParametr);
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }

            try
            {
            count = dataService.DataBaseContext.SendItemsEntities
                .Include(x => x.Contragent)
                .Count(x => x.EntityName == EntityName.ProductDirectionEntity && x.Contragent.Login == login);
            }
            catch (Exception e)
            {
                ;//TODO Записать в LOG-file ошибку
            }

            return count;
        }

        private long RemainderToUpdate(string login)
        {
            return dataService.Select<SendItemsEntity>()
                .Include(x => x.Contragent)
                .Count(x => x.Contragent.Login == login && x.EntityName == EntityName.ProductDirectionEntity);
        }

        #endregion
    }
}
