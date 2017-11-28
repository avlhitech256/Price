using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class Brands
    {
        [DataMember]
        public long Count { get; set; }

        [DataMember]
        public List<BrandInfo> Items { get; set; } 
    }
}
