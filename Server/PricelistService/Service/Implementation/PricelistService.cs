using System;
using System.Collections.Generic;
using DataBase.Service;
using DataBase.Service.Implementation;
using Option.Service;
using Option.Service.Implementation;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "PricelistService" в коде и файле конфигурации.
    public class PricelistService : IPricelistService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public PricelistService()
        {
            dataService = new DataService();
            optionService = new OptionService();
        }

        #endregion

        #region Methods

        public CompanyInfo Hello(SecurityInfo securityInfo)
        {
            var result = new CompanyInfo
            {
                Title = "Hello " + securityInfo?.Login,
                CompanyName = "Autotrend",
                Phone = "+380 (57) 784-18-81",
                WebSite = "http://autotrend.ua/",
                EMail = "office@autotrend.ua",
                TimeRequest = securityInfo?.TimeRequest ?? DateTimeOffset.Now,
                TimeResponce = DateTimeOffset.Now,
                IsAuthorized = ValidatePassword(securityInfo)
            };

            return result;
        }

        public ShortcutInfo Shortcut(long id, DateTimeOffset requestTime)
        {
            var shortcutInfo = new ShortcutInfo
            {
                Id = id,
                RequestTime = requestTime,
                ResponceTime =  DateTimeOffset.Now
            };

            return shortcutInfo;
        }

        public bool ChangePasswodr(SecurityInfo securityInfo, string newPassword)
        {
            bool result = false;

            if (ValidatePassword(securityInfo))
            {
                ISecurityService securityService = new SecurityService(dataService, optionService);
                result = securityService.ChangePasswodr(securityInfo, newPassword);
            }

            return result;
        }

        public CountInfo PrepareToUpdate(SecurityInfo securityInfo, 
                                         DateTimeOffset lastUpdateBrands,
                                         DateTimeOffset lastUpdateCatalogs,
                                         DateTimeOffset lastUpdateDirectories,
                                         DateTimeOffset lastUpdateProductDirections,
                                         DateTimeOffset lastUpdatePhotos,
                                         bool needLoadPhotos,
                                         long[] ids)
        {
            CountInfo result = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingBrands shapingBrands = new ShapingBrands(dataService, optionService);
                IShapingCatalogs shapingCatalogs = new ShapingCatalogs(dataService, optionService);
                IShapingDirectories shapingDirectories = new ShapingDirectories(dataService, optionService);
                IShapingProductDirections shapingProductDirections = new ShapingProductDirections(dataService,
                    optionService);
                IShapingPhotos shapingPhotos = new ShapingPhotos(dataService, optionService);

                result = new CountInfo
                {
                    CountBrands = shapingBrands.PrepareToUpdate(securityInfo.Login, lastUpdateBrands),
                    CountCatalogs = shapingCatalogs.PrepareToUpdate(securityInfo.Login, lastUpdateCatalogs),
                    CountDirectories = shapingDirectories.PrepareToUpdate(securityInfo.Login, lastUpdateDirectories),
                    CountProductDirections = shapingProductDirections.PrepareToUpdate(securityInfo.Login, lastUpdateProductDirections),
                    CountPhotos = needLoadPhotos ? shapingPhotos.PrepareToUpdate(securityInfo.Login, lastUpdatePhotos, ids) : 0,
                    IsAuthorized = true
                };
            }

            return result;
        }

        public CountPhotosInfo PrepareToUpdatePhotos(SecurityInfo securityInfo,
                                                     DateTimeOffset lastUpdatePhotos,
                                                     bool needLoadPhotos,
                                                     long[] ids)
        {
            CountPhotosInfo countPhotosInfo = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingPhotos shapingPhotos = new ShapingPhotos(dataService, optionService);
                countPhotosInfo = new CountPhotosInfo
                {
                    CountPhotos = needLoadPhotos ? shapingPhotos.PrepareToUpdate(securityInfo.Login, lastUpdatePhotos, ids) : 0,
                    IsAuthorized = true
                };
            }

            return countPhotosInfo;
        }

        public BrandInfo GetBrand(SecurityInfo securityInfo, long id)
        {
            BrandInfo brands = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingBrands shaping = new ShapingBrands(dataService, optionService);
                brands = shaping.GetItem(id);
            }

            return brands;
        }

        public Brands GetBrands(SecurityInfo securityInfo, DateTimeOffset lastUpdate)
        {
            Brands brands = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingBrands shaping = new ShapingBrands(dataService, optionService);
                brands = shaping.GetItems(securityInfo.Login, lastUpdate);
            }

            return brands;
        }

        public void ConfirmUpdateBrands(SecurityInfo securityInfo, List<long> itemIds)
        {
            if (ValidatePassword(securityInfo))
            {
                IShapingBrands shaping = new ShapingBrands(dataService, optionService);
                shaping.ConfirmUpdate(securityInfo.Login, itemIds);
            }
        }

        public CatalogInfo GetCatalog(SecurityInfo securityInfo, long id)
        {
            CatalogInfo catalogInfo = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingCatalogs shaping = new ShapingCatalogs(dataService, optionService);
                catalogInfo = shaping.GetItem(id);
            }

            return catalogInfo;
        }

        public Catalogs GetCatalogs(SecurityInfo securityInfo, DateTimeOffset lastUpdate)
        {
            Catalogs catalogs = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingCatalogs shaping = new ShapingCatalogs(dataService, optionService);
                catalogs = shaping.GetItems(securityInfo.Login, lastUpdate);
            }

            return catalogs;
        }

        public void ConfirmUpdateCatalogs(SecurityInfo securityInfo, List<long> itemIds)
        {
            if (ValidatePassword(securityInfo))
            {
                IShapingCatalogs shaping = new ShapingCatalogs(dataService, optionService);
                shaping.ConfirmUpdate(securityInfo.Login, itemIds);
            }
        }

        public DirectoryInfo GetDirectory(SecurityInfo securityInfo, long id)
        {
            DirectoryInfo directory = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingDirectories shaping = new ShapingDirectories(dataService, optionService);
                directory = shaping.GetItem(id);
            }

            return directory;
        }

        public Directories GetDirectories(SecurityInfo securityInfo, DateTimeOffset lastUpdate)
        {
            Directories directories = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingDirectories shaping = new ShapingDirectories(dataService, optionService);
                directories = shaping.GetItems(securityInfo.Login, lastUpdate);
            }

            return directories;
        }

        public void ConfirmUpdateDirectories(SecurityInfo securityInfo, List<long> itemIds)
        {
            if (ValidatePassword(securityInfo))
            {
                IShapingDirectories shaping = new ShapingDirectories(dataService, optionService);
                shaping.ConfirmUpdate(securityInfo.Login, itemIds);
            }
        }

        public PhotoInfo GetPhoto(SecurityInfo securityInfo, long id)
        {
            PhotoInfo photoInfo = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingPhotos shaping = new ShapingPhotos(dataService, optionService);
                photoInfo = shaping.GetItem(id);
            }

            return photoInfo;
        }

        public Photos GetPhotos(SecurityInfo securityInfo, DateTimeOffset lastUpdate, long[] ids)
        {
            Photos photos = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingPhotos shaping = new ShapingPhotos(dataService, optionService);
                photos = shaping.GetItems(securityInfo.Login, lastUpdate, ids);
            }

            return photos;
        }

        public void ConfirmUpdatePhotos(SecurityInfo securityInfo, List<long> itemIds)
        {
            if (ValidatePassword(securityInfo))
            {
                IShapingPhotos shaping = new ShapingPhotos(dataService, optionService);
                shaping.ConfirmUpdate(securityInfo.Login, itemIds);
            }
        }

        public ProductDirectionInfo GetProductDirection(SecurityInfo securityInfo, long id)
        {
            ProductDirectionInfo productDirectionInfo = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingProductDirections shaping = new ShapingProductDirections(dataService, optionService);
                productDirectionInfo = shaping.GetItem(id);
            }

            return productDirectionInfo;
        }

        public ProductDirections GetProductDirections(SecurityInfo securityInfo, DateTimeOffset lastUpdate)
        {
            ProductDirections productDirections = null;

            if (ValidatePassword(securityInfo))
            {
                IShapingProductDirections shaping = new ShapingProductDirections(dataService, optionService);
                productDirections = shaping.GetItems(securityInfo.Login, lastUpdate);
            }

            return productDirections;
        }

        public void ConfirmUpdateProductDirections(SecurityInfo securityInfo, List<long> itemIds)
        {
            if (ValidatePassword(securityInfo))
            {
                IShapingProductDirections shaping = new ShapingProductDirections(dataService, optionService);
                shaping.ConfirmUpdate(securityInfo.Login, itemIds);
            }
        }

        private bool ValidatePassword(SecurityInfo securityInfo)
        {
            ISecurityService securityService = new SecurityService(dataService, optionService);
            return securityService.ValidatePassword(securityInfo);
        }

        #endregion
    }
}
