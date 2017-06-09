using System;
using System.Collections.Generic;

namespace DataBaseService.Model
{
    public class Updates
    {
        public long Id { get; set; }
        public DateTime UpdateDateTime { get; set; }
        public List<Client> Clients { get; set; } 
        public PriceList PriceList { get; set; }
        public List<PriceGroupSetup> PriceGroups { get; set; }
        public List<NomenclatureGroupSetup> NomenclatureGroup { get; set; }
        public string ModefirdUser { get; set; }
    }
}
