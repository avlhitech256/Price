using System;
using System.Collections.Generic;
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

        [OperationContract]
        CountInfo PrepareToUpdate(string login, DateTimeOffset lastUpdate, bool needLoadPhotos);

        [OperationContract]
        BrandInfo GetBrand(long id);

        [OperationContract]
        Brands GetBrands(string login, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdateBrands(string login, DateTimeOffset lastUpdate, List<long> itemIds);

        [OperationContract]
        CatalogInfo GetCatalog(long id);

        [OperationContract]
        Catalogs GetCatalogs(string login, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdateCatalogs(string login, List<long> itemIds);
    }
}
