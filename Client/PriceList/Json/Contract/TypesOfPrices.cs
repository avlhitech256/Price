using System;
using System.Runtime.Serialization;

namespace Json.Contract
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
