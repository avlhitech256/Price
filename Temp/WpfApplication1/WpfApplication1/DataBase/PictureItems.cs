namespace WpfApplication1.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PictureItems
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "image")]
        public byte[] Picture { get; set; }

        public long? Nomenclature_Id { get; set; }

        public virtual Nomenclatures Nomenclatures { get; set; }
    }
}
