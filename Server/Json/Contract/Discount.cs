using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Discount
    {
        [DataMember]
        public string Rate;

        [DataMember]
        public string[] Nomenclature;
    }
}
