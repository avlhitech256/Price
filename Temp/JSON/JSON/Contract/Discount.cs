using System;
using System.Runtime.Serialization;

namespace JSON.Contract
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
