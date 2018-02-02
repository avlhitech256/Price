using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingCatalogs
    {
        long PrepareToUpdate(string login, DateTimeOffset lastUpdate);

        CatalogInfo GetItem(string login, long id);

        Catalogs GetItems(string login, DateTimeOffset lastUpdate);

        void ConfirmUpdate(string login, List<long> itemIds);
    }
}
