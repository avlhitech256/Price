using System;
using System.Collections.Generic;

namespace DatabaseService.Model
{
    public class PriceList
    {
        public long Id { get; set; }
        public DateTime LastUpdate { get; set; }
        public List<Nomenclature> Nomenclatures { get; set; }
    }
}
