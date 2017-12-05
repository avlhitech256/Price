using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Web.Service;
using Web.WebServiceReference;

namespace Load.Service.Implementation
{
    public class LoadService : ILoadService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IWebService webService;

        #endregion

        #region Constructors

        public LoadService(IDataService dataService, IWebService webService)
        {
            this.dataService = dataService;
            this.webService = webService;
        }

        #endregion

        #region Properties
        #endregion

        #region Methods

        public void DownLoadBrandItem(BrandInfo brandInfo)
        {
            BrandItemEntity oldBrandItem = GetBrand(brandInfo.Id);

            if (oldBrandItem != null)
            {
                Update(oldBrandItem, brandInfo);
            }
            else
            {
                BrandItemEntity brand = Create(brandInfo);
                dataService.Insert(brand);
            }
        }

        private void Update(BrandItemEntity brandItem, BrandInfo brandInfo)
        {
            brandItem.Code = brandInfo.Code;
            brandItem.Name = brandInfo.Name;
            brandItem.DateOfCreation = brandInfo.DateOfCreation;
            brandItem.ForceUpdated = brandInfo.ForceUpdated;
            brandItem.LastUpdated = brandInfo.LastUpdated;

            dataService.DataBaseContext.SaveChanges();
        }

        private BrandItemEntity Create(BrandInfo brandInfo)
        {
            BrandItemEntity brand = LoadAssembler.Assemble(brandInfo);
            return brand;
        }

        public void DownLoadBrands(Brands brands)
        {
            var entities = new List<BrandItemEntity>();
            brands.Items.ToList().ForEach(x => entities.Add(Create(x)));
            dataService.InsertMany(entities);
        }

        public void DownLoadCatalogItem(CatalogInfo catalogInfo)
        {
            CatalogItemEntity oldEntity = GetCatalogItem(catalogInfo.Id);

            if (oldEntity != null)
            {
                Update(oldEntity, catalogInfo);
            }
            else
            {
                CatalogItemEntity entity = Create(catalogInfo);
                dataService.DataBaseContext.CatalogItemEntities.Add(entity);
                dataService.Insert(entity);
            }
        }

        private void Update(CatalogItemEntity entity, CatalogInfo catalogInfo)
        {
            BrandItemEntity brandItem = GetBrandWithLoad(catalogInfo.BrandId);
            DirectoryEntity directory = GetDirectoryWithLoad(catalogInfo.DirectoryId);
            List<PhotoItemEntity> photos = GetPhotosWithLoad(catalogInfo.Photos);

            entity.Id = catalogInfo.Id;
            entity.UID = catalogInfo.UID;
            entity.Code = catalogInfo.Code;
            entity.Article = catalogInfo.Article;
            entity.Brand = brandItem;
            entity.BrandName = brandItem?.Name;
            entity.Name = catalogInfo.Name;
            entity.Unit = catalogInfo.Unit;
            entity.EnterpriceNormPack = catalogInfo.EnterpriceNormPack;
            entity.BatchOfSales = catalogInfo.BatchOfSales;
            entity.Balance = catalogInfo.Balance;
            entity.Price = catalogInfo.Price;
            entity.Currency = catalogInfo.Currency;
            entity.Multiplicity = catalogInfo.Multiplicity;
            entity.HasPhotos = catalogInfo.HasPhotos;
            entity.Photos = photos;
            entity.DateOfCreation = catalogInfo.DateOfCreation;
            entity.LastUpdated = catalogInfo.LastUpdated;
            entity.ForceUpdated = catalogInfo.ForceUpdated;
            entity.Status = LoadAssembler.Convert(catalogInfo.Status);
            entity.LastUpdatedStatus = catalogInfo.LastUpdatedStatus;
            entity.Directory = directory;

            dataService.DataBaseContext.SaveChanges();
        }

        private CatalogItemEntity GetCatalogItem(long id)
        {
            CatalogItemEntity entity = dataService.DataBaseContext.CatalogItemEntities.Find(id);
            return entity;
        }

