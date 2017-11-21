namespace WpfApplication1.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NomenclatureGroupSetups
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NomenclatureGroupSetups()
        {
            Nomenclatures = new HashSet<Nomenclatures>();
            PriceTypes = new HashSet<PriceTypes>();
        }

        public long Id { get; set; }

        public Guid UID { get; set; }

        public string Name { get; set; }

        public long? Updates_Id { get; set; }

        public virtual Updates Updates { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Nomenclatures> Nomenclatures { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PriceTypes> PriceTypes { get; set; }
    }
}
