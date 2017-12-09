using System;
using Web.WebServiceReference;

namespace Load.Service
{
    public interface ILoadService
    {
        void DownLoadBrandItem(BrandInfo brandInfo, DateTimeOffset lastUpdate);

        void DownLoadBrands(Brands brands, DateTimeOffset lastUpdate);

        void DownLoadCatalogItem(CatalogInfo catalogInfo, DateTimeOffset lastUpdate);

        void DownLoadCatalogs(Catalogs catalogs, DateTimeOffset lastUpdate);

        void DownLoadDirectoryItem(DirectoryInfo directoryInfo);

        void DownLoadDirectories(Directories directories);

        void DownLoadPhotoItem(PhotoInfo photoInfo);

        void DownLoadPhotos(Photos photos);

        void DownLoadProductDirectionItem(ProductDirectionInfo productDirectionInfo);

        void DownLoadProductDirections(ProductDirections productDirections);
    }
}
