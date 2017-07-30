using System;
using System.Collections.Generic;

namespace DatabaseService.DataBaseContext.Entities
{
    public class BasketItemEntity
    {
        public long Id { get; set; }

        public Guid UID { get; set; }

        public string Code { get; set; }

        public string Article { get; set; }

        public string Name { get; set; }

        public virtual BrandItemEntity Brand { get; set; }

        public string Unit { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public virtual List<PhotoItemEntity> Photos { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }

        public decimal Count { get; set; }

        public DateTime DateAction { get; set; }
    }
}
