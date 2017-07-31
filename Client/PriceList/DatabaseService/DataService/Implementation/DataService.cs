using System.Data.Entity;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;

namespace DatabaseService.DataService.Implementation
{
    public class DataService : IDataService
    {
        public DataService(bool showLog = false)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DataBaseContext.DataBaseContext>());
            DataBaseContext = new DataBaseContext.DataBaseContext();
        }

        public DataBaseContext.DataBaseContext DataBaseContext { get; }

        public IQueryable<TEntity> Select<TEntity>() where TEntity : class
        {
            DataBaseContext.DataBaseContext dataBaseContext = CreateDataBaseContext();

            // Загрузка данных с помощью универсального метода Set
            return dataBaseContext.Set<TEntity>();
        }

        public void LoadPhotos(CatalogItemEntity entity)
        {
            DataBaseContext.DataBaseContext dataBaseContext = CreateDataBaseContext();

            if (!dataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                dataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void LoadPhotos(OrderItemEntity entity)
        {
            DataBaseContext.DataBaseContext dataBaseContext = CreateDataBaseContext();

            if (!dataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                dataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void LoadPhotos(BasketItemEntity entity)
        {
            DataBaseContext.DataBaseContext dataBaseContext = CreateDataBaseContext();

            if (!dataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                dataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void AddPhoto(CatalogItemEntity entity, byte[] photo)
        {
            DataBaseContext.DataBaseContext dataBaseContext = CreateDataBaseContext();

            PhotoItemEntity photoEntity = new PhotoItemEntity
            {
                Photo = photo
            };

            dataBaseContext.PhotoItemEntities.Add(photoEntity);
            entity.Photos.Add(photoEntity);

            dataBaseContext.SaveChanges();
        }

        private DataBaseContext.DataBaseContext CreateDataBaseContext()
        {
            DataBaseContext.DataBaseContext dataBaseContext = new DataBaseContext.DataBaseContext();

            // Здесь мы можем указывать различные настройки контекста,
            // например выводить в отладчик сгенерированный SQL-код
            dataBaseContext.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            return dataBaseContext;
        }
    }
}
