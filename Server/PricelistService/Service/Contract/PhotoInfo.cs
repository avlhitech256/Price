using System;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class PhotoInfo
    {
        [DataMember]
        public bool IsAuthorized { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsLoad { get; set; }

        [DataMember]
        public byte[] Photo { get; set; }

        [DataMember]
        public long CatalogId { get; set; }

        [DataMember]
        public DateTimeOffset DateOfCreation { get; set; }

        [DataMember]
        public DateTimeOffset LastUpdated { get; set; }

        [DataMember]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
