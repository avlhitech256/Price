using System;
using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class Brand
    {
        [DataMember]
        public Guid UID;

        [DataMember]
        public string Name;
    }
}
