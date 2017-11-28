using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class BrandItemEntity
    {
        public long Id { get; set; }

        [Index("IX_Code", 1, IsUnique = true)]
        public Guid Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public virtual List<CatalogItemEntity> CatalogItems { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
