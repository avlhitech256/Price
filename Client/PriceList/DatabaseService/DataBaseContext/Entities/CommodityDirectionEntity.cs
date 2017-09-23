using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseService.DataBaseContext.Entities
{
    public class CommodityDirectionEntity
    {
        public long Id { get; set; }

        public Guid UID { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
    }
}
