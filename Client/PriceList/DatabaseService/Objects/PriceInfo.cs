namespace DatabaseService.Objects
{
    public class PriceInfo
    {
        #region Constructors
        
        public PriceInfo(decimal price, string currency)
        {
            Prise = price;
            Currency = currency;
        }

        public PriceInfo() : this(0M, string.Empty) { }

        #endregion

        #region Properties

        public decimal Prise { get; set; }

        public string Currency { get; set; }

        #endregion
    }
}
