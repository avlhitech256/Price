using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class TypeOfPricesNomenclatureItemEntity
    {
        public long Id { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }

        public virtual TypeOfPriceItemEntity TypeOfPriceItem { get; set; }

        public decimal Price { get; set; }

        [MaxLength(5)]
        public string Currency { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
