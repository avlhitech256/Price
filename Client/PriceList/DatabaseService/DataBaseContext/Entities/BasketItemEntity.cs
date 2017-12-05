using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.DataBaseContext.Entities
{
    public class BasketItemEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index("IX_CatalogItem", 1, IsUnique = false)]
        public virtual CatalogItemEntity CatalogItem { get; set; }

        [Index("IX_Order", 1, IsUnique = false)]
        public virtual OrderEntity Order { get; set; }

        public decimal Count { get; set; }

        [Index("IX_DateAction", 1, IsUnique = false)]
        public DateTimeOffset DateAction { get; set; }
    }
}
