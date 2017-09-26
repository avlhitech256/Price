using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using Common.Annotations;
using Common.Convert.Implementation;
using DatabaseService.DataBaseContext.Entities;
using DatabaseService.Properties;
using File.Service;
using File.Service.Implementation;
using Json.Contract;
using Json.Service;
using Json.Service.Implementation;
using Media.Image;
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
            }
            else
            {
                string dataPath = "Data\\";
                string photoPath = dataPath + "Photo\\";
                string photoSearchPattern = "*.jpeg";
                string metaDataFileName = "MetaData.json";
                string clientsFileName = "Clients.json";
                string priceFileName = "PriceList.json";

                LoadPictures(dataBaseContext, photoPath, photoSearchPattern);

                MetaData metaData = fileService.ReadMetaData(dataPath + metaDataFileName);
                Clients clients = fileService.ReadClients(dataPath + clientsFileName);
                PriceList pricelist = fileService.ReadPriceList(dataPath + priceFileName);

                List<BrandItemEntity> brandItems =
                    CreateBrandItems(dataBaseContext, metaData);
                List<DirectoryEntity> directoryItems =
                    CreateDirectoryItems(dataBaseContext, metaData);
                List<NomenclatureGroupEntity> nomenclatureGroupItems =
                    CreateNomenclatureGroupItems(dataBaseContext, metaData);
                List<CommodityDirectionEntity> commodityDirectionsItems =
                    CreateCommodityDirectionsItems(dataBaseContext, metaData);
                CreateCatalogItems(dataBaseContext, pricelist, brandItems, directoryItems,
                                   nomenclatureGroupItems, commodityDirectionsItems);
                PopulateOptionItemEntities(dataBaseContext, clients);
            }

            dataBaseContext.SaveChanges();
            base.Seed(dataBaseContext);
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

                            if (countOfFiles%10 == 0)
                            {
                                dataBaseContext.SaveChanges();
                            }
                        }
                    });

                dataBaseContext.SaveChanges();
            }
        }

        private List<BrandItemEntity> CreateBrandItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            var brandItems = new List<BrandItemEntity>();

            if (dataBaseContext != null && metaData?.Brands != null && metaData.Brands.Any())
            {
                metaData.Brands.ToList().ForEach(
                    x =>
                    {
                        BrandItemEntity brandItem = Assemble(x);

                        if (brandItem != null)
                        {
                            brandItems.Add(brandItem);
                        }
                    });

                dataBaseContext.BrandItemEntities.AddRange(brandItems);
            }

            return brandItems;
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

        private List<DirectoryEntity> CreateDirectoryItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            var directoryItems = new List<DirectoryEntity>();

            if (dataBaseContext != null && metaData?.Catalog != null && metaData.Catalog.Any())
            {
                metaData.Catalog.ToList().ForEach(
                    x =>
                    {
                        DirectoryEntity directory = Assemble(x);

                        if (directory != null)
                        {
                            directoryItems.Add(directory);
                        }
                    });

                dataBaseContext.DirectoryEntities.AddRange(directoryItems);
            }

            return directoryItems;
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

        private List<NomenclatureGroupEntity> CreateNomenclatureGroupItems(DataBaseContext dataBaseContext, MetaData metaData)
        {
            var nomenclatureGroupItems = new List<NomenclatureGroupEntity>();

            if (dataBaseContext != null && metaData?.NomenclatureGroups != null && metaData.NomenclatureGroups.Any())
            {
                metaData.NomenclatureGroups.ToList().ForEach(
                    x =>
                    {
                        NomenclatureGroupEntity nomenclatureGroup = Assemble(x);

                        if (nomenclatureGroup != null)
                        {
                            nomenclatureGroupItems.Add(nomenclatureGroup);
                        }
                    });
                dataBaseContext.NomenclatureGroupEntities.AddRange(nomenclatureGroupItems);
            }

            return nomenclatureGroupItems;
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

        private List<CommodityDirectionEntity> CreateCommodityDirectionsItems(DataBaseContext dataBaseContext, 
                                                                              MetaData metaData)
        {
            var commodityDirectionsItems = new List<CommodityDirectionEntity>();

            if (dataBaseContext != null && metaData?.CommodityDirections != null && metaData.CommodityDirections.Any())
            {
                metaData.CommodityDirections.ToList().ForEach(
                    x =>
                    {
                        CommodityDirectionEntity commodityDirection = Assemble(x);

                        if (commodityDirection != null)
                        {
                            commodityDirectionsItems.Add(commodityDirection);
                        }
                    });
                dataBaseContext.CommodityDirectionEntities.AddRange(commodityDirectionsItems);
            }

            return commodityDirectionsItems;
        }

        private CommodityDirectionEntity Assemble(CommodityDirection commodityDirection)
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

        private void CreateCatalogItems(DataBaseContext dataBaseContext, 
                                        PriceList pricelist, 
                                        List<BrandItemEntity> brandItems, 
                                        List<DirectoryEntity> directoryItems,
                                        List<NomenclatureGroupEntity> nomenclatureGroupItems,
                                        List<CommodityDirectionEntity> commodityDirectionItems)
        {
            if (dataBaseContext != null && pricelist?.Nomenclature != null)
            {
                var catalogItems = new List<CatalogItemEntity>();

                pricelist.Nomenclature.ToList().ForEach(
                    x => catalogItems.Add(Assemble(dataBaseContext, x, brandItems, directoryItems,
                        nomenclatureGroupItems, commodityDirectionItems)));
                dataBaseContext.CatalogItemEntities.AddRange(catalogItems);
            }
        }

        [NotNull]
        private CatalogItemEntity Assemble(DataBaseContext dataBaseContext, 
                                           Nomenclature nomenclature, 
                                           List<BrandItemEntity> brandItems,
                                           List<DirectoryEntity> directoryItems,
                                           List<NomenclatureGroupEntity> nomenclatureGroupItems,
                                           List<CommodityDirectionEntity> commodityDirectionItems)
        {
            BrandItemEntity brandItem = brandItems?.FirstOrDefault(x => x.Code == nomenclature.BrandUID.ConvertToNullableGuid());
            DirectoryEntity directoryItem = directoryItems?.FirstOrDefault(x => x.Code == nomenclature.CatalogUID.ConvertToNullableGuid());
            NomenclatureGroupEntity nomenclatureGroupItem =
                nomenclatureGroupItems?.FirstOrDefault(x => x.Code == nomenclature.NomenclatureGroupUID.ConvertToNullableGuid());
            List<CommodityDirectionEntity> commodityDirectionItemsForCatalogItem =
                commodityDirectionItems?.Where(
                    x => nomenclature.CommodityDirection.Any(c => c.ConvertToNullableGuid() == x.Code)).ToList() ??
                new List<CommodityDirectionEntity>();
            List<PhotoItemEntity> photoItems =
                dataBaseContext.PhotoItemEntities.Where(x => nomenclature.Photos.Any(p => p == x.Name)).ToList();
            List<string> needToCreatePhotos = nomenclature.Photos.Where(x => photoItems.All(p => p.Name != x)).ToList();
            photoItems.AddRange(CreateEmptyPhotos(dataBaseContext, needToCreatePhotos));

            var catalogItem = new CatalogItemEntity
            {

                UID = nomenclature.UID.ConvertToGuid(),
                Code = nomenclature.Code,
                Article = nomenclature.VendorCode,
                Name = nomenclature.Name,
                Brand = brandItem,
                Unit = nomenclature.Measure,
                EnterpriceNormPack = nomenclature.NormPackaging.ToString(CultureInfo.InvariantCulture),
                BatchOfSales = nomenclature.BatchOfSales.ConvertToDecimal(),
                Balance = nomenclature.InStock,
                Price = GetPrice(nomenclature),
                Currency = string.Empty,
                Multiplicity = 0,
                HasPhotos = photoItems.Any(x => x.IsLoad),
                Photos = photoItems,
                DateOfCreation = DateTimeOffset.Now,
                LastUpdated = DateTimeOffset.Now,
                PriceIsUp = false,
                PriceIsDown = false,
                IsNew = false,
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

        private IEnumerable<PhotoItemEntity> CreateEmptyPhotos(DataBaseContext dataBaseContext, List<string> photos)
        {
            var emptyPhotos = new List<PhotoItemEntity>();

            photos.ForEach(x => emptyPhotos.Add(new PhotoItemEntity {Name = x, IsLoad = false, Photo = null}));
            dataBaseContext.PhotoItemEntities.AddRange(emptyPhotos);

            return emptyPhotos;
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
            List<OptionItemEntity> optionItems = new List<OptionItemEntity>
            {
                new OptionItemEntity
                {
                    Code = "LOGIN",
                    Name = "User Login",
                    Value = clients != null && clients.Contragent.Any()
                        ? clients.Contragent.FirstOrDefault()?.Login
                        : "autotrend"
                },
                new OptionItemEntity
                {
                    Code = "PASSWORD",
                    Name = "User Password",
                    Value = ""
                },
                new OptionItemEntity
                {
                    Code = "LASTORDERNUMBER",
                    Name = "Last Order Number",
                    Value = "0"
                },
            };

            dataBaseContext.OptionItemEntities.AddRange(optionItems);
        }
    }
}
