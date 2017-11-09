namespace Common.Domain
{
    public interface IPriceListItem
    {
        string Code { get; set; }
        string VendorCode { get; set; }
        string Manufacturer { get; set; }
        string Nomenclature { get; set; }
        int AmountInPackage { get; set; }
        string Remainder { get; set; }
        float Price { get; set; }
        string Unit { get; set; }
    }
}
