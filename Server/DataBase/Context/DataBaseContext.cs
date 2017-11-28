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

        // public virtual DbSet<MyEntity> MyEntities { get; set; }

        public virtual DbSet<BasketItemEntity> BasketItemEntities { get; set; }

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
    }
}