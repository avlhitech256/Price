using System;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "PricelistService" в коде и файле конфигурации.
    public class PricelistService : IPricelistService
    {
        public CompanyInfo Hello(string login, DateTimeOffset timeRequest)
        {
            CompanyInfo result = new CompanyInfo();

            result.Title = "Hello " + login;
            result.CompanyName = "Autotrend";
            result.Phone = "+380 (57) 784-18-81";
            result.WebSite = "http://autotrend.ua/";
            result.EMail = "office@autotrend.ua";
            result.TimeRequest = timeRequest;
            result.TimeResponce = DateTimeOffset.Now;

            return result;
        }
    }
}
