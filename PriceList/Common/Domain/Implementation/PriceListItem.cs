namespace Common.Domain.Implementation
{
    public class PriceListItem : IPriceListItem
    {
        public string Code { get; set; }
        public string VendorCode { get; set; }
        public string Manufacturer { get; set; }
        public string Nomenclature { get; set; }
        public int AmountInPackage { get; set; }
        public string Remainder { get; set; }
        public float Price { get; set; }
        public string Unit { get; set; }
    }
}
