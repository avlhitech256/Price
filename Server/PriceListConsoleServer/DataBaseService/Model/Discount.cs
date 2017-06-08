using System.Collections.Generic;

namespace DataBaseService.Model
{
    public class Discount
    {
        public long Id { get; set; }
        public decimal DiscountRate { get; set; }
        public List<Nomenclature> Nomenclatures { get; set; }
    }
}
