using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class TypesOfPrices
    {
        [DataMember]
        public string PriceGroupUID;

        [DataMember]
        public string PriceTypeUID;
    }
}
