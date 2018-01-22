using System;

namespace Load.Service.Implementation
{
    public class DirectDiscount
    {
        #region Constructors

        public DirectDiscount(Guid nomenclatureCode, decimal rate)
        {
            NomenclatureCode = nomenclatureCode;
            Rate = rate;
        }

        #endregion

        #region Properties

        public Guid NomenclatureCode { get; set; }

        public decimal Rate { get; }
        
        #endregion
    }
}
