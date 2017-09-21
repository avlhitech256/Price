using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class Clients
    {
        [DataMember]
        public Client[] Contragent;
    }
}
