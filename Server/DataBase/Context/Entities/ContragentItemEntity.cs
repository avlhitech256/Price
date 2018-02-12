using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class ContragentItemEntity
    {
        public long Id { get; set; }

        [Index("IX_Code", 1, IsUnique = true)]
        [Required]
        public Guid UID { get; set; }

        [MaxLength(30)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [Index("IX_Login", 1, IsUnique = true)]
        [Required]
        [MaxLength(30)]
        public string Login { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        public decimal MutualSettlements { get; set; }

        [MaxLength(5)]
        public string MutualSettlementsCurrency { get; set; } 

        public decimal PDZ { get; set; }

        [MaxLength(5)]
        public string PDZCurrency { get; set; }

        public virtual List<PriceTypePriceGroupContragentEntity> PriceTypePriceGroups { get; set; }

        public virtual List<PriceTypeNomenclatureGroupContragentEntity> PriceTypeNomenclatureGroups { get; set; }

        public virtual List<DiscountsContragentEntity> Discounts { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }

        [Index("IX_ForceUpdated", 1, IsUnique = false)]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
