using System;
using System.Collections.Generic;

namespace PricelistService.Service.Objects
{
    public class DbBrandInfo
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        public string Name { get; set; }

        public DateTimeOffset DateOfCreation { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset ForceUpdated { get; set; }
    }
}
