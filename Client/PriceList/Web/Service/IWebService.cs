﻿using System;
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

        CountInfo PrepareToUpdate(DateTimeOffset lastUpdate, bool needLoadPhotos);

        DirectoryInfo GetDirectoryInfo(long id);

        Directories GetDirectories(DateTimeOffset lastUpdate);

        PhotoInfo GetPhotoInfo(long id);

        Photos GetPhotos(DateTimeOffset lastUpdate);

        ProductDirectionInfo GetProductDirectionInfo(long id);

        ProductDirections GetProductDirections(DateTimeOffset lastUpdate);
    }
}
