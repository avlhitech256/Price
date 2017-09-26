using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class PriceGroup
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Name;
    }
}
