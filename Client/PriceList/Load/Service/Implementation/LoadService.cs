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

        #region Brends

        private BrandItemEntity GetBrand(long id)
        {
            BrandItemEntity brand = dataService.DataBaseContext.BrandItemEntities.Find(id);
            return brand;
        }

        private BrandItemEntity GetBrandWithLoad(long id, DateTimeOffset lastUpdate)
        {
            BrandItemEntity brandItem = GetBrand(id);

            if (brandItem == null)
            {
                BrandInfo brandInfo = webService.GetBrandInfo(id);

                if (brandInfo != null)
                {
                    DownLoadBrandItem(brandInfo, lastUpdate);
                    brandItem = GetBrand(id);

                    if (brandItem != null)
                    {
                        webService.ConfirmUpdateBrands(lastUpdate, new [] { brandItem.Id });
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

        public void DownLoadBrandItem(BrandInfo brandInfo, DateTimeOffset lastUpdate)
        {
            if (brandInfo != null)
            {
                BrandItemEntity oldBrandItem = GetBrand(brandInfo.Id);

                if (oldBrandItem != null)
                {
                    Update(oldBrandItem, brandInfo);
                    dataService.DataBaseContext.SaveChanges();
                    webService.ConfirmUpdateBrands(lastUpdate, new[] {oldBrandItem.Id});
                }
                else
                {
                    BrandItemEntity brand = Create(brandInfo);
                    dataService.Insert(brand);
                    brand = dataService.DataBaseContext.BrandItemEntities.Find(brandInfo.Id);

                    if (brand != null)
                    {
                        webService.ConfirmUpdateBrands(lastUpdate, new[] { brand.Id });
                    }
                }
            }
        }

        public void DownLoadBrands(Brands brands, DateTimeOffset lastUpdate)
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
                        .Where(x => dataService.DataBaseContext.BrandItemEntities.Find(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateBrands(lastUpdate, ids);
            }
        }

        #endregion

        #region Catalogs

        private CatalogItemEntity GetCatalogItem(long id)
        {
            CatalogItemEntity entity = dataService.DataBaseContext.CatalogItemEntities.Find(id);
            return entity;
        }

        private CatalogItemEntity Create(CatalogInfo catalogInfo, DateTimeOffset lastUpdate)
        {
            BrandItemEntity brandItem = GetBrandWithLoad(catalogInfo.BrandId, lastUpdate);
            DirectoryEntity directory = GetDirectoryWithLoad(catalogInfo.DirectoryId);
            List<PhotoItemEntity> photos = GetPhotosWithLoad(catalogInfo.Photos);
            CatalogItemEntity entity = LoadAssembler.Assemble(catalogInfo, brandItem, photos, directory);
            return entity;
        }

        private void Update(CatalogItemEntity entity, CatalogInfo catalogInfo, DateTimeOffset lastUpdate)
        {
            BrandItemEntity brandItem = GetBrandWithLoad(catalogInfo.BrandId, lastUpdate);
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

        public void DownLoadCatalogItem(CatalogInfo catalogInfo, DateTimeOffset lastUpdate)
        {
            CatalogItemEntity oldEntity = GetCatalogItem(catalogInfo.Id);

            if (oldEntity != null)
            {
                Update(oldEntity, catalogInfo, lastUpdate);
                dataService.DataBaseContext.SaveChanges();
                webService.ConfirmUpdateCatalogs(new[] { oldEntity.Id });
            }
            else
            {
                CatalogItemEntity entity = Create(catalogInfo, lastUpdate);
                dataService.Insert(entity);
                entity = dataService.DataBaseContext.CatalogItemEntities.Find(catalogInfo.Id);

                if (entity != null)
                {
                    webService.ConfirmUpdateCatalogs(new[] { catalogInfo.Id });
                }
            }
        }

        public void DownLoadCatalogs(Catalogs catalogs, DateTimeOffset lastUpdate)
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
                            Update(oldCatalogItem, x, lastUpdate);
                        }
                        else
                        {
                            entities.Add(Create(x, lastUpdate));
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
                        .Where(x => dataService.DataBaseContext.BrandItemEntities.Find(x.Id) != null)
                        .Select(x => x.Id)
                        .ToArray();

                webService.ConfirmUpdateBrands(lastUpdate, ids);
            }
        }

        #endregion

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

        public void DownLoadDirectoryItem(DirectoryInfo directoryInfo)
        {
            DirectoryEntity oldDirectory = GetDirectory(directoryInfo.Id);

            if (oldDirectory != null)
            {
                Update(oldDirectory, directoryInfo);
            }
            else
            {
                DirectoryEntity parent = GetDirectoryWithLoad(directoryInfo.Parent);
                List<DirectoryEntity> subDirectories = new List<DirectoryEntity>();

                directoryInfo.SubDirectoryId.ToList().ForEach(
                    x =>
                    {
                        subDirectories.Add(GetDirectoryWithLoad(x));
                    });

                DirectoryEntity entity = LoadAssembler.Assemble(directoryInfo, parent, subDirectories);
                dataService.Insert(entity);
            }
        }

        private void Update(DirectoryEntity entity, DirectoryInfo directoryInfo)
        {
            DirectoryEntity parent = GetDirectoryWithLoad(directoryInfo.Parent);
            List<DirectoryEntity> subDirectories = new List<DirectoryEntity>();

            directoryInfo.SubDirectoryId.ToList().ForEach(
                x =>
                {
                    subDirectories.Add(GetDirectoryWithLoad(x));
                });

            entity.Code = directoryInfo.Code;
            entity.Name = directoryInfo.Name;
            entity.Parent = parent;
            entity.SubDirectory = subDirectories;
            entity.DateOfCreation = directoryInfo.DateOfCreation;
            entity.LastUpdated = directoryInfo.LastUpdated;
            entity.ForceUpdated = directoryInfo.ForceUpdated;

            dataService.DataBaseContext.SaveChanges();
        }

        public void DownLoadDirectories(Directories directories)
        {
            throw new System.NotImplementedException();
        }

        public void DownLoadPhotoItem(PhotoInfo photoInfo)
        {
            PhotoItemEntity entity = LoadAssembler.Assemble(photoInfo);
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
