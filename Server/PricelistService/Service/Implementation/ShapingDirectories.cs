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
    public class ShapingDirectories : IShapingDirectories
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public ShapingDirectories(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
        }

        #endregion

        #region Methods

        public DirectoryInfo GetItem(long id)
        {
            return Assemble(dataService.DataBaseContext.DirectoryEntities.Find(id));
        }

        public Directories GetItems(string login, DateTimeOffset lastUpdate)
        {
            long count = RemainderToUpdate(login);

            if (count == 0L)
            {
                count = PrepareToUpdate(login, lastUpdate);
            }

            Directories result = new Directories
            {
                Count = count,
                Items = GetDirectoryInfos(login)
            };

            return result;
        }

        public void ConfirmUpdate(string login, List<long> itemIds)
        {
            List<SendItemsEntity> brandsToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.DirectoryEntity)
                .Where(x => itemIds.Contains(x.EntityId))
                .ToList();

            dataService.DeleteEntities(brandsToDelete);
        }

        private List<DirectoryInfo> GetDirectoryInfos(string login)
        {
            List<long> catalogIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.DirectoryEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<DirectoryInfo> result =
                dataService.Select<DirectoryEntity>()
                    .Include(x => x.Parent)
                    .Include(x => x.SubDirectory)
                    .Include(x => x.CatalogItems)
                    .Where(x => catalogIds.Contains(x.Id))
                    .Select(Assemble)
                    .ToList();

            return result;
        }

        private DirectoryInfo Assemble(DirectoryEntity item)
        {
            DirectoryInfo result = null;

            if (item != null)
            {
                result = new DirectoryInfo
                {
                    Id = item.Id,
                    Code = item.Code,
                    Name = item.Name,
                    Parent = item.Parent?.Id,
                    SubDirectoryIds = item.SubDirectory?.Select(x => x.Id).ToList() ?? new List<long>(),
                    CatalogId = item.CatalogItems?.Select(x => x.Id).ToList() ?? new List<long>(),
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
                    .ExecuteSqlCommand("PrepareToUpdateDirectories @login, @lastUpdate, @countToUpdate",
                        loginParametr, lastUpdateParametr, countToUpdateParametr);
            }
            catch (Exception e)
            {
                ; //TODO Записать в LOG-file ошибку
            }

            try
            {
                count = dataService.DataBaseContext.SendItemsEntities
                    .Count(x => x.EntityName == EntityName.DirectoryEntity && x.Login == login);
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
                .Count(x => x.Login == login && x.EntityName == EntityName.DirectoryEntity);
        }

        #endregion
    }
}
