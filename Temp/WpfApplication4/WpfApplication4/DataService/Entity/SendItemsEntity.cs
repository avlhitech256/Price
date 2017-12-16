using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApplication4.DataService.Entity
{
    public class SendItemsEntity
    {
        public long Id { get; set; }

        [Index("IX_Login", 1, IsUnique = false)]
        [MaxLength(30)]
        public string Login { get; set; }

        public long EntityId { get; set; }

        [Index("IX_EntityName", 1, IsUnique = false)]
        public int EntityName { get; set; }

        [Index("IX_RequestDate", 1, IsUnique = false)]
        public DateTimeOffset RequestDate { get; set; }

        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }
    }
}
