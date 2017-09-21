using System;
using System.Runtime.Serialization;

namespace JSON.Contract
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
