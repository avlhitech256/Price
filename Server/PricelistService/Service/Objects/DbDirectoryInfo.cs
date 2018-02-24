using System;
using System.Collections.Generic;
using DataBase.Context.Entities;

namespace PricelistService.Service.Objects
{
    public class DbDirectoryInfo
    {
        public long Id { get; set; }

        public Guid Code { get; set; }

        public string Name { get; set; }

        public long? ParentId { get; set; }

        public DateTimeOffset DateOfCreation { get; set; }

        public DateTimeOffset LastUpdated { get; set; }

        public DateTimeOffset ForceUpdated { get; set; }
    }
}
