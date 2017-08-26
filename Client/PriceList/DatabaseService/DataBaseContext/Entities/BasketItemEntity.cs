﻿using System;

namespace DatabaseService.DataBaseContext.Entities
{
    public class BasketItemEntity
    {
        public long Id { get; set; }

        public virtual CatalogItemEntity CatalogItem { get; set; }

        public virtual OrderEntity Order { get; set; }

        public decimal Count { get; set; }

        public DateTimeOffset DateAction { get; set; }
    }
}
