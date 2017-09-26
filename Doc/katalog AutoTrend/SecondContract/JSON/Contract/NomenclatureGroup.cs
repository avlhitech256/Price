using System;
using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class NomenclatureGroup
    {
        [DataMember]
        public Guid UID;

        [DataMember]
        public string Name;
    }
}
