using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingBrands
    {
        Brands GetItems(string login, DateTimeOffset lastUpdate);

        void ConfirmUpdateBrands(string login, DateTimeOffset lastUpdate, List<long> itemIds);
    }
}
