using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingBrands
    {
        int PrepareToUpdate(string login, DateTimeOffset lastUpdate);

        BrandInfo GetItem(long id);

        Brands GetItems(string login, DateTimeOffset lastUpdate);

        void ConfirmUpdate(string login, List<long> itemIds);
    }
}
