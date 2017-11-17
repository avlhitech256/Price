using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class PriceTypeItem
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Price;
    }
}
