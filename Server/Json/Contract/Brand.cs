using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Brand
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Name;
    }
}
