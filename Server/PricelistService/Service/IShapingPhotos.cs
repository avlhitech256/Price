using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingPhotos
    {
        long PrepareToUpdate(string login, DateTimeOffset lastUpdate, long[] ids);

        PhotoInfo GetItem(long id);

        Photos GetItems(string login, DateTimeOffset lastUpdate, long[] ids);

        void ConfirmUpdate(string login, List<long> itemIds);
    }
}
