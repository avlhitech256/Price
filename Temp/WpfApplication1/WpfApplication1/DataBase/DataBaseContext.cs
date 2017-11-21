namespace WpfApplication1.DataBase
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DataBaseContext : DbContext
    {
        public DataBaseContext()
            : base("name=DataBaseContext")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Discounts> Discounts { get; set; }
        public virtual DbSet<NomenclatureGroupSetups> NomenclatureGroupSetups { get; set; }
        public virtual DbSet<Nomenclatures> Nomenclatures { get; set; }
        public virtual DbSet<PictureItems> PictureItems { get; set; }
        public virtual DbSet<PriceGroupSetups> PriceGroupSetups { get; set; }
        public virtual DbSet<PriceItems> PriceItems { get; set; }
        public virtual DbSet<PriceLists> PriceLists { get; set; }
        public virtual DbSet<PriceTypes> PriceTypes { get; set; }
        public virtual DbSet<PriceTypeSetups> PriceTypeSetups { get; set; }
        public virtual DbSet<Updates> Updates { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clients>()
                .HasMany(e => e.Discounts)
                .WithOptional(e => e.Clients)
                .HasForeignKey(e => e.Client_Id);

            modelBuilder.Entity<Clients>()
                .HasMany(e => e.PriceTypes)
                .WithOptional(e => e.Clients)
                .HasForeignKey(e => e.Client_Id);

            modelBuilder.Entity<Discounts>()
                .HasMany(e => e.Nomenclatures)
                .WithOptional(e => e.Discounts)
                .HasForeignKey(e => e.Discount_Id);

            modelBuilder.Entity<NomenclatureGroupSetups>()
                .HasMany(e => e.Nomenclatures)
                .WithOptional(e => e.NomenclatureGroupSetups)
                .HasForeignKey(e => e.NomenclatureGroupSetup_Id);

            modelBuilder.Entity<NomenclatureGroupSetups>()
                .HasMany(e => e.PriceTypes)
                .WithOptional(e => e.NomenclatureGroupSetups)
                .HasForeignKey(e => e.NomenclatureGroupItem_Id);

            modelBuilder.Entity<Nomenclatures>()
                .HasMany(e => e.PictureItems)
                .WithOptional(e => e.Nomenclatures)
                .HasForeignKey(e => e.Nomenclature_Id);

            modelBuilder.Entity<Nomenclatures>()
                .HasMany(e => e.PriceItems)
                .WithOptional(e => e.Nomenclatures)
                .HasForeignKey(e => e.Nomenclature_Id);

            modelBuilder.Entity<PriceGroupSetups>()
                .HasMany(e => e.Nomenclatures)
                .WithOptional(e => e.PriceGroupSetups)
                .HasForeignKey(e => e.PriceGroupSetup_Id);

            modelBuilder.Entity<PriceGroupSetups>()
                .HasMany(e => e.PriceTypes)
                .WithOptional(e => e.PriceGroupSetups)
                .HasForeignKey(e => e.PriceGroupItem_Id);

            modelBuilder.Entity<PriceLists>()
                .HasMany(e => e.Nomenclatures)
                .WithOptional(e => e.PriceLists)
                .HasForeignKey(e => e.PriceList_Id);

            modelBuilder.Entity<PriceLists>()
                .HasMany(e => e.Updates)
                .WithOptional(e => e.PriceLists)
                .HasForeignKey(e => e.PriceList_Id);

            modelBuilder.Entity<PriceTypeSetups>()
                .HasMany(e => e.PriceItems)
                .WithOptional(e => e.PriceTypeSetups)
                .HasForeignKey(e => e.PriceTypeSetup_Id);

            modelBuilder.Entity<PriceTypeSetups>()
                .HasMany(e => e.PriceTypes)
                .WithOptional(e => e.PriceTypeSetups)
                .HasForeignKey(e => e.PriceTypeItem_Id);

            modelBuilder.Entity<Updates>()
                .HasMany(e => e.Clients)
                .WithOptional(e => e.Updates)
                .HasForeignKey(e => e.Updates_Id);

            modelBuilder.Entity<Updates>()
                .HasMany(e => e.NomenclatureGroupSetups)
                .WithOptional(e => e.Updates)
                .HasForeignKey(e => e.Updates_Id);

            modelBuilder.Entity<Updates>()
                .HasMany(e => e.PriceGroupSetups)
                .WithOptional(e => e.Updates)
                .HasForeignKey(e => e.Updates_Id);
        }
    }
}
