using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Data.Enum;

namespace DatabaseService.DataBaseContext.Entities
{
    public class CatalogItemEntity
    {
        public long Id { get; set; }

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

        public DateTimeOffset DateOfCreation { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public CatalogItemStatus Status { get; set; }

        public DateTimeOffset LastUpdatedStatus { get; set; }

        public virtual List<BasketItemEntity> BasketItems { get; set; }

        public virtual DirectoryEntity Directory { get; set; }

        public virtual NomenclatureGroupEntity NomenclatureGroup { get; set; }

        public virtual List<CommodityDirectionEntity> CommodityDirection { get; set; }
    }
}
