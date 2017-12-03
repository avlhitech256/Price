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

        public Photos GetItems(string login, DateTimeOffset lastUpdate)
        {
            long count = RemainderToUpdate(login);

            if (count == 0L)
            {
                count = PrepareToUpdate(login, lastUpdate);
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
            IEnumerable<SendItemsEntity> photosToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.PhotoItemEntity)
                .Where(x => itemIds.Contains(x.Id))
                .AsEnumerable();

            dataService.DeleteEntities(photosToDelete);
        }

        private List<PhotoInfo> GetPhotoInfos(string login)
        {
            List<long> catalogIds = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.PhotoItemEntity)
                .Take(optionService.CountSendItems)
                .Select(x => x.EntityId)
                .ToList();

            List<PhotoInfo> result =
                dataService.Select<PhotoItemEntity>()
                    .Where(x => catalogIds.Contains(x.Id))
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

        private void ShapingPhotosList(string login, DateTimeOffset lastUpdate)
        {
            if (!dataService.Select<SendItemsEntity>()
                    .Any(x => x.RequestDate >= lastUpdate && 
                              x.Login == login && 
                              x.EntityName == EntityName.PhotoItemEntity))
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
            ShapingPhotosList(login, lastUpdate);
            return RemainderToUpdate(login);
        }

        private long RemainderToUpdate(string login)
        {
            return dataService.Select<SendItemsEntity>()
                .Count(x => x.Login == login && x.EntityName == EntityName.PhotoItemEntity);
        }

        #endregion
    }
}
