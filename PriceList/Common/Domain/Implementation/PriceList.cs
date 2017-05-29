using System;
using System.Collections.Generic;

namespace Common.Domain.Implementation
{
    public class PriceList
    {
        public DateTime LastModify { get; set; }
        public IEnumerable<PriceListItem> Items{ get; set; } 
    }
}
