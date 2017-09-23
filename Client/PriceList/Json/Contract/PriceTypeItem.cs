using System;
using System.Runtime.Serialization;

namespace Json.Contract
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
