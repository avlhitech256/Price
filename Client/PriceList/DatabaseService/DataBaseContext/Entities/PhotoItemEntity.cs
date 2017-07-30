using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseService.DataBaseContext.Entities
{
    public class PhotoItemEntity
    {
        public long Id { get; set; }

        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }
    }
}
