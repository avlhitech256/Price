using System;
using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public Guid UID;

        [DataMember]
        public string Code;

        [DataMember]
        public string Name;

        [DataMember]
        public string Login;

        [DataMember]
        public string MutualSettlements;

        [DataMember]
        public string PDZ;

        [DataMember]
        public TypesOfPrices[] PriceTypePriceGroup;

        [DataMember]
        public TypeOfNomenclature[] PriceTypeNomenclatureGroup;

        [DataMember]
        public Discount[] Discounts;
    }
}
