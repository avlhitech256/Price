using System;
using System.ServiceModel;
using Common.Domain.Implementation;

namespace PriceListService.Service
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IPriceListService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IPriceListService
    {
        [OperationContract]
        PriceList UpdatePriceList(string securityString, DateTime? lastUpdateDateTime);
    }
}
