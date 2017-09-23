using System;
using System.Runtime.Serialization;

namespace Json.Contract
{
    [DataContract]
    public class Discount
    {
        [DataMember]
        public Decimal Rate;

        [DataMember]
        public Guid[] Nomenclature;
    }
}
