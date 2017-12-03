using System;
using System.Collections.Generic;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface IShapingDirectories
    {
        long PrepareToUpdate(string login, DateTimeOffset lastUpdate);

        DirectoryInfo GetItem(long id);

        Directories GetItems(string login, DateTimeOffset lastUpdate);

        void ConfirmUpdate(string login, List<long> itemIds);
    }
}
