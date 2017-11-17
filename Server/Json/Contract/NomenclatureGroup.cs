using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class NomenclatureGroup
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Name;
    }
}
