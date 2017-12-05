using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Data.Enum;

namespace DatabaseService.DataBaseContext.Entities
{
    public class OrderEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index("IX_OrderNumber", 1, IsUnique = true)]
        [MaxLength(30)]
        public string OrderNumber { get; set; }

        public decimal Sum { get; set; }

        [Index("IX_OrderStatus", 1, IsUnique = false)]
        public virtual OrderStatus OrderStatus { get; set; }

        [Index("IX_BasketItems", 1, IsUnique = false)]
        public virtual List<BasketItemEntity> BasketItems { get; set; }

        [MaxLength(1024)]
        public string Comment { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
