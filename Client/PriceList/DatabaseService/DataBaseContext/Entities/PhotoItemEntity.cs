using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.DataBaseContext.Entities
{
    public class PhotoItemEntity
    {
        public long Id { get; set; }

        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        public virtual List<CatalogItemEntity> CatalogItems { get; set; }

        public virtual List<BasketItemEntity> BasketItems { get; set; }

        public virtual List<OrderItemEntity> OrderItems { get; set; }
    }
}
