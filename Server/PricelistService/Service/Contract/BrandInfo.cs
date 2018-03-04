using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class BrandInfo
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTimeOffset DateOfCreation { get; set; }

        [DataMember]
        public DateTimeOffset LastUpdated { get; set; }

        [DataMember]
        public DateTimeOffset ForceUpdated { get; set; }
    }
}
