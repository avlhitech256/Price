using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class DirectoryEntity
    {
        public long Id { get; set; }

        [Index("IX_Code", 1, IsUnique = true)]
        [Required]
        public Guid Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public virtual DirectoryEntity Parent { get; set; }

        public virtual List<DirectoryEntity> SubDirectory { get; set; }

        public virtual List<CatalogItemEntity> CatalogItems { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
