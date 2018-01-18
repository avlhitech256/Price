namespace DataBase.Objects
{
    public class PriceInfo
    {
        #region Constructors
        
        public PriceInfo(decimal price, string currency)
        {
            Price = price;
            Currency = currency;
        }

        public PriceInfo() : this(0M, string.Empty) { }

        #endregion

        #region Properties

        public decimal Price { get; set; }

        public string Currency { get; set; }

        #endregion
    }
}
