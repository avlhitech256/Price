using System;
using System.Collections.Generic;
using Web.WebServiceReference;

namespace Web.Service
{
    public interface IWebService
    {
        bool CheckPassword();

        CompanyInfo Hello();

        ShortcutInfo Shortcut();

        BrandInfo GetBrandInfo(long id);

        Brands GetBrands(DateTimeOffset lastUpdate);

        CatalogInfo GetCatalogInfo(long id);

        Catalogs GetCatalogs(DateTimeOffset lastUpdate);

        CountInfo PrepareToUpdate(DateTimeOffset lastUpdateBrands,
                                  DateTimeOffset lastUpdateCatalogs,
                                  DateTimeOffset lastUpdateDirectories,
                                  DateTimeOffset lastUpdateProductDirections,
                                  DateTimeOffset lastUpdatePhotos,
                                  bool needLoadPhotos,
                                  List<long> ids);

        DirectoryInfo GetDirectoryInfo(long id);

        Directories GetDirectories(DateTimeOffset lastUpdate);

        PhotoInfo GetPhotoInfo(long id);

        Photos GetPhotos(DateTimeOffset lastUpdate, IEnumerable<long> ids);

        ProductDirectionInfo GetProductDirectionInfo(long id);

        ProductDirections GetProductDirections(DateTimeOffset lastUpdate);

        void ConfirmUpdateBrands(IEnumerable<long> itemIds);

        void ConfirmUpdateCatalogs(IEnumerable<long> itemIds);

        void ConfirmUpdateDirectories(IEnumerable<long> itemIds);

        void ConfirmUpdatePhotos(IEnumerable<long> itemIds);

        void ConfirmUpdateProductDirections(IEnumerable<long> itemIds);
    }
}
