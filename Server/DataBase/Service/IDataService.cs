﻿using System.Collections.Generic;
using System.Linq;
using Common.Data.Enum;
using DataBase.Context;
using DataBase.Context.Entities;

namespace DataBase.Service
{
    public interface IDataService
    {
        DataBaseContext DataBaseContext { get; }

        IQueryable<TEntity> Select<TEntity>() where TEntity : class;

        void Insert<TEntity>(TEntity entity) where TEntity : class;

        void Insert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        void DeleteEntities<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        void LoadPhotos(CatalogItemEntity entity);

        void LoadParent(DirectoryEntity entity);

        void AddPhoto(CatalogItemEntity entity, byte[] photo);

        void SetOrderStatus(OrderEntity order, OrderStatus status);
    }
}
