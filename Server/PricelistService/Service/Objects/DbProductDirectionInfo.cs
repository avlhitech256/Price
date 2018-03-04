using System;
using Common.Data.Enum;

namespace PricelistService.Service.Objects
{
    public class DbProductDirectionInfo
    {
        public long Id { get; set; }

        public CommodityDirection Direction { get; set; }

        public long? DirectoryId { get; set; }

        public DateTimeOffset DateOfCreation { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset ForceUpdated { get; set; }
    }
}
