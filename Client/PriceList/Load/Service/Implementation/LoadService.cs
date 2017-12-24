using System.Collections.Generic;
using System.Linq;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.DataService;
using Repository.Repository;
using Repository.Repository.Implementation;
using Web.Service;
using Web.WebServiceReference;

namespace Load.Service.Implementation
{
    public class LoadService : ILoadService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IWebService webService;
        private readonly IBrandRepository brandRepository;
        private readonly IDirectoryRepository directoryRepository;

        #endregion

        #region Constructors

        public LoadService(IDataService dataService, 
                           IWebService webService, 
                           IBrandRepository brandRepository,
                           IDirectoryRepository directoryRepository)
        {
            this.dataService = dataService;
            this.webService = webService;
            this.brandRepository = brandRepository;
            this.directoryRepository = directoryRepository;
        }

        #endregion

        #region Properties
        #endregion

        #region Methods

        #region Brends

        private BrandItemEntity GetBrand(long id)
        {
            BrandItemEntity brand = brandRepository.GetItem(id);
            return brand;
        }

        private BrandItemEntity GetBrandWithLoad(long? id)
        {
            BrandItemEntity brandItem = null;

            if (id.HasValue)
            {
                brandItem = GetBrand(id.Value);

                if (brandItem == null)
                {
                    BrandInfo brandInfo = webService.GetBrandInfo(id.Value);

                    if (brandInfo != null)
                    {
                        brandItem = DownLoadBrandItem(brandInfo);

                        if (brandItem != null)
                        {
                            webService.ConfirmUpdateBrands(new[] {brandItem.Id});
                        }
                    }
                }
            }

            return brandItem;
        }

        private BrandItemEntity Create(BrandInfo brandInfo, List<BrandItemEntity> entitiesToInsert = null)
        {
            BrandItemEntity brand = LoadAssembler.Assemble(brandInfo);
            brandRepository.Add(brand);
            entitiesToInsert?.Add(brand);
            return brand;
        }

        private void Update(BrandItemEntity brandItem, BrandInfo brandInfo, List<BrandItemEntity> entitiesToUpdate)
        {
            entitiesToUpdate.Add(brandItem);
            brandItem.Code = brandInfo.Code;
            brandItem.Name = brandInfo.Name;
            brandItem.DateOfCreation = brandInfo.DateOfCreation;
            brandItem.ForceUpdated = brandInfo.ForceUpdated;
            brandItem.LastUpdated = brandInfo.LastUpdated;
        }

        public BrandItemEntity DownLoadBrandItem(BrandInfo brandInfo)
        {
            BrandItemEntity brand = null;

            if (brandInfo != null)
            {
                brand = Create(brandInfo);
                dataService.Insert(brand);
                webService.ConfirmUpdateBrands(new[] {brand.Id});
            }

            return brand;
        }

        public int DownLoadBrands(Brands brands)
        {
            int count = 0;

            if (brands != null && brands.Items != null && brands.Items.Any())
            {
                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

                var entitiesToUpdate = new List<BrandItemEntity>();
                var entitiesToInsert = new List<BrandItemEntity>();

                brands.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        BrandItemEntity oldBrandItem = GetBrand(x.Id);

                        if (oldBrandItem != null)
                        {
                            Update(oldBrandItem, x, entitiesToUpdate);
                        }
                        else
                        {
                            Create(x, entitiesToInsert);
                        }

                    });

                if (entitiesToInsert.Any())
                {
                    dataService.InsertMany(entitiesToInsert);
                }
                else
                {
                    dataService.DataBaseContext.SaveChanges();
                }

                long[] ids = entitiesToInsert
                    .Select(x => x.Id)
                    .Union(entitiesToUpdate
                    .Select(x => x.Id))
                    .Distinct()
                    .ToArray();

                webService.ConfirmUpdateBrands(ids);

