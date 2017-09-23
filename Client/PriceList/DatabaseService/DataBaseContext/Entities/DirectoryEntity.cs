using System;
using System.ComponentModel.DataAnnotations;

namespace DatabaseService.DataBaseContext.Entities
{
    public class DirectoryEntity
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        public DirectoryEntity SubDirectory { get; set; }
    }
}
