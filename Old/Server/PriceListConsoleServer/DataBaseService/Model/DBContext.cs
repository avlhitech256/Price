namespace DataBaseService.Model
{
    using System.Data.Entity;

    public class DBContext : DbContext
    {
        // Контекст настроен для использования строки подключения "DBContext" из файла конфигурации  
        // приложения (App.config или Web.config). По умолчанию эта строка подключения указывает на базу данных 
        // "DataBaseService.Model.DBContext" в экземпляре LocalDb. 
        // 
        // Если требуется выбрать другую базу данных или поставщик базы данных, измените строку подключения "DBContext" 
        // в файле конфигурации приложения.
        public DBContext() : base("name=DBContext")
        {
        }

        // Добавьте DbSet для каждого типа сущности, который требуется включить в модель. Дополнительные сведения 
        // о настройке и использовании модели Code First см. в статье http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Discount> Discount { get; set; }
        public virtual DbSet<Nomenclature> Nomenclature { get; set; }
        public virtual DbSet<NomenclatureGroupSetup> NomenclatureGroupSetup { get; set; }
        public virtual DbSet<PictureItem> PictureItem { get; set; }
        public virtual DbSet<PriceGroupSetup> PriceGroupSetup { get; set; }
        public virtual DbSet<PriceItem> PriceItem { get; set; }
        public virtual DbSet<PriceList> PriceList { get; set; }
        public virtual DbSet<PriceType> PriceType { get; set; }
        public virtual DbSet<PriceTypeSetup> PriceTypeSetup { get; set; }
        public virtual DbSet<Updates> Updates { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}