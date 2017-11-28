using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class PhotoItemEntity
    {
        public long Id { get; set; }

        [Index("IX_Name", 1, IsUnique = true)]
        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsLoad { get; set; }

        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }

    }
}
