using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class PriceList
    {
        [DataMember]
        public Nomenclature[] Nomenclature;
    }
}
