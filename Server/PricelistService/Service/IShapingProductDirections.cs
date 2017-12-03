using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingProductDirections
    {
        long PrepareToUpdate(string login, DateTimeOffset lastUpdate);

        ProductDirectionInfo GetItem(long id);

        ProductDirections GetItems(string login, DateTimeOffset lastUpdate);

        void ConfirmUpdate(string login, List<long> itemIds);
    }
}
