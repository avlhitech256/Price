using System;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Data.Enum;

namespace DataBase.Context.Entities
{
    public class ProductDirectionEntity
    {
        public long Id { get; set; }

        public CommodityDirection Direction { get; set; }

        public virtual DirectoryEntity Directory { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
