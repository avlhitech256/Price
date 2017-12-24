using DatabaseService.DataBaseContext.Entities;
using Web.WebServiceReference;

namespace Load.Service
{
    public interface ILoadService
    {
        BrandItemEntity DownLoadBrandItem(BrandInfo brandInfo);

        int DownLoadBrands(Brands brands);

        void DownLoadCatalogItem(CatalogInfo catalogInfo);

        void DownLoadCatalogs(Catalogs catalogs);

        void DownLoadDirectoryItem(DirectoryInfo directoryInfo);

        int DownLoadDirectories(Directories directories);

        void DownLoadPhotoItem(PhotoInfo photoInfo);

        int DownLoadPhotos(Photos photos);

        void DownLoadProductDirectionItem(ProductDirectionInfo productDirectionInfo);

        void DownLoadProductDirections(ProductDirections productDirections);
    }
}
