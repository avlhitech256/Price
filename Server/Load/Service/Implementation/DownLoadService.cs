using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Annotations;
using Common.Data.Enum;
using Common.Service.Implementation;
using DataBase.Context;
using DataBase.Context.Entities;
using DataBase.Objects;
using DataBase.Service;
using DataBase.Service.Implementation;
using File.Service;
using File.Service.Implementation;
using Json.Contract;
using Option.Service;
using CommodityDirection = Common.Data.Enum.CommodityDirection;
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
            LoadDataToDatabase(sender);
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

        private void LoadDataToDatabase(object sender)
        {
            JsonLoadData jsonLoadData = JsonLoadData();
            var threadInfo = sender as MovingThreadInfo;
            DateTimeOffset startMoving = threadInfo != null && threadInfo.Start.HasValue
                ? threadInfo.Start.Value
                : DateTimeOffset.Now;
            LoadToDatabase(jsonLoadData, startMoving);
        }

        private void LoadToDatabase(JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
            dataService.DataBaseContext.Configuration.AutoDetectChangesEnabled = false;
            dataService.DataBaseContext.Configuration.ValidateOnSaveEnabled = false;

            CreateBrandItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
            CreateDirectoryItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
            CreateProductDirection(dataService.DataBaseContext, loadUpdateTime);
            CreateNomenclatureGroupItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
            CreateCommodityDirectionsItems(dataService.DataBaseContext, jsonLoadData, loadUpdateTime);
            LoadPictures(dataService.DataBaseContext, 
                         optionService.WorkingSourcePath + optionService.SubDirForPhoto,
                         optionService.PhotoPatterns, loadUpdateTime);
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

        private void LoadPictures(DataBaseContext dataBaseContext, string photoPath, 
                                  string[] photoSearchPattern, DateTimeOffset loadUpdateTime)
        {
            FileInfo[] fileInfos = fileService.GetFileInfos(photoPath, photoSearchPattern);

            if (dataBaseContext != null && fileInfos.Any())
            {
                int countOfFiles = 0;

                fileInfos.Where(x => !string.IsNullOrWhiteSpace(x.FullName)).ToList().ForEach(
                    x =>
                    {
                        string fileName = fileService.GetFileName(x.Name);
                        byte[] picture = fileService.ReadByteArrayPicture(x.FullName);
                        PhotoItemEntity oldPhotoItem = GetPhotoItem(dataBaseContext, fileName);

                        if (oldPhotoItem != null)
                        {
                            Update(oldPhotoItem, fileName, picture, loadUpdateTime);
                            countOfFiles++;
                        }
                        else
                        {
                            PhotoItemEntity photoItem = Assemble(fileName, picture, loadUpdateTime);
                            dataBaseContext.PhotoItemEntities.Add(photoItem);
                            countOfFiles++;
                        }

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

        private PhotoItemEntity Assemble(string fileName, byte[] picture, DateTimeOffset loadUpdateTime)
        {
            PhotoItemEntity item = new PhotoItemEntity
            {
                Name = fileName,
                IsLoad = picture != null && picture.Length > 0,
                Photo = picture,
                DateOfCreation = loadUpdateTime,
                LastUpdated = loadUpdateTime,
                ForceUpdated = loadUpdateTime
            };

            return item;
        }

        private void Update(PhotoItemEntity entity, string fileName, byte[] picture, DateTimeOffset loadUpdateTime)
        {
            if (entity != null)
            {
                if (Equals(entity, fileName, picture))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Photo = picture;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(PhotoItemEntity entity, string fileName, byte[] picture)
        {
            return entity != null && !string.IsNullOrWhiteSpace(fileName) && entity.Name == fileName &&
                   ((entity.IsLoad && picture != null && entity.Photo != null && picture.Length == entity.Photo.Length) ||
                    (!entity.IsLoad && (entity.Photo == null || entity.Photo != null && entity.Photo.Length == 0)));
        }

        private PhotoItemEntity GetPhotoItem(DataBaseContext dataBaseContext, string fileName)
        {
            PhotoItemEntity photoItem = dataBaseContext.PhotoItemEntities.FirstOrDefault(x => x.Name == fileName);
            return photoItem;
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
                brandItem = GetBrandItem(dataBaseContext, nomenclature.BrandUID);
            }

            return brandItem;
        }

        private BrandItemEntity GetBrandItem(DataBaseContext dataBaseContext, string code)
        {
            BrandItemEntity brandItem = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                brandItem = GetBrandItem(dataBaseContext, guidCode);
            }
            
            return brandItem;
        }

        private BrandItemEntity GetBrandItem(DataBaseContext dataBaseContext, Guid code)
        {
            BrandItemEntity brandItem = dataBaseContext.BrandItemEntities.FirstOrDefault(x => x.Code == code);
            return brandItem;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext, string code)
        {
            DirectoryEntity directory = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                directory = GetDirectoryItem(dataBaseContext, guidCode);
            }

            return directory;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext, Guid code)
        {
            DirectoryEntity directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
            return directory;
        }

        private DirectoryEntity GetDirectoryItem(DataBaseContext dataBaseContext,
                                                 Nomenclature nomenclature)
        {
            DirectoryEntity directoryItem = null;

            if (dataBaseContext != null && nomenclature != null)
            {
                directoryItem = GetDirectoryItem(dataBaseContext, nomenclature.CatalogUID);
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

        private void CreateCommodityDirectionsItems(DataBaseContext dataBaseContext, 
                                                    JsonLoadData jsonLoadData, 
                                                    DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.CommodityDirections != null && jsonLoadData.CommodityDirections.Any())
            {
                int countItems = 0;

                jsonLoadData.CommodityDirections.ForEach(
                    x =>
                    {
                        CommodityDirectionEntity oldCommodityDirection = GetCommodityDirection(dataBaseContext, x.UID);

                        if (oldCommodityDirection != null)
                        {
                            Update(oldCommodityDirection, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            CommodityDirectionEntity newCommodityDirection = Assemble(x);

                            if (newCommodityDirection != null)
                            {
                                dataBaseContext.CommodityDirectionEntities.Add(newCommodityDirection);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
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

        private void Update(CommodityDirectionEntity entity, Json.Contract.CommodityDirection item, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && item != null)
            {
                if (Equals(entity, item))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = item.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(CommodityDirectionEntity entity, Json.Contract.CommodityDirection item)
        {
            Guid code;
            return entity != null && item != null &&
                   item.UID.ConvertToGuid(out code) && entity.Code == code &&
                   entity.Name == item.Name;
        }

        private CommodityDirectionEntity GetCommodityDirection(DataBaseContext dataBaseContext, string code)
        {
            CommodityDirectionEntity commodityDirection = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                commodityDirection = GetCommodityDirection(dataBaseContext, guidCode);
            }

            return commodityDirection;
        }

        private CommodityDirectionEntity GetCommodityDirection(DataBaseContext dataBaseContext, Guid code)
        {
            CommodityDirectionEntity commodityDirection =
                dataBaseContext.CommodityDirectionEntities.FirstOrDefault(x => x.Code == code);
            return commodityDirection;
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

        private void CreateNomenclatureGroupItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.NomenclatureGroups != null && jsonLoadData.NomenclatureGroups.Any())
            {
                int countItems = 0;

                jsonLoadData.NomenclatureGroups.ForEach(
                    x =>
                    {
                        NomenclatureGroupEntity oldNomenclatureGroup = GetNomenclatureGroup(dataBaseContext, x.UID);

                        if (oldNomenclatureGroup != null)
                        {
                            Update(oldNomenclatureGroup, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            NomenclatureGroupEntity newNomenclatureGroup = Assemble(x, loadUpdateTime);

                            if (newNomenclatureGroup != null)
                            {
                                dataBaseContext.NomenclatureGroupEntities.Add(newNomenclatureGroup);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
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

        private void Update(NomenclatureGroupEntity entity, NomenclatureGroup item, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && item != null)
            {
                if (Equals(entity, item))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = item.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(NomenclatureGroupEntity entity, NomenclatureGroup item)
        {
            Guid code;
            return entity != null && item != null && 
                   item.UID.ConvertToGuid(out code) && entity.Code == code &&
                   entity.Name == item.Name;
        }

        private NomenclatureGroupEntity GetNomenclatureGroup(DataBaseContext dataBaseContext, string code)
        {
            NomenclatureGroupEntity nomenclatureGroup = null;
            Guid guidCode;

            if (code.ConvertToGuid(out guidCode))
            {
                nomenclatureGroup = GetNomenclatureGroup(dataBaseContext, guidCode);
            }

            return nomenclatureGroup;
        }

        private NomenclatureGroupEntity GetNomenclatureGroup(DataBaseContext dataBaseContext, Guid code)
        {
            NomenclatureGroupEntity nomenclatureGroup =
                dataBaseContext.NomenclatureGroupEntities.FirstOrDefault(x => x.Code == code);
            return nomenclatureGroup;
        }

        private NomenclatureGroupEntity Assemble(NomenclatureGroup nomenclatureGroup, DateTimeOffset loadUpdateTime)
        {
            NomenclatureGroupEntity nomenclatureGroupItem = null;

            if (nomenclatureGroup != null)
            {
                nomenclatureGroupItem = new NomenclatureGroupEntity
                {
                    Code = nomenclatureGroup.UID.ConvertToGuid(),
                    Name = nomenclatureGroup.Name,
                    DateOfCreation = loadUpdateTime,
                    LastUpdated = loadUpdateTime,
                    ForceUpdated = loadUpdateTime
                };
            }

            return nomenclatureGroupItem;
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext, DateTimeOffset loadUpdateTime)
        {
            Dictionary< CommodityDirection, Guid> directionList = new Dictionary<CommodityDirection, Guid>
            {
                { CommodityDirection.Vaz,        Guid.Parse("1FF0EBCD-1507-40E9-A409-2AF3B8F77D49") },
                { CommodityDirection.Gaz,        Guid.Parse("9B6B4325-B435-4D65-925E-4921F09D461F") },
                { CommodityDirection.Zaz,        Guid.Parse("264D3368-ECA8-4F2F-9C3B-7B7CC582C7FD") },
                { CommodityDirection.Chemistry,  Guid.Parse("78DC5231-C9D8-49A1-9FC5-AD18BF71DC13") },
                { CommodityDirection.Battery,    Guid.Parse("C7667731-5CFE-4BB6-90DC-8520C87F4FA0") },
                { CommodityDirection.Gas,        Guid.Parse("A4151BE4-3D3F-4A18-A3D1-4BF48F03AB6C") },
                { CommodityDirection.Instrument, Guid.Parse("AFEDE7C9-85E0-4C60-A5FB-0D81B0771D3D") },
                { CommodityDirection.Common,     Guid.Parse("80135062-F8F6-4C5A-AFD2-48A0117EB2B6") }
            };

            try
            {
                foreach (KeyValuePair<CommodityDirection, Guid> item in directionList)
                {
                    ProcessingProductDirection(dataBaseContext, item.Key, item.Value, loadUpdateTime);
                }

                dataBaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                ;//throw;
            }
        }

        private void ProcessingProductDirection(DataBaseContext dataBaseContext, CommodityDirection commodityDirection,
                                                Guid directoryGuid, DateTimeOffset loadUpdateTime)
        {
            ProductDirectionEntity item = GetProductDirection(dataBaseContext, commodityDirection);

            if (item != null)
            {
                DirectoryEntity directory = GetDirectoryItem(dataBaseContext, directoryGuid);

                if (item.Directory != directory)
                {
                    item.Directory = directory;
                    item.LastUpdated = loadUpdateTime;
                }
            }
            else
            {
                CreateProductDirection(dataBaseContext, commodityDirection, directoryGuid, loadUpdateTime);
            }
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext, CommodityDirection commodityDirection, 
                                            Guid directoryGuid, DateTimeOffset loadUpdateTime)
        {
            ProductDirectionEntity item = dataBaseContext.ProductDirectionEntities.Create();
            item.Direction = commodityDirection;
            item.DateOfCreation = loadUpdateTime;
            item.ForceUpdated = loadUpdateTime;
            item.LastUpdated = loadUpdateTime;
            item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == directoryGuid);
            dataBaseContext.ProductDirectionEntities.Add(item);
        }

        private ProductDirectionEntity GetProductDirection(DataBaseContext dataBaseContext, CommodityDirection commodityDirection)
        {
            ProductDirectionEntity productDirection =
                dataBaseContext.ProductDirectionEntities.FirstOrDefault(x => x.Direction == commodityDirection);
            return productDirection;
        }

        private void CreateDirectoryItems(DataBaseContext dataBaseContext, JsonLoadData jsonLoadData, DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.Catalogs != null && jsonLoadData.Catalogs.Any())
            {
                int countItems = 0;

                jsonLoadData.Catalogs.ForEach(
                    x =>
                    {
                        DirectoryEntity oldDirectory = GetDirectoryItem(dataBaseContext, x.UID);

                        if (oldDirectory != null)
                        {
                            Update(dataBaseContext, oldDirectory, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            DirectoryEntity newDirectory = Assemble(dataBaseContext, x, loadUpdateTime);

                            if (newDirectory != null)
                            {
                                dataBaseContext.DirectoryEntities.Add(newDirectory);
                                countItems++;
                            }
                        }

                        if (countItems % 10 == 0)
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

        private void Update(DataBaseContext dataBaseContext, DirectoryEntity entity, Directory jsonItem, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = jsonItem.Name;
                    entity.SubDirectory
                        .RemoveAll(x => jsonItem.Subdirectory.All(i =>
                        {
                            bool result = true;
                            Guid code;

                            if (i.UID.ConvertToGuid(out code))
                            {
                                result = x.Code != code;
                            }

                            return result;
                        }));

                    jsonItem.Subdirectory
                        .Where(x =>
                        {
                            bool result = false;
                            Guid code;

                            if (x.UID.ConvertToGuid(out code))
                            {
                                result = entity.SubDirectory.All(i => i.Code != code);
                            }

                            return result;
                        })
                        .ToList()
                        .ForEach(x =>
                        {
                            DirectoryEntity subDirectory = Assemble(dataBaseContext, x, loadUpdateTime);
                            entity.SubDirectory.Add(subDirectory);
                        });

                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }


        private bool Equals(DirectoryEntity entity, Directory jsonItem)
        {
            Guid code;
            return jsonItem.UID.ConvertToGuid(out code) && entity.Code == code && 
                entity.Name == jsonItem.Name &&
                entity.SubDirectory.All(x => jsonItem.Subdirectory.Any(i =>
                {
                    bool result = false;
                    Guid codeSubdirectory;

                    if (i.UID.ConvertToGuid(out codeSubdirectory))
                    {
                        result = x.Code == code;
                    }

                    return result;
                })) &&
                jsonItem.Subdirectory.All(x => entity.SubDirectory.Any(i =>
                {
                    bool result = false;
                    Guid codeSubdirectory;

                    if (x.UID.ConvertToGuid(out codeSubdirectory))
                    {
                        result = i.Code == code;
                    }

                    return result;
                }));
        }

        private DirectoryEntity Assemble(DataBaseContext dataBaseContext, Directory directory, DateTimeOffset loadUpdateTime)
        {
            DirectoryEntity directoryItem = null;

            if (directory != null)
            {
                List<DirectoryEntity> subDirectories = null;

                if (directory.Subdirectory != null && directory.Subdirectory.Any())
                {
                    subDirectories = new List<DirectoryEntity>();
                    directory.Subdirectory.Where(x => x != null).ToList().ForEach(
                        x =>
                        {
                            DirectoryEntity subDir = GetDirectoryItem(dataBaseContext, x.UID);

                            if (subDir != null)
                            {
                                subDirectories.Add(subDir);
                                Update(dataBaseContext, subDir, x, loadUpdateTime);
                            }
                            else
                            {
                                subDirectories.Add(Assemble(dataBaseContext, x, loadUpdateTime));
                            }
                        });
                }

                directoryItem = new DirectoryEntity
                {
                    Code = directory.UID.ConvertToGuid(),
                    Name = directory.Name,
                    SubDirectory = subDirectories,
                    DateOfCreation = loadUpdateTime,
                    LastUpdated = loadUpdateTime,
                    ForceUpdated = loadUpdateTime
                };
            }

            return directoryItem;
        }

        private void CreateBrandItems(DataBaseContext dataBaseContext, 
                                      JsonLoadData jsonLoadData, 
                                      DateTimeOffset loadUpdateTime)
        {
            if (dataBaseContext != null && jsonLoadData?.Brands != null && jsonLoadData.Brands.Any())
            {
                int countItems = 0;

                jsonLoadData.Brands.ForEach(
                    x =>
                    {
                        BrandItemEntity oldBrandItem = GetBrandItem(dataBaseContext, x.UID);

                        if (oldBrandItem != null)
                        {
                            Update(oldBrandItem, x, loadUpdateTime);
                            countItems++;
                        }
                        else
                        {
                            BrandItemEntity newBrandItem = Assemble(x, loadUpdateTime);

                            if (newBrandItem != null)
                            {
                                dataBaseContext.BrandItemEntities.Add(newBrandItem);
                                countItems++;
                            }
                        }

                        if (countItems % 100 == 0)
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

        private void Update(BrandItemEntity entity, Brand jsonItem, DateTimeOffset loadUpdateTime)
        {
            if (entity != null && jsonItem != null)
            {
                if (Equals(entity, jsonItem))
                {
                    entity.ForceUpdated = loadUpdateTime;
                }
                else
                {
                    entity.Name = jsonItem.Name;
                    entity.LastUpdated = loadUpdateTime;
                }
            }
        }

        private bool Equals(BrandItemEntity entity, Brand jsonItem)
        {
            Guid code;
            return jsonItem.UID.ConvertToGuid(out code) && entity.Code == code && entity.Name == jsonItem.Name;
        }

        private BrandItemEntity Assemble(Brand brand, DateTimeOffset loadUpdateTime)
        {
            BrandItemEntity brandItem = null;

            if (brand != null)
            {
                brandItem = new BrandItemEntity
                {
                    Code = brand.UID.ConvertToGuid(),
                    Name = brand.Name,
                    DateOfCreation = loadUpdateTime,
                    ForceUpdated = loadUpdateTime,
                    LastUpdated = loadUpdateTime
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
