using System.Runtime.Serialization;

namespace JSON.Contract
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
