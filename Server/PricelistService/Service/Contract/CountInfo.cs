using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class CountInfo
    {
        [DataMember]
        public bool IsAuthorized { get; set; }

        [DataMember]
        public long CountBrands { get; set; }

        [DataMember]
        public long CountCatalogs { get; set; }

        [DataMember]
        public long CountDirectories { get; set; }

        [DataMember]
        public long CountProductDirections { get; set; }

        [DataMember]
        public long CountPhotos { get; set; }
    }
}
