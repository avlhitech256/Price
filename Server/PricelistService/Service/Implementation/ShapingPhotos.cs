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

namespace PricelistService.Service.Implementation
{
    public class ShapingPhotos : IShapingPhotos
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public ShapingPhotos(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
        }

        #endregion

        #region Methods

        public PhotoInfo GetItem(long id)
        {
            return Assemble(dataService.DataBaseContext.PhotoItemEntities.Find(id));
        }

        public Photos GetItems(string login, DateTimeOffset lastUpdate, long[] ids)
        {
            long count = RemainderToUpdate(login);

            if (count == 0L)
            {
                count = PrepareToUpdate(login, lastUpdate, ids);
            }

            Photos result = new Photos
            {
                Count = count,
                Items = GetPhotoInfos(login)
            };

            return result;
        }

        public void ConfirmUpdate(string login, List<long> itemIds)
        {
            List<SendItemsEntity> photosToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.PhotoItemEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();

            dataService.DeleteEntities(photosToDelete);
        }

        private List<PhotoInfo> GetPhotoInfos(string login)
        {
            List<long> photosIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.PhotoItemEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<PhotoInfo> result =
                dataService.Select<PhotoItemEntity>()
                    .Include(x => x.CatalogItem)
                    .Where(x => photosIds.Contains(x.Id))
                    .Select(Assemble)
                    .ToList();

            return result;
        }

        private PhotoInfo Assemble(PhotoItemEntity item)
        {
            PhotoInfo result = null;

            if (item != null)
            {
                result = new PhotoInfo
                {
                    Id = item.Id,
                    Name = item.Name,
                    IsLoad = item.IsLoad,
                    Photo = item.Photo,
                    CatalogId = item.CatalogItem.Id,
                    DateOfCreation = item.DateOfCreation,
                    ForceUpdated = item.ForceUpdated,
                    LastUpdated = item.LastUpdated
                };
            }

            return result;
        }

        public long PrepareToUpdate(string login, DateTimeOffset lastUpdate, long[] ids)
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

            DataTable data = new DataTable();
            data.Columns.Add("Id", typeof(long));

            foreach (long r in ids)
            {
                data.Rows.Add(r);
            }

            var idsParametr = new SqlParameter();
            idsParametr.ParameterName = "@ids";
            idsParametr.SqlDbType = SqlDbType.Structured;
            idsParametr.TypeName = "bigintTable";
            idsParametr.Value = data;
            idsParametr.Direction = ParameterDirection.Input;

            int count = 0;
            var countToUpdateParametr = new SqlParameter();
            countToUpdateParametr.ParameterName = "@countToUpdate";
            countToUpdateParametr.SqlDbType = SqlDbType.BigInt;
            countToUpdateParametr.Direction = ParameterDirection.Output;
            countToUpdateParametr.Value = count;

            dataService.DataBaseContext.Database
                .ExecuteSqlCommand("PrepareToUpdatePhotos @login, @lastUpdate, @ids, @countToUpdate",
                                   loginParametr, lastUpdateParametr, idsParametr, countToUpdateParametr);

            count = dataService.DataBaseContext.SendItemsEntities
                .Count(x => x.EntityName == EntityName.PhotoItemEntity && x.Login == login);

            return count;
        }

        private long RemainderToUpdate(string login)
        {
            return dataService.Select<SendItemsEntity>()
                .Count(x => x.Login == login && x.EntityName == EntityName.PhotoItemEntity);
        }

        #endregion
    }
}
