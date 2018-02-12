using System;
using Common.Data.Enum;

namespace PricelistService.Service.Objects
{
    public class DbCatalogInfo
    {
        public long Id { get; set; }

        public Guid UID { get; set; }

        public string Code { get; set; }

        public string Article { get; set; }

        public string Name { get; set; }

        public long? BrandId { get; set; }

        public string BrandName { get; set; }

        public string Unit { get; set; }

        public string EnterpriceNormPack { get; set; }

        public decimal BatchOfSales { get; set; }

        public string Balance { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public decimal Multiplicity { get; set; }

        public bool HasPhotos { get; set; }

        public DateTimeOffset DateOfCreation { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset ForceUpdated { get; set; }

        public CatalogItemStatus Status { get; set; }

        public DateTimeOffset LastUpdatedStatus { get; set; }

        public long? DirectoryId { get; set; }
    }
}
