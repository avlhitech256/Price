using System;

namespace Load.Service.Implementation
{
    public class DirectPriceTypePriceGroupContragent
    {
        #region Constructors

        public DirectPriceTypePriceGroupContragent(Guid typeOfPriceCode, Guid priceGroupCode)
        {
            TypeOfPriceCode = typeOfPriceCode;
            PriceGroupCode = priceGroupCode;
        }

        #endregion

        #region Properties

        public Guid TypeOfPriceCode { get; }

        public Guid PriceGroupCode { get; }

        #endregion
    }
}
