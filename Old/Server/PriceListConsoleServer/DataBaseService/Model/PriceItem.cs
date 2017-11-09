namespace DataBaseService.Model
{
    public class PriceItem
    {
        public long Id { get; set; }
        public PriceTypeSetup PriceTypeSetup { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
    }
}
