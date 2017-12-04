using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class OptionItemEntity
    {
        public long Id { get; set; }

        [Index("IX_Code", 1, IsUnique = true)]
        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Value { get; set; }
        [Index("IX_DateOfCreation", 1, IsUnique = false)]
        public DateTimeOffset DateOfCreation { get; set; }

        [Index("IX_LastUpdated", 1, IsUnique = false)]
        public DateTimeOffset LastUpdated { get; set; }
    }
}
