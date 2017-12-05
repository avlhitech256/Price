using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Data.Enum;

namespace DatabaseService.DataBaseContext.Entities
{
    public class CatalogItemEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Index("IX_UID", 1, IsUnique = true)]
        public Guid UID { get; set; }

        [Index("IX_Code", 1, IsUnique = false)]
        [MaxLength(30)]
        public string Code { get; set; }

        [Index("IX_Article", 1, IsUnique = false)]
        [MaxLength(30)]
        public string Article { get; set; }

        [Index("IX_Name", 1, IsUnique = false)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Index("IX_Brand", 1, IsUnique = false)]
        public virtual BrandItemEntity Brand { get; set; }

        [Index("IX_BrandName", 1, IsUnique = false)]
        [MaxLength(255)]
        public string BrandName { get; set; }

        [MaxLength(10)]
        public string Unit { get; set; }

        [MaxLength(30)]
        public string EnterpriceNormPack { get; set; }

        public decimal BatchOfSales { get; set; }

        [MaxLength(30)]
        public string Balance { get; set; }

        public decimal Price { get; set; }

        [MaxLength(5)]
        public string Currency { get; set; }

        public decimal Multiplicity { get; set; }

        public bool HasPhotos { get; set; }

        public virtual List<PhotoItemEntity> Photos { get; set; }

        [Index("IX_Status", 1, IsUnique = false)]
        public CatalogItemStatus Status { get; set; }

        [Index("IX_LastUpdatedStatus", 1, IsUnique = false)]
        public DateTimeOffset LastUpdatedStatus { get; set; }

        [Index("IX_BasketItems", 1, IsUnique = false)]
        public virtual List<BasketItemEntity> BasketItems { get; set; }

        [Index("IX_Directory", 1, IsUnique = false)]
        public virtual DirectoryEntity Directory { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
