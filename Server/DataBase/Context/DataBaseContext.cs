﻿using System;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Diagnostics;
using DataBase.Context.Entities;
using DataBase.Context.Initializer;

namespace DataBase.Context
{
    using System.Data.Entity;

    public class DataBaseContext : DbContext
    {
        // Контекст настроен для использования строки подключения "DataBaseContext" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "DataBase.Context.DataBaseContext" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "DataBaseContext" 
        // в файле конфигурации приложения.
        public DataBaseContext()
            : base("name=DataBaseContext")
        {
            Database.Log = (s => Debug.WriteLine(s));
            Database.SetInitializer(new DataBaseInitializer());
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<BrandItemEntity> BrandItemEntities { get; set; }

        public virtual DbSet<CatalogItemEntity> CatalogItemEntities { get; set; }

        public virtual DbSet<CommodityDirectionEntity> CommodityDirectionEntities { get; set; }

        public virtual DbSet<DirectoryEntity> DirectoryEntities { get; set; }

        public virtual DbSet<NomenclatureGroupEntity> NomenclatureGroupEntities { get; set; }

        public virtual DbSet<OptionItemEntity> OptionItemEntities { get; set; }

        public virtual DbSet<OrderEntity> OrderEntities { get; set; }

        public virtual DbSet<PhotoItemEntity> PhotoItemEntities { get; set; }

        public virtual DbSet<ProductDirectionEntity> ProductDirectionEntities { get; set; }

        public virtual DbSet<SendItemsEntity> SendItemsEntities { get; set; }

        public virtual int PrepareToUpdateBrands(string login, DateTimeOffset? lastUpdate)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));

            var lastUpdateParameter = lastUpdate.HasValue ?
                new ObjectParameter("lastUpdate", lastUpdate) :
                new ObjectParameter("lastUpdate", typeof(DateTimeOffset));

            return ((IObjectContextAdapter)this).ObjectContext
                .ExecuteFunction("PrepareToUpdateBrands", loginParameter, lastUpdateParameter);
        }

        public virtual int PrepareToUpdateCatalogs(string login, DateTimeOffset? lastUpdate)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));

            var lastUpdateParameter = lastUpdate.HasValue ?
                new ObjectParameter("lastUpdate", lastUpdate) :
                new ObjectParameter("lastUpdate", typeof(DateTimeOffset));

            return ((IObjectContextAdapter) this).ObjectContext
                .ExecuteFunction("PrepareToUpdateCatalogs", loginParameter, lastUpdateParameter);
        }

        public virtual int PrepareToUpdateDirectories(string login, DateTimeOffset? lastUpdate)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));

            var lastUpdateParameter = lastUpdate.HasValue ?
                new ObjectParameter("lastUpdate", lastUpdate) :
                new ObjectParameter("lastUpdate", typeof(DateTimeOffset));

            return ((IObjectContextAdapter)this).ObjectContext
                .ExecuteFunction("PrepareToUpdateCommodityDirections", loginParameter, lastUpdateParameter);
        }

        public virtual int PrepareToUpdatePhotos(string login, DateTimeOffset? lastUpdate)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));

            var lastUpdateParameter = lastUpdate.HasValue ?
                new ObjectParameter("lastUpdate", lastUpdate) :
                new ObjectParameter("lastUpdate", typeof(DateTimeOffset));

            return ((IObjectContextAdapter)this).ObjectContext
                .ExecuteFunction("PrepareToUpdatePhotos", loginParameter, lastUpdateParameter);
        }

        public virtual int PrepareToUpdateProductDirections(string login, DateTimeOffset? lastUpdate)
        {
            var loginParameter = login != null ?
                new ObjectParameter("login", login) :
                new ObjectParameter("login", typeof(string));

            var lastUpdateParameter = lastUpdate.HasValue ?
                new ObjectParameter("lastUpdate", lastUpdate) :
                new ObjectParameter("lastUpdate", typeof(DateTimeOffset));

            return ((IObjectContextAdapter)this).ObjectContext
                .ExecuteFunction("PrepareToUpdateProductDirections", loginParameter, lastUpdateParameter);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //SqlServerMigrationSqlGenerator
            //CreateProcedureOperation(modelBuilder)
            
            //SqlCommand cmd = new SqlCommand();
            //cmd.CommandType =CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue();
            //cmd.b
            //cmd.ExecuteNonQuery();

            base.OnModelCreating(modelBuilder);
        }
    }
}