using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DatabaseService.DataBaseContext.Entities
{
    public class NomenclatureGroupEntity
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        public virtual List<CatalogItemEntity> CatalogItems { get; set; }

    }
}
