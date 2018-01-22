using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class DiscountsContragentEntity
    {
        public long Id { get; set; }

        public decimal Rate { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }

        public virtual ContragentItemEntity ContragentItem { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