                count = ids.Length;

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }

            return count;
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
            DirectoryEntity directory = GetDirectoryWithLoad(catalogInfo.DirectoryId);
            List<PhotoItemEntity> photos = GetPhotosWithLoad(catalogInfo.Photos);
            CatalogItemEntity entity = LoadAssembler.Assemble(catalogInfo, brandItem, photos, directory);
            return entity;
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
                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

                var entities = new List<CatalogItemEntity>();

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
                        }

                    });

                if (entities.Any())
                {
                    dataService.InsertMany(entities);
                }
                else
                {
                    dataService.DataBaseContext.SaveChanges();
                }

                long[] ids =
                    catalogs.Items
                        .Where(x => GetCatalogItem(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateCatalogs(ids);

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        #endregion

        #region Directory

        private DirectoryEntity GetDirectory(long id, List<DirectoryEntity> cacheDirectoryEntity)
        {
            DirectoryEntity directory = directoryRepository.GetItem(id);//cacheDirectoryEntity.FirstOrDefault(x => x.Id == id);

            if (directory == null)
            {
                directory = dataService.DataBaseContext.DirectoryEntities.Find(id);

                if (directory != null)
                {
                    cacheDirectoryEntity.Add(directory);
                }
            }

            return directory;
        }

        private DirectoryEntity GetDirectoryWithLoad(long? id)
        {
            List<DirectoryInfo> cacheDirectoryInfos = new List<DirectoryInfo>();
            List<DirectoryEntity> cacheDirectoryEntities = new List<DirectoryEntity>();
            List<DirectoryEntity> entitiesToInsert = new List<DirectoryEntity>();
            DirectoryEntity entity = GetDirectoryWithLoad(id, cacheDirectoryInfos, cacheDirectoryEntities, entitiesToInsert);
            dataService.DataBaseContext.DirectoryEntities.AddRange(entitiesToInsert);
            return entity;
        }

        private DirectoryEntity GetDirectoryWithLoad(long? id, 
                                                     List<DirectoryInfo> cacheDirectoryInfos, 
                                                     List<DirectoryEntity> cacheDirectoryEntities,
                                                     List<DirectoryEntity> entitiesToInsert)
        {
            DirectoryEntity directory = id.HasValue ? GetDirectory(id.Value, cacheDirectoryEntities) : null;

            if (id.HasValue && directory == null)
            {
                DirectoryInfo directoryInfo = cacheDirectoryInfos?.FirstOrDefault(x => x.Id == id.Value);

                if (directoryInfo == null)
                {
                    directoryInfo = webService.GetDirectoryInfo(id.Value);

                    if (directoryInfo != null)
                    {
                        cacheDirectoryInfos?.Add(directoryInfo);
                    }
                }

                if (directoryInfo != null)
                {
                    directory = Create(directoryInfo, cacheDirectoryInfos, cacheDirectoryEntities, entitiesToInsert);
                }
            }

            return directory;
        }

        private DirectoryEntity Create(DirectoryInfo directoryInfo, 
                                       List<DirectoryInfo> cacheDirectoryInfos, 
                                       List<DirectoryEntity> cacheDirectoryEntities,
                                       List<DirectoryEntity> entitiesToInsert)
        {
            DirectoryEntity directory = null;

            if (directoryInfo != null)
            {
                directory = LoadAssembler.Assemble(directoryInfo, null, new List<DirectoryEntity>());
                directoryRepository.Add(directory);
                entitiesToInsert.Add(directory);
                cacheDirectoryEntities.Add(directory);
                directory.Parent = GetDirectoryWithLoad(directoryInfo.Parent, 
                                                        cacheDirectoryInfos, 
                                                        cacheDirectoryEntities, 
                                                        entitiesToInsert);

                directoryInfo.SubDirectoryIds.ToList().ForEach(
                    x =>
                    {
                        DirectoryEntity subDirectory = GetDirectoryWithLoad(x, 
                                                                            cacheDirectoryInfos, 
                                                                            cacheDirectoryEntities, 
                                                                            entitiesToInsert);

                        if (subDirectory != null)
                        {
                            directory.SubDirectory.Add(subDirectory);
                        }
                    });
            }

            return directory;
        }

        private void Update(DirectoryEntity entity, 
                            DirectoryInfo directoryInfo, 
                            List<DirectoryInfo> cacheDirectoryInfos, 
                            List<DirectoryEntity> cacheDirectoryEntities,
                            List<DirectoryEntity> entitiesToInsert)
        {
            if (entity != null && directoryInfo != null && entity.Id == directoryInfo.Id)
            {
                DirectoryEntity parent = GetDirectoryWithLoad(directoryInfo.Parent, 
                                                              cacheDirectoryInfos, 
                                                              cacheDirectoryEntities,
                                                              entitiesToInsert);
                List<DirectoryEntity> subDirectories = new List<DirectoryEntity>();

                directoryInfo.SubDirectoryIds.ToList().ForEach(
                    x =>
                    {
                        DirectoryEntity subDirectory = GetDirectoryWithLoad(x, 
                                                                            cacheDirectoryInfos, 
                                                                            cacheDirectoryEntities,
                                                                            entitiesToInsert);

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
        }

        public void DownLoadDirectoryItem(DirectoryInfo directoryInfo)
        {
            DownLoadDirectoryItem(directoryInfo, 
                                  new List<DirectoryInfo> { directoryInfo }, 
                                  new List<DirectoryEntity>(), 
                                  new List<DirectoryEntity>());
        }

        private void DownLoadDirectoryItem(DirectoryInfo directoryInfo, 
                                           List<DirectoryInfo> cacheDirectoryInfos, 
                                           List<DirectoryEntity> cacheDirectoryEntities,
                                           List<DirectoryEntity> entitiesToInsert)
        {
            dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            DirectoryEntity oldDirectory = GetDirectory(directoryInfo.Id, cacheDirectoryEntities);

            if (oldDirectory != null)
            {
                Update(oldDirectory, directoryInfo, cacheDirectoryInfos, cacheDirectoryEntities, entitiesToInsert);
            }
            else
            {
                Create(directoryInfo, cacheDirectoryInfos, cacheDirectoryEntities, entitiesToInsert);
            }

            if (entitiesToInsert.Any())
            {
                dataService.InsertMany(entitiesToInsert);
            }
            else
            {
                dataService.DataBaseContext.SaveChanges();
            }

            if (cacheDirectoryEntities.Any())
            {
                webService.ConfirmUpdateDirectories(cacheDirectoryEntities.Select(x => x.Id));
            }

            dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
            dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
        }

        public int DownLoadDirectories(Directories directories)
        {
            int count = 0;

            if (directories != null && directories.Items != null && directories.Items.Any())
            {
                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

                List<DirectoryInfo> cacheDirectoryInfos = directories.Items.ToList();
                var cacheDirectoryEntities = new List<DirectoryEntity>();
                var entitiesToInsert = new List<DirectoryEntity>();

                directories.Items.Where(x => x != null).ToList().ForEach(
                    x =>
                    {
                        DirectoryEntity oldDirectoryItem = GetDirectory(x.Id, cacheDirectoryEntities);

                        if (oldDirectoryItem != null)
                        {
                            Update(oldDirectoryItem, x, cacheDirectoryInfos, cacheDirectoryEntities, entitiesToInsert);
                        }
                        else
                        {
                            Create(x, cacheDirectoryInfos, cacheDirectoryEntities, entitiesToInsert);
                        }

                    });

                if (entitiesToInsert.Any())
                {
                    dataService.InsertMany(entitiesToInsert);
                }
                else
                {
                    dataService.DataBaseContext.SaveChanges();
                }

                long[] ids =
                    cacheDirectoryEntities
                        .Where(x => x != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateDirectories(ids);

                count = ids.Length;

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }

            return count;
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
                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

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

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
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
            DirectoryEntity directory = GetDirectoryWithLoad(brandInfo.Id);
            ProductDirectionEntity brand = LoadAssembler.Assemble(brandInfo, directory);
            return brand;
        }

        private void Update(ProductDirectionEntity brandItem, ProductDirectionInfo brandInfo)
        {
            brandItem.Direction = LoadAssembler.Convert(brandInfo.Direction);
            brandItem.Directory = GetDirectoryWithLoad(brandInfo.Id);
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
                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

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

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        #endregion

        #endregion
    }
}
