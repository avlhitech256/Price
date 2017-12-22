using System;
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
        private readonly List<DirectoryInfo> cacheDirectoryInfo;
        private readonly List<DirectoryEntity> cacheDirectoryEntity;

        #endregion

        #region Constructors

        public LoadService(IDataService dataService, IWebService webService)
        {
            this.dataService = dataService;
            this.webService = webService;
            cacheDirectoryInfo = new List<DirectoryInfo>();
            cacheDirectoryEntity = new List<DirectoryEntity>();
        }

        #endregion

        #region Properties
        #endregion

        #region Methods

        public void Init()
        {
            cacheDirectoryInfo.Clear();
            cacheDirectoryEntity.Clear();
        }

        #region Brends

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

                    if (brandItem != null)
                    {
                        webService.ConfirmUpdateBrands(new [] { brandItem.Id });
                    }
                }
            }

            return brandItem;
        }

        private BrandItemEntity Create(BrandInfo brandInfo)
        {
            BrandItemEntity brand = LoadAssembler.Assemble(brandInfo);
            return brand;
        }

        private void Update(BrandItemEntity brandItem, BrandInfo brandInfo)
        {
            brandItem.Code = brandInfo.Code;
            brandItem.Name = brandInfo.Name;
            brandItem.DateOfCreation = brandInfo.DateOfCreation;
            brandItem.ForceUpdated = brandInfo.ForceUpdated;
            brandItem.LastUpdated = brandInfo.LastUpdated;
        }

        public void DownLoadBrandItem(BrandInfo brandInfo)
        {
            if (brandInfo != null)
            {
                BrandItemEntity oldBrandItem = GetBrand(brandInfo.Id);

                if (oldBrandItem != null)
                {
                    Update(oldBrandItem, brandInfo);
                    dataService.DataBaseContext.SaveChanges();
                    webService.ConfirmUpdateBrands(new[] {oldBrandItem.Id});
                }
                else
                {
                    BrandItemEntity brand = Create(brandInfo);
                    dataService.Insert(brand);
                    brand = GetBrand(brandInfo.Id);

                    if (brand != null)
                    {
                        webService.ConfirmUpdateBrands(new[] { brand.Id });
                    }
                }
            }
        }

        public void DownLoadBrands(Brands brands)
        {
            if (brands != null && brands.Items != null && brands.Items.Any())
            {
                var entities = new List<BrandItemEntity>();
                bool needToSave = true;

                brands.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        BrandItemEntity oldBrandItem = GetBrand(x.Id);

                        if (oldBrandItem != null)
                        {
                            Update(oldBrandItem, x);
                        }
                        else
                        {
                            entities.Add(Create(x));
                            needToSave = false;
                        }

                    });

                if (needToSave)
                {
                    dataService.DataBaseContext.SaveChanges();
                }
                else
                {
                    dataService.InsertMany(entities);
                }

                long[] ids =
                    brands.Items
                        .Where(x => GetBrand(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateBrands(ids);
            }
        }

        #endregion

        #region Catalogs

        private CatalogItemEntity GetCatalogItem(long id)
        {
            CatalogItemEntity entity = dataService.DataBaseContext.CatalogItemEntities.Find(id);
            return entity;
        }

        private CatalogItemEntity Create(CatalogInfo catalogInfo)
        {
            BrandItemEntity brandItem = GetBrandWithLoad(catalogInfo.BrandId);
            DirectoryEntity directory = GetDirectoryWithLoad(catalogInfo.DirectoryId, null);
            List<PhotoItemEntity> photos = GetPhotosWithLoad(catalogInfo.Photos);
            CatalogItemEntity entity = LoadAssembler.Assemble(catalogInfo, brandItem, photos, directory);
            return entity;
        }

        private void Update(CatalogItemEntity entity, CatalogInfo catalogInfo)
        {
            BrandItemEntity brandItem = GetBrandWithLoad(catalogInfo.BrandId);
            DirectoryEntity directory = GetDirectoryWithLoad(catalogInfo.DirectoryId, null);
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
        }

        public void DownLoadCatalogItem(CatalogInfo catalogInfo)
        {
            CatalogItemEntity oldEntity = GetCatalogItem(catalogInfo.Id);

            if (oldEntity != null)
            {
                Update(oldEntity, catalogInfo);
                dataService.DataBaseContext.SaveChanges();
                webService.ConfirmUpdateCatalogs(new[] { oldEntity.Id });
            }
            else
            {
                CatalogItemEntity entity = Create(catalogInfo);
                dataService.Insert(entity);
                entity = dataService.DataBaseContext.CatalogItemEntities.Find(catalogInfo.Id);

                if (entity != null)
                {
                    webService.ConfirmUpdateCatalogs(new[] { catalogInfo.Id });
                }
            }
        }

        public void DownLoadCatalogs(Catalogs catalogs)
        {
            if (catalogs != null && catalogs.Items != null && catalogs.Items.Any())
            {
                var entities = new List<CatalogItemEntity>();
                bool needToSave = true;

                catalogs.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        CatalogItemEntity oldCatalogItem = GetCatalogItem(x.Id);

                        if (oldCatalogItem != null)
                        {
                            Update(oldCatalogItem, x);
                        }
                        else
                        {
                            entities.Add(Create(x));
                            needToSave = false;
                        }

                    });

                if (needToSave)
                {
                    dataService.DataBaseContext.SaveChanges();
                }
                else
                {
                    dataService.InsertMany(entities);
                }

                long[] ids =
                    catalogs.Items
                        .Where(x => GetCatalogItem(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateBrands(ids);
            }
        }

        #endregion

        #region Directory

        private DirectoryEntity GetDirectory(long id)
        {
            DirectoryEntity directory = cacheDirectoryEntity.FirstOrDefault(x => x.Id == id);

            if (directory == null)
            {
                directory = dataService.DataBaseContext.DirectoryEntities.Find(id);
            }

            return directory;
        }

        private DirectoryEntity GetDirectoryWithLoad(long? id, Directories directories)
        {
            DirectoryEntity directory = id.HasValue ? GetDirectory(id.Value) : null;

            if (id.HasValue && directory == null)
            {
                DirectoryInfo directoryInfo = null;

                if (directories?.Items != null && directories.Items.Any())
                {
                    directoryInfo = directories.Items.FirstOrDefault(x => x.Id == id.Value);
                }

                if (directoryInfo == null)
                {
                    directoryInfo = cacheDirectoryInfo.FirstOrDefault(x => x.Id == id.Value);

                }

                if (directoryInfo == null)
                {
                    directoryInfo = webService.GetDirectoryInfo(id.Value);

                    if (directoryInfo != null)
                    {
                        cacheDirectoryInfo.Add(directoryInfo);
                    }
                }

                if (directoryInfo != null)
                {
                    DownLoadDirectoryItem(directoryInfo, directories);
                    directory = GetDirectory(id.Value);

                    if (directory != null)
                    {
                        webService.ConfirmUpdateDirectories(new[] { directory.Id });
                    }
                }
            }

            return directory;
        }

        private DirectoryEntity Create(DirectoryInfo directoryInfo, Directories directories)
        {
            DirectoryEntity directory = LoadAssembler.Assemble(directoryInfo, null, new List<DirectoryEntity>());
            cacheDirectoryEntity.Add(directory);
            directory.Parent = GetDirectoryWithLoad(directoryInfo.Parent, directories);

            directoryInfo.SubDirectoryIds.ToList().ForEach(
                x =>
                {
                    DirectoryEntity subDirectory = GetDirectoryWithLoad(x, directories);

                    if (subDirectory != null)
                    {
                        directory.SubDirectory.Add(subDirectory);
                    }
                });

            return directory;
        }

        private void Update(DirectoryEntity entity, DirectoryInfo directoryInfo, Directories directories)
        {
            DirectoryEntity parent = GetDirectoryWithLoad(directoryInfo.Parent, directories);
            List<DirectoryEntity> subDirectories = new List<DirectoryEntity>();

            directoryInfo.SubDirectoryIds.ToList().ForEach(
                x =>
                {
                    DirectoryEntity subDirectory = GetDirectoryWithLoad(x, directories);

                    if (subDirectory != null)
                    {
                        subDirectories.Add(subDirectory);
                    }
                });

            entity.Code = directoryInfo.Code;
            entity.Name = directoryInfo.Name;
            entity.Parent = parent;
            entity.SubDirectory = subDirectories;
            entity.DateOfCreation = directoryInfo.DateOfCreation;
            entity.LastUpdated = directoryInfo.LastUpdated;
            entity.ForceUpdated = directoryInfo.ForceUpdated;
        }

        public void DownLoadDirectoryItem(DirectoryInfo directoryInfo)
        {
            DownLoadDirectoryItem(directoryInfo, null);
        }

        private void DownLoadDirectoryItem(DirectoryInfo directoryInfo, Directories directories)
        {
            DirectoryEntity oldDirectory = GetDirectory(directoryInfo.Id);

            if (oldDirectory != null)
            {
                Update(oldDirectory, directoryInfo, directories);
                dataService.DataBaseContext.SaveChanges();
                webService.ConfirmUpdateDirectories(new[] { oldDirectory.Id });
            }
            else
            {
                DirectoryEntity entity = Create(directoryInfo, directories);
                dataService.Insert(entity);
                entity = GetDirectory(directoryInfo.Id);

                if (entity != null)
                {
                    webService.ConfirmUpdateDirectories(new[] { entity.Id });
                }
            }
        }

        public void DownLoadDirectories(Directories directories)
        {
            if (directories != null && directories.Items != null && directories.Items.Any())
            {
                var entities = new List<DirectoryEntity>();
                bool needToSave = true;

                directories.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        DirectoryEntity oldDirectoryItem = GetDirectory(x.Id);

                        if (oldDirectoryItem != null)
                        {
                            Update(oldDirectoryItem, x, directories);
                        }
                        else
                        {
                            entities.Add(Create(x, directories));
                            needToSave = false;
                        }

                    });

                if (needToSave)
                {
                    dataService.DataBaseContext.SaveChanges();
                }
                else
                {
                    dataService.InsertMany(entities);
                }

                long[] ids =
                    directories.Items
                        .Where(x => GetDirectory(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateDirectories(ids);
            }
        }

        #endregion

        #region Photos

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
                List<long> loadedPhotoIds = new List<long>();

                foreach (long id in needToLoadPhotos)
                {
                    PhotoInfo photoInfo = webService.GetPhotoInfo(id);
                    DownLoadPhotoItem(photoInfo);
                    PhotoItemEntity photo = dataService.DataBaseContext.PhotoItemEntities.Find(id);

                    if (photo != null)
                    {
                        photos.Add(photo);
                        loadedPhotoIds.Add(photo.Id);
                    }
                }

                if (loadedPhotoIds.Any())
                {
                    webService.ConfirmUpdatePhotos(loadedPhotoIds);
                }
            }

            return photos;
        }

        private PhotoItemEntity Create(PhotoInfo photoInfo)
        {
            PhotoItemEntity entity = LoadAssembler.Assemble(photoInfo);
            return entity;
        }

        private void Update(PhotoItemEntity photoItem, PhotoInfo photoInfo)
        {
            photoItem.Name = photoInfo.Name;
            photoItem.IsLoad = photoInfo.IsLoad;
            photoItem.Photo = photoInfo.Photo;
            photoItem.DateOfCreation = photoInfo.DateOfCreation;
            photoItem.ForceUpdated = photoInfo.ForceUpdated;
            photoItem.LastUpdated = photoInfo.LastUpdated;
        }

        public void DownLoadPhotoItem(PhotoInfo photoInfo)
        {
            PhotoItemEntity oldPhoto = GetPhoto(photoInfo.Id);

            if (oldPhoto != null)
            {
                Update(oldPhoto, photoInfo);
                dataService.DataBaseContext.SaveChanges();
                webService.ConfirmUpdatePhotos(new[] { oldPhoto.Id });

            }
            else
            {
                PhotoItemEntity entity = Create(photoInfo);
                dataService.Insert(entity);
                entity = GetPhoto(photoInfo.Id);

                if (entity != null)
                {
                    webService.ConfirmUpdatePhotos(new[] { entity.Id });
                }
            }
        }

        public void DownLoadPhotos(Photos photos)
        {
            if (photos != null && photos.Items != null && photos.Items.Any())
            {
                var entities = new List<PhotoItemEntity>();
                bool needToSave = true;

                photos.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        PhotoItemEntity oldPhotoItem = GetPhoto(x.Id);

                        if (oldPhotoItem != null)
                        {
                            Update(oldPhotoItem, x);
                        }
                        else
                        {
                            entities.Add(Create(x));
                            needToSave = false;
                        }

                    });

                if (needToSave)
                {
                    dataService.DataBaseContext.SaveChanges();
                }
                else
                {
                    dataService.InsertMany(entities);
                }

                long[] ids =
                    photos.Items
                        .Where(x => GetPhoto(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateBrands(ids);
            }
        }

        #endregion

        #region ProductDirection

        private ProductDirectionEntity GetProductDirection(long id)
        {
            ProductDirectionEntity productDirection = dataService.DataBaseContext.ProductDirectionEntities.Find(id);
            return productDirection;
        }

        private ProductDirectionEntity GetProductDirectionWithLoad(long id)
        {
            ProductDirectionEntity productDirectionItem = GetProductDirection(id);

            if (productDirectionItem == null)
            {
                ProductDirectionInfo productDirectionInfo = webService.GetProductDirectionInfo(id);

                if (productDirectionInfo != null)
                {
                    DownLoadProductDirectionItem(productDirectionInfo);
                    productDirectionItem = GetProductDirection(id);

                    if (productDirectionItem != null)
                    {
                        webService.ConfirmUpdateProductDirections(new[] { productDirectionItem.Id });
                    }
                }
            }

            return productDirectionItem;
        }

        private ProductDirectionEntity Create(ProductDirectionInfo brandInfo)
        {
            DirectoryEntity directory = GetDirectoryWithLoad(brandInfo.Id, null);
            ProductDirectionEntity brand = LoadAssembler.Assemble(brandInfo, directory);
            return brand;
        }

        private void Update(ProductDirectionEntity brandItem, ProductDirectionInfo brandInfo)
        {
            brandItem.Direction = LoadAssembler.Convert(brandInfo.Direction);
            brandItem.Directory = GetDirectoryWithLoad(brandInfo.Id, null);
            brandItem.DateOfCreation = brandInfo.DateOfCreation;
            brandItem.ForceUpdated = brandInfo.ForceUpdated;
            brandItem.LastUpdated = brandInfo.LastUpdated;
        }


        public void DownLoadProductDirectionItem(ProductDirectionInfo productDirectionInfo)
        {
            if (productDirectionInfo != null)
            {
                ProductDirectionEntity oldProductDirectionItem = GetProductDirection(productDirectionInfo.Id);

                if (oldProductDirectionItem != null)
                {
                    Update(oldProductDirectionItem, productDirectionInfo);
                    dataService.DataBaseContext.SaveChanges();
                    webService.ConfirmUpdateBrands(new[] { oldProductDirectionItem.Id });
                }
                else
                {
                    ProductDirectionEntity productDirectionItem = Create(productDirectionInfo);
                    dataService.Insert(productDirectionItem);
                    productDirectionItem = GetProductDirection(productDirectionInfo.Id);

                    if (productDirectionItem != null)
                    {
                        webService.ConfirmUpdateBrands(new[] { productDirectionItem.Id });
                    }
                }
            }
        }

        public void DownLoadProductDirections(ProductDirections productDirections)
        {
            if (productDirections != null && productDirections.Items != null && productDirections.Items.Any())
            {
                var entities = new List<ProductDirectionEntity>();
                bool needToSave = true;

                productDirections.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        ProductDirectionEntity oldProductDirectionItem = GetProductDirection(x.Id);

                        if (oldProductDirectionItem != null)
                        {
                            Update(oldProductDirectionItem, x);
                        }
                        else
                        {
                            entities.Add(Create(x));
                            needToSave = false;
                        }

                    });

                if (needToSave)
                {
                    dataService.DataBaseContext.SaveChanges();
                }
                else
                {
                    dataService.InsertMany(entities);
                }

                long[] ids =
                    productDirections.Items
                        .Where(x => GetProductDirection(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateProductDirections(ids);
            }
        }

        #endregion

        #endregion
    }
}
