using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingPhotos
    {
        long PrepareToUpdate(string login, DateTimeOffset lastUpdate);

        PhotoInfo GetItem(long id);

        Photos GetItems(string login, DateTimeOffset lastUpdate);

        void ConfirmUpdate(string login, List<long> itemIds);
    }
}
