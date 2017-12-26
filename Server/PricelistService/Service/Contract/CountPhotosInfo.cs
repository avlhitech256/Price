using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class CountPhotosInfo
    {
        [DataMember]
        public bool IsAuthorized { get; set; }

        [DataMember]
        public long CountPhotos { get; set; }
    }
}
