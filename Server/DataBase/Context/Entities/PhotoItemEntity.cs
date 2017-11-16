using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Context.Entities
{
    public class PhotoItemEntity
    {
        public long Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public bool IsLoad { get; set; }

        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }
    }
}
