using System.ComponentModel.DataAnnotations.Schema;

namespace DataBaseService.Model
{
    public class PictureItem
    {
        public long Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture { get; set; }
    }
}
