using System;
using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class PriceTypeItem
    {
        [DataMember]
        public Guid UID;

        [DataMember]
        public string Price;
    }
}
