using System.Runtime.Serialization;

namespace JSON.Contract
{
    [DataContract]
    public class PriceList
    {
        [DataMember]
        public Nomenclature[] Nomenclature;
    }
}
