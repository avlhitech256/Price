using System.Data.Entity;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;

namespace DatabaseService.DataService.Implementation
{
    public class DataService : IDataService
    {
        public DataService(bool showLog = false)
        {
            DataBaseContext = CreateDataBaseContext();
        }

        public DataBaseContext.DataBaseContext DataBaseContext { get; }

        public IQueryable<TEntity> Select<TEntity>() where TEntity : class
        {
            // Загрузка данных с помощью универсального метода Set
            return DataBaseContext.Set<TEntity>();
        }

        public void LoadPhotos(CatalogItemEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void LoadPhotos(OrderItemEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void LoadPhotos(BasketItemEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void AddPhoto(CatalogItemEntity entity, byte[] photo)
        {
            PhotoItemEntity photoEntity = new PhotoItemEntity
            {
                Photo = photo
            };

            DataBaseContext.PhotoItemEntities.Add(photoEntity);
            entity.Photos.Add(photoEntity);

            DataBaseContext.SaveChanges();
        }

        private DataBaseContext.DataBaseContext CreateDataBaseContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DataBaseContext.DataBaseContext>());
            DataBaseContext.DataBaseContext dataBaseContext = new DataBaseContext.DataBaseContext();

            // Здесь мы можем указывать различные настройки контекста,
            // например выводить в отладчик сгенерированный SQL-код
            dataBaseContext.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            return dataBaseContext;
        }
    }
}
