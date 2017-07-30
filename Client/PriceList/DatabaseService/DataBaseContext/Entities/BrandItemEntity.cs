using System;

namespace DatabaseService.DataBaseContext.Entities
{
    public class BrandItemEntity
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        public string Name { get; set; }
    }
}
