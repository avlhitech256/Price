using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Annotations;
using Common.Data.Enum;
using Common.Service.Implementation;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.Objects;
using DatabaseService.Properties;
using File.Service;
using File.Service.Implementation;
using Json.Contract;
using Json.Service;
using Json.Service.Implementation;
using Media.Image;
using CommodityDirection = Common.Data.Enum.CommodityDirection;
using Directory = Json.Contract.Directory;

namespace DatabaseService.DataBaseContext.Initializer
{
    public class DataBaseInitializer : DropCreateDatabaseIfModelChanges<DataBaseContext>
    {
        private readonly IImageService imageService;
        private readonly IJsonService jsonService;
        private readonly IFileService fileService;

        public DataBaseInitializer()
        {
            imageService = new ImageService();
            jsonService = new JsonService();
            fileService = new FileService(jsonService);
        }
        protected override void Seed(DataBaseContext dataBaseContext)
        {
            bool loadFakeData = false;

            if (loadFakeData)
            {
                List<BrandItemEntity> brandItems = PopulateBrandItemEntities(dataBaseContext);
                List<PhotoItemEntity> photoItems = PopulatePhotoEntities(dataBaseContext);
                PopulateCatalogItemEntities(dataBaseContext, brandItems, photoItems);
                PopulateOptionItemEntities(dataBaseContext);
                dataBaseContext.SaveChanges();
            }
            else
            {
                string dataPath = "Data\\";
                string photoPath = dataPath + "Photo\\";
                string photoSearchPattern = "*.jpeg";
                string metaDataFileName = "MetaData.json";
                string clientsFileName = "Clients.json";
                string priceFileName = "PriceList.json";

                MetaData metaData = fileService.ReadMetaData(dataPath + metaDataFileName);
                PriceList pricelist = fileService.ReadPriceList(dataPath + priceFileName);
                Clients clients = fileService.ReadClients(dataPath + clientsFileName);
                Pause10();

                // Отключаем отслеживание и проверку изменений для оптимизации вставки множества полей
                dataBaseContext.Configuration.AutoDetectChangesEnabled = false;
                dataBaseContext.Configuration.ValidateOnSaveEnabled = false;

                CreateBrandItems(dataBaseContext, metaData);
                CreateDirectoryItems(dataBaseContext, metaData);
                CreateProductDirection(dataBaseContext);
                CreateNomenclatureGroupItems(dataBaseContext, metaData);
                CreateCommodityDirectionsItems(dataBaseContext, metaData);
                PopulateOptionItemEntities(dataBaseContext, clients);

                LoadPictures(dataBaseContext, photoPath, photoSearchPattern);

                CreateCatalogItems(dataBaseContext, pricelist);

                dataBaseContext.Configuration.AutoDetectChangesEnabled = true;
                dataBaseContext.Configuration.ValidateOnSaveEnabled = true;
            }

            base.Seed(dataBaseContext);
        }

        private void Pause10()
        {
            Thread.Sleep(10);
        }

        private void Save(DataBaseContext dataBaseContext)
        {
            dataBaseContext.SaveChanges();
            Pause10();
        }

        private void LoadPictures(DataBaseContext dataBaseContext, string photoPath, string photoSearchPattern = null)
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

                            var photoItem = new PhotoItemEntity
                            {
                                Name = fileService.GetFileName(x.Name),
                                IsLoad = true,
                                Photo = picture
                            };

                            dataBaseContext.PhotoItemEntities.Add(photoItem);
                            countOfFiles++;

