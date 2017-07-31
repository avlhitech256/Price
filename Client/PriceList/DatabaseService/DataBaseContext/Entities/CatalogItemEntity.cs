using System;
using System.Collections.Generic;

namespace DatabaseService.DataBaseContext.Entities
{
    public class CatalogItemEntity
    {
        public long Id { get; set; }

        public Guid UID { get; set; }
        
        public string Code { get; set; }

        public string Article { get; set; }

        public string Name { get; set; }

        public virtual BrandItemEntity Brand { get; set; }

        public string Unit { get; set; }

        public string EnterpriceNormPack { get; set; }

        public string Balance { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public decimal Multiplicity { get; set; }

        public virtual List<PhotoItemEntity> Photos { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public bool PriceIsUp { get; set; }
        
        public bool PriceIsDown { get; set; }

        public bool IsNew { get; set; }
    }
}
