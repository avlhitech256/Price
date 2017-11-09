namespace DataService.Model
{
    public class PriceType
    {
        public long Id { get; set; }
        public NomenclatureGroupSetup NomenclatureGroupItem { get; set; }
        public PriceGroupSetup PriceGroupItem { get; set; }
        public PriceTypeSetup PriceTypeItem { get; set; }
    }
}
