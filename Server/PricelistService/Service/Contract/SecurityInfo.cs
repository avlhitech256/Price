using System;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class SecurityInfo
    {
        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public DateTimeOffset TimeRequest { get; set; }

        [DataMember]
        public string Version { get; set; }

        [DataMember]
        public string TypeSecurity { get; set; }

        [DataMember]
        public string OSLogin { get; set; }

        [DataMember]
        public string Workstation { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public string OSVersion { get; set; }
    }
}
