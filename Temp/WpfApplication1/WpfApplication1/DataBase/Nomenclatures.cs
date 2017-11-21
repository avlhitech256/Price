namespace WpfApplication1.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Nomenclatures
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Nomenclatures()
        {
            PictureItems = new HashSet<PictureItems>();
            PriceItems = new HashSet<PriceItems>();
        }

        public long Id { get; set; }

        public Guid UID { get; set; }

        public string Name { get; set; }

        public long? NomenclatureGroupSetup_Id { get; set; }

        public long? PriceGroupSetup_Id { get; set; }

        public long? Discount_Id { get; set; }

        public long? PriceList_Id { get; set; }

        public virtual Discounts Discounts { get; set; }

        public virtual NomenclatureGroupSetups NomenclatureGroupSetups { get; set; }

        public virtual PriceGroupSetups PriceGroupSetups { get; set; }

        public virtual PriceLists PriceLists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PictureItems> PictureItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceItems> PriceItems { get; set; }
    }
}
