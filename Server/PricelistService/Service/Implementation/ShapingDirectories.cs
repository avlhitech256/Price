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
            IEnumerable<SendItemsEntity> brandsToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.DirectoryEntity)
                .Where(x => itemIds.Contains(x.Id))
                .AsEnumerable();

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
                    Parent = item.Parent.Id,
                    SubDirectoryId = item.SubDirectory.Select(x => x.Id).ToList(),
                    CatalogId = item.CatalogItems.Select(x => x.Id).ToList(),
                    DateOfCreation = item.DateOfCreation,
                    ForceUpdated = item.ForceUpdated,
                    LastUpdated = item.LastUpdated
                };
            }

            return result;
        }

        private void ShapingBrandsList(string login, DateTimeOffset lastUpdate)
        {
            if (!dataService.Select<SendItemsEntity>()
                    .Any(x => x.RequestDate >= lastUpdate && 
                              x.Login == login && 
                              x.EntityName == EntityName.DirectoryEntity))
            {
                int count = 0;

                List<long> directoryIds = dataService.Select<SendItemsEntity>()
                    .Where(x => x.Login == login)
                    .Where(x => x.EntityName == EntityName.DirectoryEntity)
                    .Select(x => x.Id)
                    .ToList();

                dataService.Select<DirectoryEntity>()
                    .Where(x => x.LastUpdated > lastUpdate)
                    .Where(x => !directoryIds.Contains(x.Id))
                    .ToList()
                    .ForEach(
                        x =>
                        {
                            SendItemsEntity item = new SendItemsEntity
                            {
                                Login = login,
                                EntityName = EntityName.DirectoryEntity,
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
                .Count(x => x.Login == login && x.EntityName == EntityName.DirectoryEntity);
        }

        #endregion
    }
}
