using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Data.Enum;

namespace DataBase.Context.Entities
{
    public class CatalogItemEntity
    {
        public long Id { get; set; }

        [Index("IX_UID", 1, IsUnique = true)]
        public Guid UID { get; set; }

        [MaxLength(30)]
        public string Code { get; set; }

        [MaxLength(30)]
        public string Article { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public virtual BrandItemEntity Brand { get; set; }

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

        public CatalogItemStatus Status { get; set; }

        public DateTimeOffset LastUpdatedStatus { get; set; }

        public virtual DirectoryEntity Directory { get; set; }

        public virtual NomenclatureGroupEntity NomenclatureGroup { get; set; }

        public virtual List<CommodityDirectionEntity> CommodityDirection { get; set; }

        public virtual PriceGroupItemEntity PriceGroup { get; set; }

        public virtual List<TypeOfPricesNomenclatureItemEntity> TypeOfPriceItems { get; set; }

        public virtual List<DiscountsContragentEntity> Discounts { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
