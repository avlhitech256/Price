using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Domain.Implementation;

namespace PriceListService.Service.Implementation
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "PriceListService" в коде и файле конфигурации.
    public class PriceListService : IPriceListService
    {
        public PriceList UpdatePriceList(string securityString, DateTime? lastUpdateDateTime)
        {
            Console.WriteLine(OperationContext.Current.RequestContext.RequestMessage + "\n");

            PriceList result = new PriceList();
            List<PriceListItem> items = new List<PriceListItem>();

            PriceListItem item = new PriceListItem();
            items.Add(item);
            item.Code = "04157";
            item.VendorCode = "11180-1108054-00";
            item.Manufacturer = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД";
            item.Nomenclature = "Трос акселератора ВАЗ 1118 дв. 1,6л";
            item.AmountInPackage = 10;
            item.Remainder = "+";
            item.Price = 50.52f;
            item.Unit = "шт.";

            item = new PriceListItem();
            items.Add(item);
            item.Code = "09251";
            item.VendorCode = "1119-1108054";
            item.Manufacturer = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД";
            item.Nomenclature = "Трос акселератора ВАЗ 1119 дв. 1,4л";
            item.AmountInPackage = 50;
            item.Remainder = "+";
            item.Price = 55.14f;
            item.Unit = "шт.";

            result.Items = items;
            result.LastModify = DateTime.Now;

            Console.WriteLine("Security string: {0}", securityString);
            Console.WriteLine("Data: {0}", lastUpdateDateTime);

            return result;
        }
    }
}
