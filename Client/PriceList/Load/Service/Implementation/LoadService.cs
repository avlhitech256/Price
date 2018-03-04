using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
        private readonly ICatalogRepository catalogRepository;
        private readonly IPhotoRepository photoRepository;

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
            catalogRepository = new CatalogRepository(dataService);
            photoRepository = new PhotoRepository(dataService);
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

        public void DownLoadBrands(Brands brands)
        {
            if (brands != null && brands.Items != null && brands.Items.Any())
            {
                try
                {
                    DataTable brandsTable = CreateBrandsTable(brands.Items);

                    var brandsParametr = new SqlParameter();
                    brandsParametr.ParameterName = "@brands";
                    brandsParametr.SqlDbType = SqlDbType.Structured;
                    brandsParametr.TypeName = "brandsTable";
                    brandsParametr.Value = brandsTable;
                    brandsParametr.Direction = ParameterDirection.Input;

                    dataService.DataBaseContext.Database
                        .ExecuteSqlCommand("UpdateBrands @brands", brandsParametr);
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        private DataTable CreateBrandsTable(BrandInfo[] items)
        {
            DataTable brandsTable = new DataTable();

            brandsTable.Columns.Add("Id", typeof(long));
            brandsTable.Columns.Add("Code", typeof(Guid));
            brandsTable.Columns.Add("Name", typeof(string));
            brandsTable.Columns.Add("DateOfCreation", typeof(DateTimeOffset));
            brandsTable.Columns.Add("LastUpdated", typeof(DateTimeOffset));
            brandsTable.Columns.Add("ForceUpdated", typeof(DateTimeOffset));

            items?.ToList().ForEach(x => AddDirectoryDataRow(brandsTable, x));

            return brandsTable;
        }

        private void AddDirectoryDataRow(DataTable directoriesTable, BrandInfo item)
        {
            DataRow row = directoriesTable.NewRow();

            row.SetField("Id", item.Id);
            row.SetField("Code", item.Code);
            row.SetField("Name", item.Name);
            row.SetField("DateOfCreation", item.DateOfCreation);
            row.SetField("LastUpdated", item.LastUpdated);
            row.SetField("ForceUpdated", item.ForceUpdated);

            directoriesTable.Rows.Add(row);
        }

        #endregion

        #region Catalogs

        private CatalogItemEntity GetCatalogItem(long id)
        {
            CatalogItemEntity entity = catalogRepository.GetItem(id);
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
            if (catalogs?.Items != null && catalogs.Items.Any())
            {
                long[] ids = new long[0];

                try
                {
                    DataTable catalogsTable = CreateCatalogsTable(catalogs.Items);

                    var catalogsParametr = new SqlParameter();
                    catalogsParametr.ParameterName = "@catalogs";
                    catalogsParametr.SqlDbType = SqlDbType.Structured;
                    catalogsParametr.TypeName = "catalogsTable";
                    catalogsParametr.Value = catalogsTable;
                    catalogsParametr.Direction = ParameterDirection.Input;

                    DataTable linkToPhotoTable = CreateLinkToPhoto(catalogs.Items);

                    var linkToPhotosParametr = new SqlParameter();
                    linkToPhotosParametr.ParameterName = "@linkToPhotos";
                    linkToPhotosParametr.SqlDbType = SqlDbType.Structured;
                    linkToPhotosParametr.TypeName = "linkToPhotoTable";
                    linkToPhotosParametr.Value = linkToPhotoTable;
                    linkToPhotosParametr.Direction = ParameterDirection.Input;

                    dataService.DataBaseContext.Database
                        .ExecuteSqlCommand("UpdateCatalogs @catalogs, @linkToPhotos",
                                           catalogsParametr, linkToPhotosParametr);
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        private DataTable CreateLinkToPhoto(CatalogInfo[] items)
        {
            DataTable linkToPhotoTable = new DataTable();

            linkToPhotoTable.Columns.Add("Catalog_Id", typeof (long));
            linkToPhotoTable.Columns.Add("Photo_Id", typeof (long));

            items.ToList().ForEach(x => AddLinkToPhotoDataRows(linkToPhotoTable, x));

            return linkToPhotoTable;
        }

        private void AddLinkToPhotoDataRows(DataTable linkToPhotosTable, CatalogInfo item)
        {
            if (linkToPhotosTable != null)
            {
                item?.Photos.ToList().ForEach(x => AddLinkToPhotoDataRow(linkToPhotosTable, item.Id, x));
            }
        }

        private void AddLinkToPhotoDataRow(DataTable linkToPhotosTable, long catalogId, long photoId)
        {
            DataRow row = linkToPhotosTable.NewRow();

            row["Catalog_Id"] = catalogId;
            row["Photo_Id"] = photoId;

            linkToPhotosTable.Rows.Add(row);
        }

        private DataTable CreateCatalogsTable(CatalogInfo[] items)
        {
            DataTable catalogsTable = new DataTable();

            catalogsTable.Columns.Add("Id", typeof(long));
            catalogsTable.Columns.Add("UID", typeof(Guid));
            catalogsTable.Columns.Add("Code", typeof(string));
            catalogsTable.Columns.Add("Article", typeof(string));
            catalogsTable.Columns.Add("Name", typeof(string));
            catalogsTable.Columns.Add("BrandName", typeof(string));
            catalogsTable.Columns.Add("Unit", typeof(string));
            catalogsTable.Columns.Add("EnterpriceNormPack", typeof(string));
            catalogsTable.Columns.Add("BatchOfSales", typeof(decimal));
            catalogsTable.Columns.Add("Balance", typeof(string));
            catalogsTable.Columns.Add("Price", typeof(decimal));
            catalogsTable.Columns.Add("Currency", typeof(string));
            catalogsTable.Columns.Add("Multiplicity", typeof(decimal));
            catalogsTable.Columns.Add("HasPhotos", typeof(byte));
            catalogsTable.Columns.Add("Status", typeof(int));
            catalogsTable.Columns.Add("LastUpdatedStatus", typeof(DateTimeOffset));
            catalogsTable.Columns.Add("DateOfCreation", typeof(DateTimeOffset));
            catalogsTable.Columns.Add("LastUpdated", typeof(DateTimeOffset));
            catalogsTable.Columns.Add("ForceUpdated", typeof(DateTimeOffset));
            DataColumn brandIdColumn = catalogsTable.Columns.Add("Brand_Id", typeof(long));
            brandIdColumn.AllowDBNull = true;
            DataColumn directoryIdColumn = catalogsTable.Columns.Add("Directory_Id", typeof(long));
            directoryIdColumn.AllowDBNull = true;

            items?.ToList().ForEach(x => AddCatalogDataRow(catalogsTable, x));

            return catalogsTable;
        }

        private void AddCatalogDataRow(DataTable catalogsTable, CatalogInfo item)
        {
            DataRow row = catalogsTable.NewRow();

            row.SetField("Id", item.Id);
            row.SetField("UID", item.UID);
            row.SetField("Code", item.Code);
            row.SetField("Article", item.Article);
            row.SetField("Name", item.Name);
            row.SetField("BrandName", item.BrandName);
            row.SetField("Unit", item.Unit);
            row.SetField("EnterpriceNormPack", item.EnterpriceNormPack);
            row.SetField("BatchOfSales", item.BatchOfSales);
            row.SetField("Balance", item.Balance);
            row.SetField("Price", item.Price);
            row.SetField("Currency", item.Currency);
            row.SetField("Multiplicity", item.Multiplicity);
            row.SetField("HasPhotos", item.HasPhotos);
            row.SetField("Status", item.Status);
            row.SetField("LastUpdatedStatus", item.LastUpdatedStatus);
            row.SetField("DateOfCreation", item.DateOfCreation);
            row.SetField("LastUpdated", item.LastUpdated);
            row.SetField("ForceUpdated", item.ForceUpdated);
            row.SetField("Brand_Id", item.BrandId);
            row.SetField("Directory_Id", item.DirectoryId);

            catalogsTable.Rows.Add(row);
        }


    //public void DownLoadCatalogs(Catalogs catalogs)
    //{
    //    if (catalogs?.Items != null && catalogs.Items.Any())
    //    {
    //        catalogRepository.Load(catalogs.Items.Select(x => x.Id));
    //        photoRepository.Load(catalogs.Items.SelectMany(x => x.Photos));

    //        dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
    //        dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

    //        var entities = new List<CatalogItemEntity>();

    //        catalogs.Items.ToList().ForEach(
    //            x =>
    //            {
    //                try
    //                {
    //                    CatalogItemEntity oldCatalogItem = GetCatalogItem(x.Id);

    //                    if (oldCatalogItem != null)
    //                    {
    //                        Update(oldCatalogItem, x);
    //                    }
    //                    else
    //                    {
    //                        entities.Add(Create(x));
    //                    }
    //                }
    //                catch (Exception e)
    //                {
    //                    ;
    //                }

    //            });

    //        if (entities.Any())
    //        {
    //            dataService.InsertMany(entities);
    //        }
    //        else
    //        {
    //            dataService.DataBaseContext.SaveChanges();
    //        }

    //        long[] ids =
    //            catalogs.Items
    //                .Where(x => GetCatalogItem(x.Id) != null)
    //                .Select(x => x.Id)
    //                .ToArray();

    //        webService.ConfirmUpdateCatalogs(ids);

    //        dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
    //        dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
    //    }
    //}

    #endregion

    #region Directory

    private DirectoryEntity GetDirectory(long id, List<DirectoryEntity> cacheDirectoryEntity)
        {
            DirectoryEntity directory = directoryRepository.GetItem(id);

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

        public void DownLoadDirectories(Directories directories)
        {
            if (directories != null && directories.Items != null && directories.Items.Any())
            {
                try
                {
                    DataTable directoriesTable = CreateDirectoriesTable(directories.Items);

                    var directoriesParametr = new SqlParameter();
                    directoriesParametr.ParameterName = "@directories";
                    directoriesParametr.SqlDbType = SqlDbType.Structured;
                    directoriesParametr.TypeName = "directoriesTable";
                    directoriesParametr.Value = directoriesTable;
                    directoriesParametr.Direction = ParameterDirection.Input;

                    dataService.DataBaseContext.Database
                        .ExecuteSqlCommand("UpdateDirectories @directories", directoriesParametr);
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        private DataTable CreateDirectoriesTable(DirectoryInfo[] items)
        {
            DataTable directoriesTable = new DataTable();

            directoriesTable.Columns.Add("Id", typeof(long));
            directoriesTable.Columns.Add("Code", typeof(Guid));
            directoriesTable.Columns.Add("Name", typeof(string));
            DataColumn parentIdColumn = directoriesTable.Columns.Add("Parent_Id", typeof(long));
            parentIdColumn.AllowDBNull = true;
            directoriesTable.Columns.Add("DateOfCreation", typeof(DateTimeOffset));
            directoriesTable.Columns.Add("LastUpdated", typeof(DateTimeOffset));
            directoriesTable.Columns.Add("ForceUpdated", typeof(DateTimeOffset));

            items?.ToList().ForEach(x => AddDirectoryDataRow(directoriesTable, x));

            return directoriesTable;
        }

        private void AddDirectoryDataRow(DataTable directoriesTable, DirectoryInfo item)
        {
            DataRow row = directoriesTable.NewRow();

            row.SetField("Id", item.Id);
            row.SetField("Code", item.Code);
            row.SetField("Name", item.Name);
            row.SetField("Parent_Id", item.Parent);
            row.SetField("DateOfCreation", item.DateOfCreation);
            row.SetField("LastUpdated", item.LastUpdated);
            row.SetField("ForceUpdated", item.ForceUpdated);

            directoriesTable.Rows.Add(row);
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

        public int DownLoadPhotos(Photos photos)
        {
            int count = 0;

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

                count = ids.Length;

                dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }

            return count;
        }

        #endregion

        #region ProductDirection

        public void DownLoadProductDirections(ProductDirections productDirections)
        {
            if (productDirections != null && productDirections.Items != null && productDirections.Items.Any())
            {
                try
                {
                    DataTable directoriesTable = CreateProductDirectionsTable(productDirections.Items);

                    var productDirectionsParametr = new SqlParameter();
                    productDirectionsParametr.ParameterName = "@productDirections";
                    productDirectionsParametr.SqlDbType = SqlDbType.Structured;
                    productDirectionsParametr.TypeName = "productDirectionsTable";
                    productDirectionsParametr.Value = directoriesTable;
                    productDirectionsParametr.Direction = ParameterDirection.Input;

                    dataService.DataBaseContext.Database
                        .ExecuteSqlCommand("UpdateProductDirections @productDirections", productDirectionsParametr);
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        private DataTable CreateProductDirectionsTable(ProductDirectionInfo[] items)
        {
            DataTable productDirectionsTable = new DataTable();

            productDirectionsTable.Columns.Add("Id", typeof(long));
            productDirectionsTable.Columns.Add("Direction", typeof(int));
            DataColumn directoryIdColumn = productDirectionsTable.Columns.Add("Directory_Id", typeof(long));
            directoryIdColumn.AllowDBNull = true;
            productDirectionsTable.Columns.Add("DateOfCreation", typeof(DateTimeOffset));
            productDirectionsTable.Columns.Add("LastUpdated", typeof(DateTimeOffset));
            productDirectionsTable.Columns.Add("ForceUpdated", typeof(DateTimeOffset));

            items?.ToList().ForEach(x => AddProductDirectionDataRow(productDirectionsTable, x));

            return productDirectionsTable;
        }

        private void AddProductDirectionDataRow(DataTable directoriesTable, ProductDirectionInfo item)
        {
            DataRow row = directoriesTable.NewRow();

            row.SetField("Id", item.Id);
            row.SetField("Direction", item.Direction);
            row.SetField("Directory_Id", item.DirectoryId);
            row.SetField("DateOfCreation", item.DateOfCreation);
            row.SetField("LastUpdated", item.LastUpdated);
            row.SetField("ForceUpdated", item.ForceUpdated);

            directoriesTable.Rows.Add(row);
        }

        #endregion

        #endregion
    }
}
