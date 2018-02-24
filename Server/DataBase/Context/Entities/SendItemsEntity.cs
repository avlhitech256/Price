using System;
using System.ComponentModel.DataAnnotations.Schema;
using DataBase.Context.Object;

namespace DataBase.Context.Entities
{
    public class SendItemsEntity
    {
        public long Id { get; set; }

        public virtual ContragentItemEntity Contragent { get; set; }

        public long EntityId { get; set; }

        [Index("IX_EntityName", 1, IsUnique = false)]
        public EntityName EntityName { get; set; }

        [Index("IX_RequestDate", 1, IsUnique = false)]
        public DateTimeOffset RequestDate { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }
    }
}
