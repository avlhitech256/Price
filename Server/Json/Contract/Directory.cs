using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Directory
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Name;

        [DataMember]
        public Directory[] Subdirectory;
    }
}
