using System;
using System.Runtime.Serialization;
using Common.Data.Enum;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class ProductDirectionInfo
    {
        [DataMember]
        public bool IsAuthorized { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public CommodityDirection Direction { get; set; }

        [DataMember]
        public long DirectoryId { get; set; }

        [DataMember]
        public DateTimeOffset DateOfCreation { get; set; }

        [DataMember]
        public DateTimeOffset LastUpdated { get; set; }

        [DataMember]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
