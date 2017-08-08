using System;

namespace DatabaseService.DataBaseContext.Entities
{
    public class BasketItemEntity
    {
        public long Id { get; set; }

        public Guid UID { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }

        public decimal Count { get; set; }

        public DateTime DateAction { get; set; }
    }
}
