using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Data.Enum;
using DataBase.Context;
using DataBase.Context.Entities;

namespace DataBase.Service.Implementation
{
    public class DataService : IDataService
    {
        public DataService(bool showLog = false)
        {
            DataBaseContext = CreateDataBaseContext();
        }

        public DataBaseContext DataBaseContext { get; }

        private DataBaseContext CreateDataBaseContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DataBaseContext>());
            DataBaseContext dataBaseContext = new DataBaseContext();

            // Здесь мы можем указывать различные настройки контекста,
            // например выводить в отладчик сгенерированный SQL-код
            dataBaseContext.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            return dataBaseContext;
        }

        public IQueryable<TEntity> Select<TEntity>() where TEntity : class
        {
            // Загрузка данных с помощью универсального метода Set
            return DataBaseContext.Set<TEntity>();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            DataBaseContext.Entry(entity).State = EntityState.Added;
            DataBaseContext.SaveChanges();
        }

        public void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
            DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            foreach (TEntity entity in entities)
            {
                DataBaseContext.Entry(entity).State = EntityState.Added;
            }

            DataBaseContext.SaveChanges();

            DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
            DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            DataBaseContext.Entry(entity).State = EntityState.Deleted;
            DataBaseContext.SaveChanges();
        }

        public void DeleteEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            entities.ToList().ForEach(x => DataBaseContext.Entry(x).State = EntityState.Deleted);
            DataBaseContext.SaveChanges();
        }

        public void LoadPhotos(CatalogItemEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Collection(c => c.Photos).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.Photos).Load();
            }
        }

        public void LoadParent(DirectoryEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Reference(p => p.Parent).IsLoaded)
            {
                DataBaseContext.Entry(entity).Reference(p => p.Parent).Load();
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

        public void SetOrderStatus(OrderEntity order, OrderStatus status)
        {
            order.OrderStatus = OrderStatus.SentOut;
            DataBaseContext.SaveChanges();
        }
    }
}
