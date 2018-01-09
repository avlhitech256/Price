using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataBaseContext.Initializer;

namespace DatabaseService.DataBaseContext
{
    using System.Data.Entity;

    public class DataBaseContext : DbContext
    {
        // Контекст настроен для использования строки подключения "DataBaseContext" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "DatabaseService.DataBaseContext.DataBaseContext" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "DataBaseContext" 
        // в файле конфигурации приложения.
        public DataBaseContext()
            : base("name=DataBaseContext")
        {
            Debug.Write("Old Connection String" + Database.Connection.ConnectionString);
            Database.Connection.ConnectionString = PrepareConnectionString();
            Debug.Write("New Connection String" + Database.Connection.ConnectionString);
            Database.Log = (s => Debug.WriteLine(s));
            Database.SetInitializer(new DataBaseInitializer());

            try
            {
                Database.Initialize(false);
            }
            catch (SqlException e)
            {
                if (e.HResult != -2146232060)
                {
                    throw e;
                }
            }

        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }


        public virtual DbSet<BasketItemEntity> BasketItemEntities { get; set; }

        public virtual DbSet<BrandItemEntity> BrandItemEntities { get; set; }

        public virtual DbSet<CatalogItemEntity> CatalogItemEntities { get; set; }

        public virtual DbSet<DirectoryEntity> DirectoryEntities { get; set; }

        public virtual DbSet<OptionItemEntity> OptionItemEntities { get; set; }

        public virtual DbSet<OrderEntity> OrderEntities { get; set; }

        public virtual DbSet<PhotoItemEntity> PhotoItemEntities { get; set; }

        public virtual DbSet<ProductDirectionEntity> ProductDirectionEntities { get; set; }

        private string PrepareConnectionString()
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(Database.Connection.ConnectionString);

            if (string.IsNullOrWhiteSpace(connectionString.AttachDBFilename))
            {
                string databaseName = connectionString.InitialCatalog;
                //string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
                //if (Environment.OSVersion.Version.Major >= 6)
                //{
                //    path = Directory.GetParent(path).ToString();
                //}

                connectionString.AttachDBFilename = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) +
                                                    "\\" + databaseName + ".mdf";
            }

            PrepareDirectory(connectionString.AttachDBFilename);
            return connectionString.ToString();
        }

        private void PrepareDirectory(string attachDBFilename)
        {
            int endOfPathIndex = attachDBFilename.LastIndexOf("\\");

            if (endOfPathIndex > 0)
            {
                string dirPath = attachDBFilename.Substring(0, endOfPathIndex + 1);

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
        }
    }
}