using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class CountInfo
    {
        [DataMember]
        public long CountBrands { get; set; }

        [DataMember]
        public long CountCatalog { get; set; }

        [DataMember]
        public long CountDirectory { get; set; }

        [DataMember]
        public long CountPhoto { get; set; }
    }
}
