using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class PriceType
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Name;
    }
}
