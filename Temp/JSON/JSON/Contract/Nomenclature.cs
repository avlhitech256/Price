using System;
using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class Nomenclature
    {
        [DataMember]
        public Guid UID;

        [DataMember]
        public string Code;

        [DataMember]
        public string VendorCode;

        [DataMember]
        public Guid BrandUID;

        [DataMember]
        public string Name;

        [DataMember]
        public string Measure;

        [DataMember]
        public decimal NormPackaging;

        [DataMember]
        public decimal BatchOfSales;

        [DataMember]
        public Guid PriceGroupUID;

        [DataMember]
        public Guid NomenclatureGroupUID;

        [DataMember]
        public string DateOfCreation;

        [DataMember]
        public Guid CatalogUID;

        [DataMember]
        public string InStock;

        [DataMember]
        public PriceTypeItem[] TypesOfPrices;

        [DataMember]
        public Guid[] CommodityDirection;

        [DataMember]
        public string[] Photos;
    }
}
