using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.DataBaseContext.Entities
{
    public class BrandItemEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Index("IX_Code", 1, IsUnique = true)]
        public Guid Code { get; set; }

        [Index("IX_Name", 1, IsUnique = false)]
        [MaxLength(255)]
        public string Name { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public virtual List<CatalogItemEntity> CatalogItems { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
