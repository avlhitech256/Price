using System;
using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class TypesOfPrices
    {
        [DataMember]
        public Guid PriceGroupUID;

        [DataMember]
        public Guid PriceTypeUID;
    }
}
