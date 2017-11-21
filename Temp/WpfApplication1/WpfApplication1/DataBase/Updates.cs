namespace WpfApplication1.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Updates
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Updates()
        {
            Clients = new HashSet<Clients>();
            NomenclatureGroupSetups = new HashSet<NomenclatureGroupSetups>();
            PriceGroupSetups = new HashSet<PriceGroupSetups>();
        }

        public long Id { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public string ModefirdUser { get; set; }

        public long? PriceList_Id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Clients> Clients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NomenclatureGroupSetups> NomenclatureGroupSetups { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceGroupSetups> PriceGroupSetups { get; set; }

        public virtual PriceLists PriceLists { get; set; }
    }
}
