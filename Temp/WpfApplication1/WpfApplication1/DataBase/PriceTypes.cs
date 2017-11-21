namespace WpfApplication1.DataBase
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PriceTypes
    {
        public long Id { get; set; }

        public long? NomenclatureGroupItem_Id { get; set; }

        public long? PriceGroupItem_Id { get; set; }

        public long? PriceTypeItem_Id { get; set; }

        public long? Client_Id { get; set; }

        public virtual Clients Clients { get; set; }

        public virtual NomenclatureGroupSetups NomenclatureGroupSetups { get; set; }

        public virtual PriceGroupSetups PriceGroupSetups { get; set; }

        public virtual PriceTypeSetups PriceTypeSetups { get; set; }
    }
}
