using System;
using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class TypeOfNomenclature
    {
        [DataMember]
        public Guid NomenclatureGroupUID;

        [DataMember]
        public Guid PriceTypeUID;
    }
}
