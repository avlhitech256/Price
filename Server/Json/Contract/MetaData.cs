using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class MetaData
    {
        [DataMember]
        public Directory[] Catalog; 

        [DataMember]
        public PriceGroup[] PriceGroups;

        [DataMember]
        public NomenclatureGroup[] NomenclatureGroups;

        [DataMember]
        public Brand[] Brands;

        [DataMember]
        public PriceType[] TypesOfPrices;

        [DataMember]
        public CommodityDirection[] CommodityDirections;
    }
}
