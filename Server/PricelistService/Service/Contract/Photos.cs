﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class Photos
    {
        [DataMember]
        public long StartNumber { get; set; }

        [DataMember]
        public long Count { get; set; }

        [DataMember]
        public List<PhotoInfo> Items { get; set; }
    }
}