                            if (countOfFiles % 10 == 0)
                            {
                                Save(dataBaseContext);
                            }
                        }
                    });

                Save(dataBaseContext);
            }
        }

        private void CreateProductDirection(DataBaseContext dataBaseContext)
        {
            try
            {
                ProductDirectionEntity item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Vaz;
                Guid code = Guid.Parse("1FF0EBCD-1507-40E9-A409-2AF3B8F77D49");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Gaz;
                code = Guid.Parse("9B6B4325-B435-4D65-925E-4921F09D461F");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Zaz;
                code = Guid.Parse("264D3368-ECA8-4F2F-9C3B-7B7CC582C7FD");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Chemistry;
                code = Guid.Parse("78DC5231-C9D8-49A1-9FC5-AD18BF71DC13");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Battery;
                code = Guid.Parse("C7667731-5CFE-4BB6-90DC-8520C87F4FA0");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Gas;
                code = Guid.Parse("A4151BE4-3D3F-4A18-A3D1-4BF48F03AB6C");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Instrument;
                code = Guid.Parse("AFEDE7C9-85E0-4C60-A5FB-0D81B0771D3D");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                item = dataBaseContext.ProductDirectionEntities.Create();
                item.Direction = CommodityDirection.Common;
                code = Guid.Parse("80135062-F8F6-4C5A-AFD2-48A0117EB2B6");
                item.Directory = dataBaseContext.DirectoryEntities.FirstOrDefault(x => x.Code == code);
                dataBaseContext.ProductDirectionEntities.Add(item);

                Save(dataBaseContext);
            }
            catch (Exception e)
            {
                ;//throw;
            }
        }

        private void CreateBrandItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            if (dataBaseContext != null && metaData?.Brands != null && metaData.Brands.Any())
            {
                int countInsertedItems = 0;

                metaData.Brands.ToList().ForEach(
                    x =>
                    {
                        BrandItemEntity brandItem = Assemble(x);

                        if (brandItem != null)
                        {
                            dataBaseContext.BrandItemEntities.Add(brandItem);
                            countInsertedItems++;

                            if (countInsertedItems % 100 == 0)
                            {
                                Save(dataBaseContext);
                            }
                        }
                    });

                Save(dataBaseContext);
            }
        }

        private BrandItemEntity Assemble(Brand brand)
        {
            BrandItemEntity brandItem = null;

            if (brand != null)
            {
                brandItem = new BrandItemEntity
                {
                    Code = brand.UID.ConvertToGuid(),
                    Name = brand.Name
                };
            }

            return brandItem;
        }

        private void CreateDirectoryItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            if (dataBaseContext != null && metaData?.Catalog != null && metaData.Catalog.Any())
            {
                int countInsertedItems = 0;

                metaData.Catalog.ToList().ForEach(
                    x =>
                    {
                        DirectoryEntity directory = Assemble(x);

                        if (directory != null)
                        {
                            dataBaseContext.DirectoryEntities.Add(directory);

                            if (countInsertedItems % 10 == 0)
                            {
                                Save(dataBaseContext);
                            }
                        }
                    });

                Save(dataBaseContext);
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

                directoryItem = new DirectoryEntity
                {
                    Code = directory.UID.ConvertToGuid(),
                    Name = directory.Name,
                    SubDirectory = subDirectories
                };
            }

            return directoryItem;
        }

        private void CreateNomenclatureGroupItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            if (dataBaseContext != null && metaData?.NomenclatureGroups != null && metaData.NomenclatureGroups.Any())
            {
                int countInsertedItems = 0;

                metaData.NomenclatureGroups.ToList().ForEach(
                    x =>
                    {
                        NomenclatureGroupEntity nomenclatureGroup = Assemble(x);

                        if (nomenclatureGroup != null)
                        {
                            dataBaseContext.NomenclatureGroupEntities.Add(nomenclatureGroup);

                            if (countInsertedItems % 100 == 0)
                            {
                                Save(dataBaseContext);
                            }
                        }
                    });

                Save(dataBaseContext);
            }
        }

        private NomenclatureGroupEntity Assemble(NomenclatureGroup nomenclatureGroup)
        {
            NomenclatureGroupEntity nomenclatureGroupItem = null;

            if (nomenclatureGroup != null)
            {
                nomenclatureGroupItem = new NomenclatureGroupEntity
                {
                    Code = nomenclatureGroup.UID.ConvertToGuid(),
                    Name = nomenclatureGroup.Name
                };
            }

            return nomenclatureGroupItem;
        }

        private void CreateCommodityDirectionsItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            if (dataBaseContext != null && metaData?.CommodityDirections != null && metaData.CommodityDirections.Any())
            {
                int countInsertedItems = 0;

                metaData.CommodityDirections.ToList().ForEach(
                    x =>
                    {
                        CommodityDirectionEntity commodityDirection = Assemble(x);

                        if (commodityDirection != null)
                        {
                            dataBaseContext.CommodityDirectionEntities.Add(commodityDirection);

                            if (countInsertedItems % 100 == 0)
                            {
                                Save(dataBaseContext);
                            }
                        }
                    });

                Save(dataBaseContext);
            }
        }

        private CommodityDirectionEntity Assemble(Json.Contract.CommodityDirection commodityDirection)
        {
            CommodityDirectionEntity commodityDirectionItem = null;

            if (commodityDirection != null)
            {
                commodityDirectionItem = new CommodityDirectionEntity
                {

                    Code = commodityDirection.UID.ConvertToGuid(),
                    Name = commodityDirection.Name
                };
            }

            return commodityDirectionItem;
        }

        private void CreateCatalogItems(DataBaseContext dataBaseContext, PriceList pricelist)
        {
            if (dataBaseContext != null && pricelist?.Nomenclature != null && pricelist.Nomenclature.Any())
            {
                int countOfCatalogItem = 0;

                pricelist.Nomenclature.ToList().ForEach(
                    x =>
                    {
                        dataBaseContext.CatalogItemEntities.Add(Assemble(dataBaseContext, x));
                        countOfCatalogItem++;

                        if (countOfCatalogItem % 100 == 0)
                        {
                            Save(dataBaseContext);
                        }
                    });

                Save(dataBaseContext);
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
                LastUpdated = DateTimeOffset.Now,
                Status = GenerateStatus(),
                LastUpdatedStatus = DateTimeOffset.Now,
                BasketItems = null,
                Directory = directoryItem,
                NomenclatureGroup = nomenclatureGroupItem,
                CommodityDirection = commodityDirectionItemsForCatalogItem
            };

            if (brandItem != null)
            {
                if (brandItem.CatalogItems != null)
                {
                    brandItem.CatalogItems.Add(catalogItem);
                }
                else
                {
                    brandItem.CatalogItems = new List<CatalogItemEntity> {catalogItem};
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
                    directoryItem.CatalogItems = new List<CatalogItemEntity> {catalogItem};
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
                    nomenclatureGroupItem.CatalogItems = new List<CatalogItemEntity> {catalogItem};
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
                                x.CatalogItems = new List<CatalogItemEntity> {catalogItem};
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

        private static bool EqualStringToGuid(string stringGuid, Guid guid)
        {
            Guid comparerGuid;
            bool result = !string.IsNullOrWhiteSpace(stringGuid) &&
                          stringGuid.ConvertToGuid(out comparerGuid) &&
                          comparerGuid == guid;
            return result;
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

            photos.ForEach(x => emptyPhotos.Add(new PhotoItemEntity {Name = x, IsLoad = false, Photo = null}));
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

        private decimal GetPrice(Nomenclature nomenclature)
        {
            decimal price = 0;

            if (nomenclature?.TypesOfPrices != null && nomenclature.TypesOfPrices.Any())
            {
                PriceTypeItem typeOfPrice = nomenclature.TypesOfPrices.FirstOrDefault();

                if (typeOfPrice != null)
                {
                    price = PriceParse(typeOfPrice.Price);
                }
            }

            return price;
        }

        private string GetCurrency(Nomenclature nomenclature)
        {
            string currency = string.Empty;

            if (nomenclature?.TypesOfPrices != null && nomenclature.TypesOfPrices.Any())
            {
                PriceTypeItem typeOfPrice = nomenclature.TypesOfPrices.FirstOrDefault();

                if (typeOfPrice != null)
                {
                    currency = CurrencyParse(typeOfPrice.Price);
                }
            }

            return currency;
        }

        private decimal PriceParse(string priceWithCurrency)
        {
            decimal price = 0;

            if (!string.IsNullOrWhiteSpace(priceWithCurrency))
            {
                string[] splitPriceString = priceWithCurrency.Split(' ');

                if (splitPriceString.Any())
                {
                    string stringPrice = splitPriceString[0];

                    if (!string.IsNullOrWhiteSpace(stringPrice))
                    {
                        price = decimal.TryParse(stringPrice, out price) ? price : 0;
                    }
                }
            }

            return price;
        }

        private string CurrencyParse(string priceWithCurrency)
        {
            string  currency = string.Empty;

            if (!string.IsNullOrWhiteSpace(priceWithCurrency))
            {
                string[] splitPriceString = priceWithCurrency.Split(' ');

                if (splitPriceString.Any() && splitPriceString.Length >= 2)
                {
                    currency = splitPriceString[1];
                }
            }

            return currency;
        }

        private void CreateIndex(DataBaseContext dataBaseContext)
        {
            dataBaseContext.Database.ExecuteSqlCommand
                ("CREATE INDEX Index_CatalogItemEntity_Name ON CatalogItemEntities (Name)");
            //("CREATE INDEX Index_CatalogItemEntity_Name ON CatalogItemEntities (Id) INCLUDE (Name)");
        }

        private List<BrandItemEntity> PopulateBrandItemEntities(DataBaseContext dataBaseContext)
        {
            List<BrandItemEntity> brandItems = new List<BrandItemEntity>
            {
                new BrandItemEntity
                {
                    Code = Guid.NewGuid(),
                    Name = "АВТОПАРТНЕР ООО Г.ДИМИТРОВГРАД"
                },
                new BrandItemEntity
                {
                    Code = Guid.NewGuid(),
                    Name = "АВТОВАЗ"
                },
                new BrandItemEntity
                {
                    Code = Guid.NewGuid(),
                    Name = "Daewoo-ZAZ"
                }
            };

            dataBaseContext.BrandItemEntities.AddRange(brandItems);

            return brandItems;
        }

        private List<PhotoItemEntity> PopulatePhotoEntities(DataBaseContext dataBaseContext)
        {
            List<PhotoItemEntity> photoItems = new List<PhotoItemEntity>
            {
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo1), Name = "Photo1"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo2), Name = "Photo2"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo3), Name = "Photo3"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo2), Name = "Photo2"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo3), Name = "Photo3"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo4), Name = "Photo4"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo3), Name = "Photo3"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo2), Name = "Photo2"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo4), Name = "Photo4"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo5), Name = "Photo5"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo5), Name = "Photo5"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo6), Name = "Photo6"},
                new PhotoItemEntity {Photo = imageService.ConvertToByteArray(Resources.Photo3), Name = "Photo3"}
            };

            dataBaseContext.PhotoItemEntities.AddRange(photoItems);

            return photoItems;
        }

        private void PopulateCatalogItemEntities(DataBaseContext dataBaseContext, 
                                                 List<BrandItemEntity> brandItems, 
                                                 List<PhotoItemEntity> photoItems)
        {
            List<CatalogItemEntity> catalogItems = new List<CatalogItemEntity>
            {
                new CatalogItemEntity
                {
                    Code = "04157",
                    UID = Guid.NewGuid(),
                    Article = "11180-1108054-00",
                    Brand = brandItems[0],
                    Name = "Трос акселератора ВАЗ 1118 дв. 1,6л",
                    EnterpriceNormPack = "10 шт.",
                    Balance = "свыше 100 шт.",
                    Unit = "шт.",
                    Price = 50.52M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    HasPhotos = true,
                    Photos = new List<PhotoItemEntity> {photoItems[0], photoItems[1], photoItems[2]}
                },
                new CatalogItemEntity
                {
                    Code = "87930",
                    UID = Guid.NewGuid(),
                    Article = "3160-3819010-00",
                    Brand = brandItems[1],
                    Name = "Ступица передняя HYUNDAI H-1 07- Без ABS",
                    EnterpriceNormPack = "10 шт.",
                    Balance = "свыше 100 шт.",
                    Unit = "шт.",
                    Price = 320.45M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    HasPhotos = false,
                    Photos = null //new List<PhotoItemEntity> {photoItems[3], photoItems[4], photoItems[5]}
                },
                new CatalogItemEntity
                {
                    Code = "98486",
                    UID = Guid.NewGuid(),
                    Article = "21213",
                    Brand = brandItems[0],
                    Name = "Крышка бачка расширительного DAEWOO LANOS/SENS",
                    EnterpriceNormPack = "10 шт.",
                    Balance = "от 10 до 100 шт.",
                    Unit = "шт.",
                    Price = 18.65M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    HasPhotos = true,
                    Photos = new List<PhotoItemEntity> {photoItems[6], photoItems[7]}
                },
                new CatalogItemEntity
                {
                    Code = "24029",
                    UID = Guid.NewGuid(),
                    Article = "11180-3508180-00",
                    Brand = brandItems[2],
                    Name = "Баллон тороидальный 42л 600х200мм наружный, Atiker",
                    EnterpriceNormPack = "1 шт.",
                    Balance = "8 шт.",
                    Unit = "шт.",
                    Price = 120.78M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    HasPhotos = true,
                    Photos = new List<PhotoItemEntity> {photoItems[8], photoItems[9]}
                },
                new CatalogItemEntity
                {
                    Code = "24029",
                    UID = Guid.NewGuid(),
                    Article = "3302-8406140-00",
                    Brand = brandItems[0],
                    Name = "Трос капота ГАЗ 3302",
                    EnterpriceNormPack = "1 шт.",
                    Balance = "2 шт.",
                    Unit = "шт.",
                    Price = 1850.46M,
                    Currency = "грн.",
                    LastUpdated = DateTimeOffset.Now,
                    Multiplicity = 1.00M,
                    HasPhotos = false,
                    Photos = null//new List<PhotoItemEntity> {photoItems[10], photoItems[11], photoItems[12]}
                }
            };

            dataBaseContext.CatalogItemEntities.AddRange(catalogItems);
        }
        private void PopulateOptionItemEntities(DataBaseContext dataBaseContext, Clients clients = null)
        {
            Client client = clients != null && clients.Contragent.Any() ? clients.Contragent.FirstOrDefault() : null;

            List<OptionItemEntity> optionItems = new List<OptionItemEntity>
            {
                new OptionItemEntity
                {
                    Code = OptionName.Login,
                    Name = "User Login",
                    Value = client != null ? client.Login : "autotrend"
                },
                new OptionItemEntity
                {
                    Code = OptionName.Password,
                    Name = "User Password",
                    Value = ""
                },
                new OptionItemEntity
                {
                    Code = OptionName.Debt,
                    Name = "Mutual Settlements",
                    Value = client?.MutualSettlements
                },
                new OptionItemEntity
                {
                    Code = OptionName.OverdueAccountsReceivable,
                    Name = "Overdue accounts receivable",
                    Value = client?.PDZ
                },
                new OptionItemEntity
                {
                    Code = OptionName.LastOrderNumber,
                    Name = "Last Order Number",
                    Value = "0"
                },
                new OptionItemEntity
                {
                    Code = OptionName.CatalogMaximumRows,
                    Name = "Maximum Rows Displayed in Catalog Entry",
                    Value = "13"
                }
            };

            dataBaseContext.OptionItemEntities.AddRange(optionItems);
            Save(dataBaseContext);
        }
    }
}
