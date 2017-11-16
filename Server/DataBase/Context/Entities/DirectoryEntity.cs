using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataBase.Context.Entities
{
    public class DirectoryEntity
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public virtual DirectoryEntity Parent { get; set; }

        public virtual List<DirectoryEntity> SubDirectory { get; set; }

        public virtual List<CatalogItemEntity> CatalogItems { get; set; }
    }
}
