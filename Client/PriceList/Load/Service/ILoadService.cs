using System;
using Web.WebServiceReference;

namespace Load.Service
{
    public interface ILoadService
    {
        void DownLoadBrandItem(BrandInfo brandInfo);

        void DownLoadBrands(Brands brands);

        void DownLoadCatalogItem(CatalogInfo catalogInfo);

        void DownLoadCatalogs(Catalogs catalogs);

        void DownLoadDirectoryItem(DirectoryInfo directoryInfo);

        void DownLoadDirectories(Directories directories);

        void DownLoadPhotoItem(PhotoInfo photoInfo);

        void DownLoadPhotos(Photos photos);

        void DownLoadProductDirectionItem(ProductDirectionInfo productDirectionInfo);

        void DownLoadProductDirections(ProductDirections productDirections);
    }
}
