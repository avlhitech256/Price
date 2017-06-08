using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.Model
{
    public class PictureItem
    {
        public long Id { get; set; }
        [Column(TypeName = "image")]
        public string Name { get; set; }
        public byte[] Picture { get; set; }
    }
}
