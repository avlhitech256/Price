using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Data.Enum;

namespace DatabaseService.DataBaseContext.Entities
{
    public class ProductDirectionEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        [Index("IX_Direction", 1, IsUnique = true)]
        public CommodityDirection Direction { get; set; }

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
