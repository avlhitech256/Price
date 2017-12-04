using System;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class ShortcutInfo
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public DateTimeOffset RequestTime { get; set; }

        [DataMember]
        public DateTimeOffset ResponceTime { get; set; }
    }
}
