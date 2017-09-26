using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class CommodityDirection
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Name;
    }
}
