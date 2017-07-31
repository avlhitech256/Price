using System.Linq;
using DatabaseService.DataBaseContext.Entities;

namespace DatabaseService.DataService
{
    public interface IDataService
    {
        DataBaseContext.DataBaseContext DataBaseContext { get; }

        IQueryable<TEntity> Select<TEntity>() where TEntity : class;

        void LoadPhotos(CatalogItemEntity entity);

        void LoadPhotos(OrderItemEntity entity);

        void LoadPhotos(BasketItemEntity entity);

        void AddPhoto(CatalogItemEntity entity, byte[] photo);
    }
}
