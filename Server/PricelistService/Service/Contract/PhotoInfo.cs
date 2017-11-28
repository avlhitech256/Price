using System.Runtime.Serialization;

namespace PricelistService.Service.Contract
{
    [DataContract]
    public class PhotoInfo
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool IsLoad { get; set; }

        [DataMember]
        public byte[] Photo { get; set; }

    }
}
