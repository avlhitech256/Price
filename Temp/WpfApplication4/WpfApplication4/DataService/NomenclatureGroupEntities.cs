//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApplication4.DataService
{
    using System;
    using System.Collections.Generic;
    
    public partial class NomenclatureGroupEntities
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NomenclatureGroupEntities()
        {
            this.CatalogItemEntities = new HashSet<CatalogItemEntities>();
        }
    
        public long Id { get; set; }
        public System.Guid Code { get; set; }
        public string Name { get; set; }
        public System.DateTimeOffset DateOfCreation { get; set; }
        public System.DateTimeOffset LastUpdated { get; set; }
        public System.DateTimeOffset ForceUpdated { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CatalogItemEntities> CatalogItemEntities { get; set; }
    }
}
