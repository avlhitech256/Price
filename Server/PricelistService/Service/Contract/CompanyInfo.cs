using System;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class CompanyInfo
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string Phone { get; set; }

        [DataMember]
        public string EMail { get; set; }

        [DataMember]
        public string WebSite { get; set; }

        [DataMember]
        public DateTimeOffset TimeRequest { get; set; }

        [DataMember]
        public DateTimeOffset TimeResponce { get; set; }
    }
}
