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
        CompanyInfo Hello(SecurityInfo securityInfo);

        [OperationContract]
        ShortcutInfo Shortcut(long id, DateTimeOffset requestTime);

        [OperationContract]
        bool ChangePasswodr(SecurityInfo securityInfo, string newPassword);
        
        [OperationContract]
        CountInfo PrepareToUpdate(SecurityInfo securityInfo, DateTimeOffset lastUpdate, bool needLoadPhotos);

        [OperationContract]
        BrandInfo GetBrand(SecurityInfo securityInfo, long id);

        [OperationContract]
        Brands GetBrands(SecurityInfo securityInfo, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdateBrands(SecurityInfo securityInfo, DateTimeOffset lastUpdate, List<long> itemIds);

        [OperationContract]
        CatalogInfo GetCatalog(SecurityInfo securityInfo, long id);

        [OperationContract]
        Catalogs GetCatalogs(SecurityInfo securityInfo, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdateCatalogs(SecurityInfo securityInfo, List<long> itemIds);

        [OperationContract]
        DirectoryInfo GetDirectory(SecurityInfo securityInfo, long id);

        [OperationContract]
        Directories GetDirectories(SecurityInfo securityInfo, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdateDirectories(SecurityInfo securityInfo, List<long> itemIds);

        [OperationContract]
        PhotoInfo GetPhoto(SecurityInfo securityInfo, long id);

        [OperationContract]
        Photos GetPhotos(SecurityInfo securityInfo, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdatePhotos(SecurityInfo securityInfo, List<long> itemIds);

        [OperationContract]
        ProductDirectionInfo GetProductDirection(SecurityInfo securityInfo, long id);

        [OperationContract]
        ProductDirections GetProductDirections(SecurityInfo securityInfo, DateTimeOffset lastUpdate);

        [OperationContract]
        void ConfirmUpdateProductDirections(SecurityInfo securityInfo, List<long> itemIds);
    }
}
