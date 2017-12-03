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
            IEnumerable<SendItemsEntity> photosToDelete = dataService.Select<SendItemsEntity>()
                .Where(x => x.Login == login)
                .Where(x => x.EntityName == EntityName.ProductDirectionEntity)
                .Where(x => itemIds.Contains(x.Id))
                .AsEnumerable();

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

        private void ShapingPhotosList(string login, DateTimeOffset lastUpdate)
        {
            if (!dataService.Select<SendItemsEntity>()
                    .Any(x => x.RequestDate >= lastUpdate &&
                              x.Login == login &&
                              x.EntityName == EntityName.ProductDirectionEntity))
            {
                int count = 0;

                List<long> productDirectionIds = dataService.Select<SendItemsEntity>()
                    .Where(x => x.Login == login)
                    .Where(x => x.EntityName == EntityName.ProductDirectionEntity)
                    .Select(x => x.Id)
                    .ToList();

                dataService.Select<ProductDirectionEntity>()
                    .Where(x => x.LastUpdated > lastUpdate)
                    .Where(x => !productDirectionIds.Contains(x.Id))
                    .ToList()
                    .ForEach(
                        x =>
                        {
                            SendItemsEntity item = new SendItemsEntity
                            {
                                Login = login,
                                EntityName = EntityName.ProductDirectionEntity,
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
                .Count(x => x.Login == login && x.EntityName == EntityName.ProductDirectionEntity);
        }

        #endregion
    }
}
