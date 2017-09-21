using System;
using System.Runtime.Serialization;

namespace JSON.Contract
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
