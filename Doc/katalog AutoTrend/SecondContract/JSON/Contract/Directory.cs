using System;
using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Directory
    {
        [DataMember]
        public Guid UID;

        [DataMember]
        public string Name;

        [DataMember]
        public Directory[] Subdirectory;
    }
}
