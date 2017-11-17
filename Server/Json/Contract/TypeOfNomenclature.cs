using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class TypeOfNomenclature
    {
        [DataMember]
        public string NomenclatureGroupUID;

        [DataMember]
        public string PriceTypeUID;
    }
}
