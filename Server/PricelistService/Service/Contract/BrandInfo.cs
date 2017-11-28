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
        public virtual List<long> CatalogId { get; set; }

    }
}
