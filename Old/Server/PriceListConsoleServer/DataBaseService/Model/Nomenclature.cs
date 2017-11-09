using System;
using System.Collections.Generic;

namespace DataBaseService.Model
{
    public class Nomenclature
    {
        public long Id { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public PriceGroupSetup PriceGroupSetup { get; set; }
        public NomenclatureGroupSetup NomenclatureGroupSetup { get; set; }
        public List<PictureItem> PictureItems { get; set; }
        public List<PriceItem> PriceItems { get; set; }
    }
}
