using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Annotations;
using Common.Service.Implementation;
using DataBase.Context;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using DataBase.Objects;
using DataBase.Service;
using DataBase.Service.Implementation;
using File.Service;
using File.Service.Implementation;
using Json.Contract;
using Option.Service;
using Directory = Json.Contract.Directory;

namespace Load.Service.Implementation
{
    public class DownLoadService : IDownLoadService
    {
        #region Members

        private readonly Timer timer;
        private readonly LoadingThreadInfo loadingThreadInfo;
        private readonly MovingThreadInfo movingThreadInfo;
        private readonly IFileService fileService;
        private bool isLoading;
        private readonly IOptionService optionService;
        private readonly IDataService dataService;

        #endregion

        #region Constructors

        public DownLoadService(uint timeOut, uint dueTime, uint period, List<MovingQueueItem> movingQueue, IOptionService optionService)
        {
            isLoading = false;
            this.optionService = optionService;
            dataService = new DataService();
            movingThreadInfo = new MovingThreadInfo(timeOut, dueTime, period, movingQueue);
            movingThreadInfo.StartMoving += StartMovingFiles;
            movingThreadInfo.Completed += MovingCompleted;
            movingThreadInfo.TimeOutIsOver += MovingTimeOutIsOver;
            loadingThreadInfo = new LoadingThreadInfo();
            loadingThreadInfo.Start += (sender, args) => Stop();
            loadingThreadInfo.End += (sender, args) => Start();
            timer = new Timer(LoadData, loadingThreadInfo, Timeout.Infinite, Timeout.Infinite);
            fileService = new FileService();
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        public void Start()
        {
            timer.Change(movingThreadInfo.DueTime, movingThreadInfo.Period);
        }

        public void Stop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void LoadData(object info)
        {
            LoadingThreadInfo threadInfo = info as LoadingThreadInfo;

            if (loadingThreadInfo != null)
            {
                LoadData(threadInfo);
            }
        }

        private void LoadData(LoadingThreadInfo threadInfo)
        {
            if (!threadInfo.IsLoading)
            {
                movingThreadInfo.MovingInfo.MovedFiles.Clear();
                movingThreadInfo.MovingInfo.WaitingFiles.Clear();
                movingThreadInfo.MovingInfo.ExceptionFiles.Clear();
                movingThreadInfo.Start = null;
                fileService.AsyncLoadFiles(movingThreadInfo);
            }
        }

        private void StartMovingFiles(object sender, EventArgs args)
        {
            loadingThreadInfo.IsLoading = true;
            //************************************
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  Start moving files...", DateTime.Now);
            Console.WriteLine("-------------------------------------------------------------------------");
            //************************************
        }

        private void MovingCompleted(object sender, EventArgs e)
        {
            LoadData();
            BackupToArchive(sender);
            loadingThreadInfo.IsLoading = false;
            //************************************
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("[{0:yyyy'.'MM'.'dd HH':'mm':'ss fffffff}]  End moving files", DateTime.Now);
            Console.WriteLine("-------------------------------------------------------------------------");
            //************************************
        }

        private void BackupToArchive(object sender)
        {
            var movingThreadInfo = sender as MovingThreadInfo;

            if (movingThreadInfo != null)
            {
                string subArcPath = movingThreadInfo.Start.HasValue
                    ? movingThreadInfo.Start.Value.ToString("yyyyMMddTHHmmss") + "\\"
                    : string.Empty;
                string archivePath = optionService.ArcSourcePath + subArcPath;
                fileService.MoveFiles(optionService.WorkingSourcePath,
                    archivePath,
                    optionService.SourcePatterns);
                fileService.MoveFiles(optionService.WorkingSourcePath + optionService.SubDirForPhoto,
                    archivePath + optionService.SubDirForPhoto,
                    optionService.PhotoPatterns);
            }
        }

        private void MovingTimeOutIsOver(object sender, EventArgs e)
        {
            SaveMovedErrors(sender);
            MovingCompleted(sender, e);
        }

        private void LoadData()
        {
            JsonLoadData jsonLoadData = JsonLoadData();
            LoadToDatabase(jsonLoadData);
        }

        private void LoadToDatabase(JsonLoadData jsonLoadData)
        {
            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
            dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            CreateBrandItems(dataService.DataBaseContext, jsonLoadData);
            CreateDirectoryItems(dataService.DataBaseContext, jsonLoadData);
            CreateProductDirection(dataService.DataBaseContext);
            CreateNomenclatureGroupItems(dataService.DataBaseContext, jsonLoadData);
            CreateCommodityDirectionsItems(dataService.DataBaseContext, jsonLoadData);
            LoadPictures(dataService.DataBaseContext, 
                         optionService.WorkingSourcePath + optionService.SubDirForPhoto,
                         optionService.PhotoPatterns);
            CreateCatalogItems(dataService.DataBaseContext, jsonLoadData);

            dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = true;
            dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = true;
        }

        private void CreateCatalogItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData)
        {
            if (dataBaseContext != null && jsonLoadData?.Nomenclatures != null && jsonLoadData.Nomenclatures.Any())
            {
                int countOfCatalogItem = 0;

                jsonLoadData.Nomenclatures.ForEach(
                    x =>
                    {
                        dataBaseContext.CatalogItemEntities.Add(Assemble(dataBaseContext, x));
                        countOfCatalogItem++;

                        if (countOfCatalogItem % 100 == 0)
                        {
                            try
                            {
                                dataBaseContext.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                ;
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception e)
                {
                    ;
                }
            }
        }

        [NotNull]
        private CatalogItemEntity Assemble(DataBaseContext dataBaseContext,
                                           Nomenclature nomenclature)
        {
            BrandItemEntity brandItem = GetBrandItem(dataBaseContext, nomenclature);
            DirectoryEntity directoryItem = GetDirectoryItem(dataBaseContext, nomenclature);
            NomenclatureGroupEntity nomenclatureGroupItem =
                GetNomenclatureGroupItem(dataBaseContext, nomenclature);
            List<CommodityDirectionEntity> commodityDirectionItemsForCatalogItem =
                GetCommodityDirection(dataBaseContext, nomenclature);
            PriceInfo priceInfo = GetPriceInfo(nomenclature);
            List<PhotoItemEntity> photoItems = GetPhotoItems(dataBaseContext, nomenclature);
            List<string> needToCreatePhotos = GetNeedToCreatePhotos(photoItems, nomenclature);
            photoItems.AddRange(CreateEmptyPhotos(dataBaseContext, needToCreatePhotos));
            var now = DateTimeOffset.Now;

            var catalogItem = new CatalogItemEntity
            {

                UID = nomenclature.UID.ConvertToGuid(),
                Code = nomenclature.Code,
                Article = nomenclature.VendorCode,
                Name = nomenclature.Name,
                Brand = brandItem,
                BrandName = brandItem?.Name,
                Unit = nomenclature.Measure,
                EnterpriceNormPack = nomenclature.NormPackaging,
                BatchOfSales = nomenclature.BatchOfSales.ConvertToDecimal(),
                Balance = nomenclature.InStock,
                Price = priceInfo.Prise,
                Currency = priceInfo.Currency,
                Multiplicity = 0,
                HasPhotos = photoItems.Any(x => x.IsLoad),
                Photos = photoItems,
                DateOfCreation = nomenclature.DateOfCreation.ConvertToDateTimeOffset(),
                LastUpdated = now,
                ForceUpdated = now,
                Status = GenerateStatus(),
                LastUpdatedStatus = DateTimeOffset.Now,
                BasketItems = null,
                Directory = directoryItem,
                NomenclatureGroup = nomenclatureGroupItem,
                CommodityDirection = commodityDirectionItemsForCatalogItem,
            };

            if (brandItem != null)
            {
                if (brandItem.CatalogItems != null)
                {
                    brandItem.CatalogItems.Add(catalogItem);
                }
                else
                {
                    brandItem.CatalogItems = new List<CatalogItemEntity> { catalogItem };
                }
            }

            if (directoryItem != null)
            {
                if (directoryItem.CatalogItems != null)
                {
                    directoryItem.CatalogItems.Add(catalogItem);
                }
                else
                {
                    directoryItem.CatalogItems = new List<CatalogItemEntity> { catalogItem };
                }
            }

            if (nomenclatureGroupItem != null)
            {
                if (nomenclatureGroupItem.CatalogItems != null)
                {
                    nomenclatureGroupItem.CatalogItems.Add(catalogItem);
                }
                else
                {
                    nomenclatureGroupItem.CatalogItems = new List<CatalogItemEntity> { catalogItem };
                }
            }

            if (commodityDirectionItemsForCatalogItem.Any())
            {
                commodityDirectionItemsForCatalogItem.ForEach(
                    x =>
                    {
                        if (x != null)
                        {
                            if (x.CatalogItems != null)
                            {
                                x.CatalogItems.Add(catalogItem);
                            }
                            else
                            {
                                x.CatalogItems = new List<CatalogItemEntity> { catalogItem };
                            }
                        }
                    });
            }

            if (photoItems.Any())
            {
                photoItems.ForEach(
                    x =>
                    {
                        if (x != null)
                        {
                            x.CatalogItem = catalogItem;
                        }
                    });
            }

            return catalogItem;
        }

        private void LoadPictures(DataBaseContext dataBaseContext, string photoPath, string[] photoSearchPattern = null)
        {
            FileInfo[] fileInfos = fileService.GetFileInfos(photoPath, photoSearchPattern);

            if (dataBaseContext != null && fileInfos.Any())
            {
                int countOfFiles = 0;

                fileInfos.ToList().ForEach(
                    x =>
                    {
                        if (!string.IsNullOrWhiteSpace(x.FullName))
                        {
                            byte[] picture = fileService.ReadByteArrayPicture(x.FullName);

                            var now = DateTimeOffset.Now;

                            var photoItem = new PhotoItemEntity
                            {
                                Name = fileService.GetFileName(x.Name),
                                IsLoad = true,
                                Photo = picture,
                                DateOfCreation = now,
                                LastUpdated = now,
                                ForceUpdated = now
                            };

                            dataBaseContext.PhotoItemEntities.Add(photoItem);
                            countOfFiles++;

                            if (countOfFiles % 10 == 0)
                            {
                                try
                                {
                                    dataBaseContext.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    ;
                                }
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private CatalogItemStatus GenerateStatus()
        {
            int miliseconds = DateTime.Now.Millisecond;

            CatalogItemStatus status = CatalogItemStatus.Old;

            if (miliseconds % 7 == 0)
            {
                status = CatalogItemStatus.PriceIsDown;
            }
            else if (miliseconds % 5 == 0)
            {
                status = CatalogItemStatus.PriceIsUp;
            }
            else if (miliseconds % 2 == 0)
            {
                status = CatalogItemStatus.New;
            }

            return status;
        }

        private List<PhotoItemEntity> GetPhotoItems(DataBaseContext dataBaseContext,
                                                    Nomenclature nomenclature)
        {
            List<PhotoItemEntity> photoItems = new List<PhotoItemEntity>();

            if (dataBaseContext != null && nomenclature != null && nomenclature.Photos.Any())
            {
                photoItems = dataBaseContext
                    .PhotoItemEntities
                    .Where(x => nomenclature.Photos.Any(p => p == x.Name)).ToList();
            }

            return photoItems;
        }

        private List<string> GetNeedToCreatePhotos(List<PhotoItemEntity> photoItems, Nomenclature nomenclature)
        {
            List<string> needToCreatePhotos = new List<string>();

            if (nomenclature?.Photos != null) // photoItems != null && photoItems.Any()
            {
                needToCreatePhotos = nomenclature.Photos.Where(x => photoItems == null ||
                                                                    !photoItems.Any() ||
                                                                    photoItems.All(p => p.Name != x)).ToList();
            }
            return needToCreatePhotos;
        }
        private IEnumerable<PhotoItemEntity> CreateEmptyPhotos(DataBaseContext dataBaseContext,
                                                               List<string> photos)
        {
            var emptyPhotos = new List<PhotoItemEntity>();

            photos.ForEach(x => emptyPhotos.Add(new PhotoItemEntity { Name = x, IsLoad = false, Photo = null }));
            dataBaseContext.PhotoItemEntities.AddRange(emptyPhotos);

            return emptyPhotos;
        }

        private PriceInfo GetPriceInfo(Nomenclature nomenclature)
        {
            var priceInfo = new PriceInfo();

            if (nomenclature?.TypesOfPrices != null && nomenclature.TypesOfPrices.Any())
            {
                PriceTypeItem typeOfPrice = nomenclature.TypesOfPrices.FirstOrDefault();

                if (typeOfPrice != null)
                {
                    PriceInfoParse(typeOfPrice.Price, priceInfo);
                }
            }

            return priceInfo;
        }

        private void PriceInfoParse(string priceWithCurrency, PriceInfo priceInfo)
        {
            if (!string.IsNullOrWhiteSpace(priceWithCurrency))
            {
                string[] splitPriceString = priceWithCurrency.Split(' ');

                if (splitPriceString.Any())
                {
                    string stringPrice = splitPriceString[0];

                    if (!string.IsNullOrWhiteSpace(stringPrice))
                    {
                        decimal price;

                        if (decimal.TryParse(stringPrice, out price))
                        {
                            priceInfo.Prise = price;

                            if (splitPriceString.Length >= 2)
                            {
                                priceInfo.Currency = splitPriceString[1];
                            }
                        }
                        else
                        {
                            if (splitPriceString.Length >= 2)
                            {
                                priceInfo.Currency = splitPriceString[1];
                            }
                            else
                            {
                                priceInfo.Currency = splitPriceString[0];
                            }
                        }
                    }
                    else if (splitPriceString.Length >= 2)
                    {
                        priceInfo.Currency = splitPriceString[1];
                    }
                }
            }
        }

        private BrandItemEntity GetBrandItem(DataBaseContext dataBaseContext,
                                             Nomenclature nomenclature)
        {
            BrandItemEntity brandItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                Guid brandGuid;

                if (nomenclature.BrandUID.ConvertToGuid(out brandGuid))
                {
                    brandItem = dataBaseContext
                        .BrandItemEntities
                        .FirstOrDefault(x => x.Code == brandGuid);
                }
            }

            return brandItem;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext,
                                                 Nomenclature nomenclature)
        {
            DirectoryEntity directoryItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                Guid directoryGuid;

                if (nomenclature.CatalogUID.ConvertToGuid(out directoryGuid))
                {
                    directoryItem = dataBaseContext
                        .DirectoryEntities
                        .FirstOrDefault(x => x.Code == directoryGuid);
                }
            }

            return directoryItem;
        }

        private NomenclatureGroupEntity GetNomenclatureGroupItem(DataBaseContext dataBaseContext,
                                                                 Nomenclature nomenclature)
        {
            NomenclatureGroupEntity nomenclatureGroupItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                Guid nomenclatureGroupGuid;

                if (nomenclature.NomenclatureGroupUID.ConvertToGuid(out nomenclatureGroupGuid))
                {
                    nomenclatureGroupItem = dataBaseContext
                        .NomenclatureGroupEntities
                        .FirstOrDefault(x => x.Code == nomenclatureGroupGuid);
                }
            }

            return nomenclatureGroupItem;
        }

        private List<CommodityDirectionEntity> GetCommodityDirection(DataBaseContext dataBaseContext,
                                                                     Nomenclature nomenclature)
        {
            List<CommodityDirectionEntity> commodityDirectionItems = new List<CommodityDirectionEntity>();

            if (dataBaseContext != null &&
                nomenclature?.CommodityDirection != null &&
                nomenclature.CommodityDirection.Any())
            {
                List<Guid> commodityDirectionGuids = nomenclature
                    .CommodityDirection
                    .Where(IsGuid)
                    .Select(x => x.ConvertToGuid())
                    .ToList();

                if (commodityDirectionGuids.Any())
                {
                    commodityDirectionItems = dataBaseContext
                        .CommodityDirectionEntities.Where(x => commodityDirectionGuids.Any(c => c == x.Code))
                        .ToList();
                }
            }

            return commodityDirectionItems;
        }

        private bool IsGuid(string code)
        {
            Guid guid;
            return code.ConvertToGuid(out guid);
        }

        private void CreateCommodityDirectionsItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData)
        {
            if (dataBaseContext != null && jsonLoadData?.CommodityDirections != null && jsonLoadData.CommodityDirections.Any())
            {
                int countInsertedItems = 0;

                jsonLoadData.CommodityDirections.ForEach(
                    x =>
                    {
                        CommodityDirectionEntity commodityDirection = Assemble(x);

                        if (commodityDirection != null)
                        {
                            dataBaseContext.CommodityDirectionEntities.Add(commodityDirection);

                            if (countInsertedItems % 100 == 0)
                            {
                                try
                                {
                                    dataBaseContext.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    ;
                                }
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private CommodityDirectionEntity Assemble(Json.Contract.CommodityDirection commodityDirection)
        {
            CommodityDirectionEntity commodityDirectionItem = null;

            if (commodityDirection != null)
            {
                var now = DateTimeOffset.Now;

                commodityDirectionItem = new CommodityDirectionEntity
                {

                    Code = commodityDirection.UID.ConvertToGuid(),
                    Name = commodityDirection.Name,
                    DateOfCreation = now,
                    LastUpdated = now,
                    ForceUpdated = now
                };
            }

            return commodityDirectionItem;
        }

        private void CreateNomenclatureGroupItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData)
        {
            if (dataBaseContext != null && jsonLoadData?.NomenclatureGroups != null && jsonLoadData.NomenclatureGroups.Any())
            {
                int countInsertedItems = 0;

                jsonLoadData.NomenclatureGroups.ForEach(
                    x =>
                    {
                        NomenclatureGroupEntity nomenclatureGroup = Assemble(x);

                        if (nomenclatureGroup != null)
                        {
                            dataBaseContext.NomenclatureGroupEntities.Add(nomenclatureGroup);

                            if (countInsertedItems % 100 == 0)
                            {
                                try
                                {
                                    dataBaseContext.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    ;
                                }
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private NomenclatureGroupEntity Assemble(NomenclatureGroup nomenclatureGroup)
        {
            NomenclatureGroupEntity nomenclatureGroupItem = null;

            if (nomenclatureGroup != null)
            {
                var now = DateTimeOffset.Now;

                nomenclatureGroupItem = new NomenclatureGroupEntity
                {
                    Code = nomenclatureGroup.UID.ConvertToGuid(),
                    Name = nomenclatureGroup.Name,
                    DateOfCreation = now,
                    LastUpdated = now,
                    ForceUpdated = now
                };
            }

            return nomenclatureGroupItem;
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext)
        {
            try
            {
                var now = DateTimeOffset.Now;
                ProductDirectionEntity item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Vaz;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                Guid code = Guid.Parse("1FF0EBCD-1507-40E9-A409-2AF3B8F77D49");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Gaz;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("9B6B4325-B435-4D65-925E-4921F09D461F");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Zaz;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("264D3368-ECA8-4F2F-9C3B-7B7CC582C7FD");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Chemistry;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("78DC5231-C9D8-49A1-9FC5-AD18BF71DC13");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Battery;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("C7667731-5CFE-4BB6-90DC-8520C87F4FA0");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Gas;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("A4151BE4-3D3F-4A18-A3D1-4BF48F03AB6C");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Instrument;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("AFEDE7C9-85E0-4C60-A5FB-0D81B0771D3D");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = DataBase.Context.Object.CommodityDirection.Common;
                item.DateOfCreation = now;
                item.ForceUpdated = now;
                item.LastUpdated = now;
                code = Guid.Parse("80135062-F8F6-4C5A-AFD2-48A0117EB2B6");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
            catch (Exception e)
            {
                ;//throw;
            }
        }

        private void CreateDirectoryItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData)
        {
            if (dataBaseContext != null && jsonLoadData?.Catalogs != null && jsonLoadData.Catalogs.Any())
            {
                int countInsertedItems = 0;

                jsonLoadData.Catalogs.ForEach(
                    x =>
                    {
                        DirectoryEntity directory = Assemble(x);

                        if (directory != null)
                        {
                            dataBaseContext.DirectoryEntities.Add(directory);

                            if (countInsertedItems % 10 == 0)
                            {
                                try
                                {
                                    dataBaseContext.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    ;
                                }
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private DirectoryEntity Assemble(Directory directory)
        {
            DirectoryEntity directoryItem = null;

            if (directory != null)
            {
                List<DirectoryEntity> subDirectories = null;

                if (directory.Subdirectory != null && directory.Subdirectory.Any())
                {
                    subDirectories = new List<DirectoryEntity>();
                    directory.Subdirectory.ToList().ForEach(
                        x =>
                        {
                            if (x != null)
                            {
                                subDirectories.Add(Assemble(x));
                            }
                        });
                }

                var now = DateTimeOffset.Now;
                directoryItem = new DirectoryEntity
                {
                    Code = directory.UID.ConvertToGuid(),
                    Name = directory.Name,
                    SubDirectory = subDirectories,
                    DateOfCreation = now,
                    LastUpdated = now,
                    ForceUpdated = now
                };
            }

            return directoryItem;
        }

        private void CreateBrandItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData)
        {
            if (dataBaseContext != null && jsonLoadData?.Brands != null && jsonLoadData.Brands.Any())
            {
                int countInsertedItems = 0;

                jsonLoadData.Brands.ForEach(
                    x =>
                    {
                        BrandItemEntity brandItem = Assemble(x);

                        if (brandItem != null)
                        {
                            dataBaseContext.BrandItemEntities.Add(brandItem);
                            countInsertedItems++;

                            if (countInsertedItems % 100 == 0)
                            {
                                try
                                {
                                    dataBaseContext.SaveChanges();
                                }
                                catch (Exception)
                                {
                                    ;
                                }
                            }
                        }
                    });

                try
                {
                    dataBaseContext.SaveChanges();
                }
                catch (Exception)
                {
                    ;
                }
            }
        }

        private BrandItemEntity Assemble(Brand brand)
        {
            BrandItemEntity brandItem = null;

            if (brand != null)
            {
                var now = DateTimeOffset.Now;
                brandItem = new BrandItemEntity
                {
                    Code = brand.UID.ConvertToGuid(),
                    Name = brand.Name,
                    DateOfCreation = now,
                    ForceUpdated = now,
                    LastUpdated = now
                };
            }

            return brandItem;
        }

        private JsonLoadData JsonLoadData()
        {
            var data = new JsonLoadData();
            string[] patterns = optionService.SourcePatterns;
            string[] metaFiles = fileService.GetFileNames(optionService.WorkingSourcePath,
                patterns.First(x => x.StartsWith("MetaData")));

            foreach (string fileName in metaFiles)
            {
                data.AddMetaData(fileService.ReadMetaData(optionService.WorkingSourcePath + fileName));
            }

            string[] priceListFiles = fileService.GetFileNames(optionService.WorkingSourcePath,
                patterns.First(x => x.StartsWith("PriceList")));

            foreach (string fileName in priceListFiles)
            {
                data.AddPriceList(fileService.ReadPriceList(optionService.WorkingSourcePath + fileName));
            }

            string[] clientFiles = fileService.GetFileNames(optionService.WorkingSourcePath,
                patterns.First(x => x.StartsWith("Clients")));

            foreach (string fileName in clientFiles)
            {
                data.AddClient(fileService.ReadClients(optionService.WorkingSourcePath + fileName));
            }

            return data;
        }

        private void SaveMovedErrors(object sender)
        {
            MovingThreadInfo movingInfo = sender as MovingThreadInfo;

            if (movingInfo != null)
            {
                SaveMovedErrors(movingInfo);
            }
        }

        private void SaveMovedErrors(MovingThreadInfo movingInfo)
        {
            ; // TODO Save Moved Errors to DataBase & LOG;
        }

        #endregion

        #region Events
        #endregion

    }
}
