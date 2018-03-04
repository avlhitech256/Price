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
            BrandItemEntity entity = dataService.DataBaseContext.BrandItemEntities.Find(id);
            BrandInfo result = Assemble(entity);
            return result;
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
            List<SendItemsEntity> brandsToDelete = dataService.Select<SendItemsEntity>()
                .Include(x => x.Contragent)
                .Where(x => x.Contragent.Login == login)
                .Where(x => x.EntityName == EntityName.BrandItemEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();
            dataService.DeleteEntities(brandsToDelete);
        }

        private List<BrandInfo> GetBrandInfos(string login)
        {
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

            List<DbBrandInfo> brandInfos = null;

            try
            {
                brandInfos = dataService.DataBaseContext.Database
                    .SqlQuery<DbBrandInfo>("GetBrandItems @login, @countToUpdate",
                        loginParametr, countToUpdateParametr).ToList();
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }

            var result = new List<BrandInfo>();

            if (brandInfos != null && brandInfos.Any())
            {
                brandInfos.ForEach(
                    x =>
                    {
                        BrandInfo brandInfo = Assemble(x);

                        if (brandInfo != null)
                        {
                            result.Add(brandInfo);
                        }
                    });
            }

            return result;
        }

        private BrandInfo Assemble(DbBrandInfo item)
        {
            BrandInfo result = null;

            if (item != null)
            {
                result = new BrandInfo
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    DateOfCreation = item.DateOfCreation,
                    LastUpdated = item.LastUpdated,
                    ForceUpdated = item.ForceUpdated
                };
            }

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
                    DateOfCreation = item.DateOfCreation,
                    ForceUpdated = item.ForceUpdated,
                    LastUpdated = item.LastUpdated
                };
            }

            return result;
        }

        public int PrepareToUpdate(string login, DateTimeOffset lastUpdate)
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
                    .ExecuteSqlCommand("PrepareToUpdateBrands @login, @lastUpdate, @countToUpdate",
                        loginParametr, lastUpdateParametr, countToUpdateParametr);
            }
            catch (Exception e)
            {
                ;//TODO Записать в LOG-file ошибку
            }

            try
            {
                count = dataService.DataBaseContext.SendItemsEntities
                .Include(x => x.Contragent)
                .Count(x => x.EntityName == EntityName.BrandItemEntity && x.Contragent.Login == login);
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
                .Count(x => x.Contragent.Login == login && x.EntityName == EntityName.BrandItemEntity);
        }

        #endregion
    }
}
