using System;

namespace Load.Service.Implementation
{
    public class DirectPriceTypeNomenclatureGroupContragent
    {
        #region Constructors

        public DirectPriceTypeNomenclatureGroupContragent(Guid typeOfPriceCode, Guid nomenclatureGroupCode)
        {
            TypeOfPriceCode = typeOfPriceCode;
            NomenclatureGroupCode = nomenclatureGroupCode;
        }

        #endregion

        #region Properties

        public Guid TypeOfPriceCode { get; set; }

        public Guid NomenclatureGroupCode { get; set; }

        #endregion
    }
}
