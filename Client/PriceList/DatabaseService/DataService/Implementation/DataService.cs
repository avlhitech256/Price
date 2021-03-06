﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Common.Data.Enum;
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

        private DataBaseContext.DataBaseContext CreateDataBaseContext()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DataBaseContext.DataBaseContext>());
            DataBaseContext.DataBaseContext dataBaseContext = new DataBaseContext.DataBaseContext();

            // Здесь мы можем указывать различные настройки контекста,
            // например выводить в отладчик сгенерированный SQL-код
            dataBaseContext.Database.Log = (s => System.Diagnostics.Debug.WriteLine(s));

            return dataBaseContext;
        }

        public TEntity Find<TEntity>(object id) where TEntity : class
        {
            // Поиск сущности по первичному ключу с помощью универсального метода Set
            return DataBaseContext.Set<TEntity>().Find(id);
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

        public void InsertMany<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
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

        public void LoadPhotos(BasketItemEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Collection(c => c.CatalogItem.Photos).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.CatalogItem.Photos).Load();
            }
        }

        public void LoadParent(DirectoryEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Reference(p => p.Parent).IsLoaded)
            {
                DataBaseContext.Entry(entity).Reference(p => p.Parent).Load();
            }
        }

        public void LoadBasket(CatalogItemEntity entity)
        {
            if (!DataBaseContext.Entry(entity).Collection(c => c.BasketItems).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.BasketItems).Load();
            }
        }

        public void LoadBasket(OrderEntity entity)
        {
            if (entity != null && !DataBaseContext.Entry(entity).Collection(c => c.BasketItems).IsLoaded)
            {
                DataBaseContext.Entry(entity).Collection(c => c.BasketItems).Load();
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

        private BasketItemEntity GetBasketEntity(CatalogItemEntity entity)
        {
            LoadBasket(entity);
            BasketItemEntity basketItem = entity.BasketItems.FirstOrDefault(x => x.Order == null);
            return basketItem;
        }
        public decimal GetCount(CatalogItemEntity entity)
        {
            decimal count = entity != null ? GetBasketEntity(entity)?.Count ?? 0 : 0;
            return count;
        }

        public void SetCount(CatalogItemEntity entity, decimal count)
        {
            BasketItemEntity basketItem = GetBasketEntity(entity);

            if (count == 0)
            {
                if (basketItem != null)
                {
                    Delete(basketItem);
                }
            }
            else
            {
                if (basketItem == null)
                {
                    basketItem = new BasketItemEntity();
                    entity.BasketItems.Add(basketItem);
                }

                basketItem.Count = count;
                basketItem.DateAction = DateTimeOffset.Now;
                DataBaseContext.SaveChanges();
            }
        }

        public void SetOrderStatus(OrderEntity order, OrderStatus status)
        {
            order.OrderStatus = OrderStatus.SentOut;
            DataBaseContext.SaveChanges();
        }

        public decimal GetSumBasket()
        {
            decimal sum = Select<BasketItemEntity>()
                .Include(x => x.CatalogItem)
                .Where(x => x.Order == null)
                .Where(x => x.CatalogItem != null)
                .ToList()
                .Select(x => x.Count*x.CatalogItem.Price)
                .Sum();
            return sum;
        }

        public void CalculateOrderSum(BasketItemEntity basketItem)
        {
            OrderEntity order = basketItem?.Order;

            if (order != null)
            {
                LoadBasket(order);
                order.Sum = order.BasketItems.Sum(x => x.Count*x.CatalogItem.Price);
                DataBaseContext.SaveChanges();
            }
        }

        public DateTimeOffset GetLastUpdate()
        {
            DateTimeOffset lastUpdate = DateTimeOffset.MinValue;
            List<DateTimeOffset> dates = new List<DateTimeOffset>
            {
                GetLastUpdateBrands(),
                GetLastUpdateCatalogs(),
                GetLastUpdateDirectories(),
                GetLastUpdatePhotos(),
                GetLastUpdateProductDirections()
            };

            lastUpdate = dates.Max();
            return lastUpdate;
        }

        private DateTimeOffset GetLastUpdateBrands()
        {
            DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

            if (DataBaseContext.BrandItemEntities.Any())
            {
                lastUpdate = DataBaseContext.BrandItemEntities.Select(x => x.LastUpdated).Max();
            }

            return lastUpdate;
        }

        private DateTimeOffset GetLastUpdateCatalogs()
        {
            DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

            if (DataBaseContext.CatalogItemEntities.Any())
            {
                lastUpdate = DataBaseContext.CatalogItemEntities.Select(x => x.LastUpdated).Max();
            }

            return lastUpdate;
        }

        private DateTimeOffset GetLastUpdateDirectories()
        {
            DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

            if (DataBaseContext.DirectoryEntities.Any())
            {
                lastUpdate = DataBaseContext.DirectoryEntities.Select(x => x.LastUpdated).Max();
            }

            return lastUpdate;
        }

        private DateTimeOffset GetLastUpdatePhotos()
        {
            DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

            if (DataBaseContext.PhotoItemEntities.Any())
            {
                lastUpdate = DataBaseContext.PhotoItemEntities.Select(x => x.LastUpdated).Max();
            }

            return lastUpdate;
        }

        private DateTimeOffset GetLastUpdateProductDirections()
        {
            DateTimeOffset lastUpdate = DateTimeOffset.MinValue;

            if (DataBaseContext.ProductDirectionEntities.Any())
            {
                lastUpdate = DataBaseContext.ProductDirectionEntities.Select(x => x.LastUpdated).Max();
            }

            return lastUpdate;
        }
    }
}
