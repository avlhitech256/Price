using DatabaseService.DataBaseContext.Entities;
using Web.WebServiceReference;

namespace Load.Service
{
    public interface ILoadService
    {
        BrandItemEntity DownLoadBrandItem(BrandInfo brandInfo);

        void DownLoadBrands(Brands brands);

        void DownLoadCatalogItem(CatalogInfo catalogInfo);

        void DownLoadCatalogs(Catalogs catalogs);

        void DownLoadDirectoryItem(DirectoryInfo directoryInfo);

        void DownLoadDirectories(Directories directories);

        void DownLoadPhotoItem(PhotoInfo photoInfo);

        int DownLoadPhotos(Photos photos);

        void DownLoadProductDirections(ProductDirections productDirections);
    }
}
