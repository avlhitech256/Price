using System;
using System.Collections.Generic;
using Common.Domain;
using Common.Domain.Implementation;
using Common.ServiceContract;

namespace PriceListConsoleServer.PriceListService
{
    public class PriceListService : IPriceList
    {
        public IEnumerable<IPriceListItem> UpdatePriceList(string securityString, DateTime? lastUpdateDateTime)
        {
            List<IPriceListItem> result = new List<IPriceListItem>();

            IPriceListItem item = new PriceListItem();
            result.Add(item);
            item.Code = "04157";
            item.VendorCode = "11180-1108054-00";
            item.Manufacturer = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД";
            item.Nomenclature = "Трос акселератора ВАЗ 1118 дв. 1,6л";
            item.AmountInPackage = 10;
            item.Remainder = "+";
            item.Price = 50.52f;
            item.Unit = "шт.";

            item = new PriceListItem();
            result.Add(item);
            item.Code = "09251";
            item.VendorCode = "1119-1108054";
            item.Manufacturer = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД";
            item.Nomenclature = "Трос акселератора ВАЗ 1119 дв. 1,4л";
            item.AmountInPackage = 50;
            item.Remainder = "+";
            item.Price = 55.14f;
            item.Unit = "шт.";

            return result;
        }
    }
}
