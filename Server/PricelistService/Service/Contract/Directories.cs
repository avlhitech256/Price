﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class Directories
    {
        [DataMember]
        public bool IsAuthorized { get; set; }

        [DataMember]
        public long Count { get; set; }

        [DataMember]
        public List<DirectoryInfo> Items { get; set; }
    }
}
