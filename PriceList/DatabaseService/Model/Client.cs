﻿using System;
using System.Collections.Generic;

namespace DatabaseService.Model
{
    public class Client
    {
        public long Id { get; set; }
        public Guid UID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<PriceType> TypesOfPrices { get; set; }
        public List<Discount> Discounts { get; set; }

    }
}
