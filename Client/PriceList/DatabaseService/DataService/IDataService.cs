using System.Collections.Generic;
using System.Linq;
using Common.Data.Enum;
using DatabaseService.DataBaseContext.Entities;

namespace DatabaseService.DataService
{
    public interface IDataService
    {
        DataBaseContext.DataBaseContext DataBaseContext { get; }

        IQueryable<TEntity> Select<TEntity>() where TEntity : class;

        void Insert<TEntity>(TEntity entity) where TEntity : class;

        void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        void DeleteEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void LoadPhotos(CatalogItemEntity entity);

        void LoadPhotos(BasketItemEntity entity);

        void LoadParent(DirectoryEntity entity);

        void AddPhoto(CatalogItemEntity entity, byte[] photo);

        decimal GetCount(CatalogItemEntity entity);

        void SetCount(CatalogItemEntity entity, decimal count);

        void SetOrderStatus(OrderEntity order, OrderStatus status);

        bool ExistOption(string optionCode);

        decimal GetSumBasket();

        string GetOption(string optionCode);

        void SetOption(string optionCode, string value);

        void CalculateOrderSum(BasketItemEntity basketItem);
    }
}
