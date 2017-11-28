using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class Catalogs
    {
        [DataMember]
        public long StartNumber { get; set; }

        [DataMember]
        public long Count { get; set; }

        [DataMember]
        public List<CatalogInfo> Items { get; set; }
    }
}
