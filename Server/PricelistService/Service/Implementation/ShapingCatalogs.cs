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
using PricelistService.Service.Objects;

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
                .Include(x => x.Contragent)
                .Where(x => x.Contragent.Login == login)
                .Where(x => x.EntityName == EntityName.CatalogItemEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();
            dataService.DeleteEntities(brandsToDelete);
        }

        private List<CatalogInfo> GetCatalogInfos(string login)
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

            List<DbCatalogInfo> catalogInfos = null;

            try
            {
                catalogInfos = dataService.DataBaseContext.Database
                    .SqlQuery<DbCatalogInfo>("GetCatalogItems @login, @countToUpdate",
                        loginParametr, countToUpdateParametr).ToList();
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }

            List<CatalogInfo> result = new List<CatalogInfo>();

            if (catalogInfos != null && catalogInfos.Any())
            {
                DataTable ctalogIds = new DataTable();
                ctalogIds.Columns.Add("Id", typeof(long));
                catalogInfos.ForEach(x => ctalogIds.Rows.Add(x.Id));

                var idsParametr = new SqlParameter();
                idsParametr.ParameterName = "@ids";
                idsParametr.SqlDbType = SqlDbType.Structured;
                idsParametr.TypeName = "bigintTable";
                idsParametr.Value = ctalogIds;
                idsParametr.Direction = ParameterDirection.Input;

                List<DbPhotoIdInfo> photoIds = new List<DbPhotoIdInfo>();

                try
                {
                    photoIds = dataService.DataBaseContext.Database
                        .SqlQuery<DbPhotoIdInfo>("GetPhotosIds @ids", idsParametr).ToList();
                }
                catch (Exception e)
                {
                    ; //TODO Записать в LOG-file ошибку
                }

                catalogInfos.ForEach(
                    x =>
                    {
                        CatalogInfo catalogInfo = Assemble(x, photoIds);
                        if (catalogInfo != null)
                        {
                            result.Add(catalogInfo);
                        }
                    });
            }

            return result;
        }

        private CatalogInfo Assemble(DbCatalogInfo dbCatalogInfo, List<DbPhotoIdInfo> photoIds)
        {
            CatalogInfo result = null;

            if (dbCatalogInfo != null && photoIds != null)
            {
                List<long> photos = photoIds.Where(x => x.CatalogId == dbCatalogInfo.Id).Select(x => x.PhotoId).ToList();

                result = new CatalogInfo
                {
                    IsAuthorized = true,
                    Id = dbCatalogInfo.Id,
                    UID = dbCatalogInfo.UID,
                    Code = dbCatalogInfo.Code,
                    Article = dbCatalogInfo.Article,
                    Name = dbCatalogInfo.Name,
                    BrandId = dbCatalogInfo.BrandId,
                    BrandName = dbCatalogInfo.BrandName,
                    Unit = dbCatalogInfo.Unit,
                    EnterpriceNormPack = dbCatalogInfo.EnterpriceNormPack,
                    BatchOfSales = dbCatalogInfo.BatchOfSales,
                    Balance = dbCatalogInfo.Balance,
                    Price = dbCatalogInfo.Price,
                    Currency = dbCatalogInfo.Currency,
                    Multiplicity = dbCatalogInfo.Multiplicity,
                    HasPhotos = dbCatalogInfo.HasPhotos,
                    Photos = photos,
                    DateOfCreation = dbCatalogInfo.DateOfCreation,
                    LastUpdated = dbCatalogInfo.LastUpdated,
                    ForceUpdated = dbCatalogInfo.ForceUpdated,
                    Status = dbCatalogInfo.Status,
                    LastUpdatedStatus = dbCatalogInfo.LastUpdatedStatus,
                    DirectoryId = dbCatalogInfo.DirectoryId
                };
            }

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
                    .Include(x => x.Contragent)
                    .Count(x => x.EntityName == EntityName.CatalogItemEntity && x.Contragent.Login == login);
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
                .Include(x => x.Contragent)
                .Count(x => x.Contragent.Login == login && x.EntityName == EntityName.CatalogItemEntity);
        }

        #endregion
    }
}
