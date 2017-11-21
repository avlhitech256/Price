namespace WpfApplication1.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PriceItems
    {
        public long Id { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public long? PriceTypeSetup_Id { get; set; }

        public long? Nomenclature_Id { get; set; }

        public virtual Nomenclatures Nomenclatures { get; set; }

        public virtual PriceTypeSetups PriceTypeSetups { get; set; }
    }
}