        private CatalogItemEntity Create(CatalogInfo catalogInfo)
        {
            BrandItemEntity brandItem = GetBrandWithLoad(catalogInfo.BrandId);
            DirectoryEntity directory = GetDirectoryWithLoad(catalogInfo.DirectoryId);
            List<PhotoItemEntity> photos = GetPhotosWithLoad(catalogInfo.Photos);
            CatalogItemEntity entity = LoadAssembler.Assemble(catalogInfo, brandItem, photos, directory);
            return entity;
        }

        private BrandItemEntity GetBrand(long id)
        {
            BrandItemEntity brand = dataService.DataBaseContext.BrandItemEntities.Find(id);
            return brand;
        }

        private BrandItemEntity GetBrandWithLoad(long id)
        {
            BrandItemEntity brandItem = GetBrand(id);

            if (brandItem == null)
            {
                BrandInfo brandInfo = webService.GetBrandInfo(id);

                if (brandInfo != null)
                {
                    DownLoadBrandItem(brandInfo);
                    brandItem = GetBrand(id);
                }
            }

            return brandItem;
        }

        private DirectoryEntity GetDirectory(long id)
        {
            DirectoryEntity directory = dataService.DataBaseContext.DirectoryEntities.Find(id);
            return directory;
        }

        private DirectoryEntity GetDirectoryWithLoad(long id)
        {
            DirectoryEntity directory = GetDirectory(id);

            if (directory == null)
            {
                DirectoryInfo directoryInfo = webService.GetDirectoryInfo(id);

                if (directoryInfo != null)
                {
                    DownLoadDirectoryItem(directoryInfo);
                    directory = GetDirectory(id);
                }
            }

            return directory;
        }

        private PhotoItemEntity GetPhoto(long id)
        {
            PhotoItemEntity photo = dataService.DataBaseContext.PhotoItemEntities.Find(id);
            return photo;
        }

        private List<PhotoItemEntity> GetPhotos(long[] ids)
        {
            List<PhotoItemEntity> photos =
                dataService.DataBaseContext.PhotoItemEntities.Where(x => ids.Contains(x.Id)).ToList();
            return photos;
        }

        private List<PhotoItemEntity> GetPhotosWithLoad(long[] ids)
        {
            List<PhotoItemEntity> photos = GetPhotos(ids);

            if (photos.Count != ids.Length)
            {
                List<long> needToLoadPhotos = ids.Where(x => photos.All(p => p.Id != x)).ToList();

                foreach (long id in needToLoadPhotos)
                {
                    PhotoInfo photoInfo = webService.GetPhotoInfo(id);
                    DownLoadPhotoItem(photoInfo);
                    PhotoItemEntity photo = dataService.DataBaseContext.PhotoItemEntities.Find(id);

                    if (photo != null)
                    {
                        photos.Add(photo);
                    }
                }
            }

            return photos;
        }

        public void DownLoadCatalogs(Catalogs catalogs)
        {
            var entities = new List<CatalogItemEntity>();
            catalogs.Items.ToList().ForEach(x => entities.Add(Create(x)));
            dataService.DataBaseContext.CatalogItemEntities.AddRange(entities);
            dataService.InsertMany(entities);
        }

        public void DownLoadDirectoryItem(DirectoryInfo directoryInfo)
        {
            throw new System.NotImplementedException();
        }


        public void DownLoadDirectories(Directories directories)
        {
            throw new System.NotImplementedException();
        }

        public void DownLoadPhotoItem(PhotoInfo photoInfo)
        {
            PhotoItemEntity entity = LoadAssembler.Assemble(photoInfo);
            dataService.DataBaseContext.PhotoItemEntities.Add(entity);
            dataService.Insert(entity);
        }

        public void DownLoadPhotos(Photos photos)
        {
            throw new System.NotImplementedException();
        }

        public void DownLoadProductDirectionItem(ProductDirectionInfo productDirectionInfo)
        {
            throw new System.NotImplementedException();
        }

        public void DownLoadProductDirections(ProductDirections productDirections)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
