using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Clients
    {
        [DataMember]
        public Client[] Contragent;
    }
}
