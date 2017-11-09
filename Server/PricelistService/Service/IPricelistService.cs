using System;
using System.ServiceModel;
using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IPricelistService" в коде и файле конфигурации.
    [ServiceContract]
    public interface IPricelistService
    {
        [OperationContract]
        CompanyInfo Hello(string login, DateTimeOffset timeRequest);
    }
}
