﻿using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public string UID;

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
