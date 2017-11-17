using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Nomenclature
    {
        [DataMember]
        public string UID;

        [DataMember]
        public string Code;

        [DataMember]
        public string VendorCode;

        [DataMember]
        public string BrandUID;

        [DataMember]
        public string Name;

        [DataMember]
        public string Measure;

        [DataMember]
        public string NormPackaging;

        [DataMember]
        public string BatchOfSales;

        [DataMember]
        public string PriceGroupUID;

        [DataMember]
        public string NomenclatureGroupUID;

        [DataMember]
        public string DateOfCreation;

        [DataMember]
        public string CatalogUID;

        [DataMember]
        public string InStock;

        [DataMember]
        public PriceTypeItem[] TypesOfPrices;

        [DataMember]
        public string[] CommodityDirection;

        [DataMember]
        public string[] Photos;
    }
}
