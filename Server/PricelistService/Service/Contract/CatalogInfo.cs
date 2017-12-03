using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Data.Enum;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class CatalogInfo
    {
        [DataMember]
        public bool IsAuthorized { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid UID { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Article { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long BrandId { get; set; }

        [DataMember]
        public string BrandName { get; set; }

        [DataMember]
        public string Unit { get; set; }

        [DataMember]
        public string EnterpriceNormPack { get; set; }

        [DataMember]
        public decimal BatchOfSales { get; set; }

        [DataMember]
        public string Balance { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public decimal Multiplicity { get; set; }

        [DataMember]
        public bool HasPhotos { get; set; }

        [DataMember]
        public List<long> Photos { get; set; }

        [DataMember]
        public DateTimeOffset DateOfCreation { get; set; }

        [DataMember]
        public DateTimeOffset LastUpdated { get; set; }

        [DataMember]
        public DateTimeOffset ForceUpdated { get; set; }

        [DataMember]
        public CatalogItemStatus Status { get; set; }

        [DataMember]
        public DateTimeOffset LastUpdatedStatus { get; set; }

        [DataMember]
        public long DirectoryId { get; set; }
    }
}
