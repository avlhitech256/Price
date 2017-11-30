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

        public CompanyInfo Hello(string login, DateTimeOffset timeRequest)
        {
            CompanyInfo result = new CompanyInfo();

            result.Title = "Hello " + login;
            result.CompanyName = "Autotrend";
            result.Phone = "+380 (57) 784-18-81";
            result.WebSite = "http://autotrend.ua/";
            result.EMail = "office@autotrend.ua";
            result.TimeRequest = timeRequest;
            result.TimeResponce = DateTimeOffset.Now;

            return result;
        }

        public CountInfo PrepareToUpdate(string login, DateTimeOffset lastUpdate, bool needLoadPhotos)
        {
            IShapingBrands shapingBrands = new ShapingBrands(dataService, optionService);
            IShapingCatalogs shapingCatalogs = new ShapingCatalogs(dataService, optionService);

            CountInfo result = new CountInfo
            {
                CountBrands = shapingBrands.PrepareToUpdate(login, lastUpdate),
                CountCatalog = shapingCatalogs.PrepareToUpdate(login, lastUpdate),
                CountDirectory = 0,
                CountPhoto = needLoadPhotos ? 1 : 0
            };

            return result;
        }

        public BrandInfo GetBrand(long id)
        {
            IShapingBrands shaping = new ShapingBrands(dataService, optionService);
            BrandInfo brands = shaping.GetItem(id);
            return brands;
        }

        public Brands GetBrands(string login, DateTimeOffset lastUpdate)
        {
            IShapingBrands shaping = new ShapingBrands(dataService, optionService);
            Brands brands = shaping.GetItems(login, lastUpdate);
            return brands;
        }

        public void ConfirmUpdateBrands(string login, DateTimeOffset lastUpdate, List<long> itemIds)
        {
            IShapingBrands shaping = new ShapingBrands(dataService, optionService);
            shaping.ConfirmUpdate(login, itemIds);
        }

        public CatalogInfo GetCatalog(long id)
        {
            IShapingCatalogs shaping = new ShapingCatalogs(dataService, optionService);
            CatalogInfo brands = shaping.GetItem(id);
            return brands;
        }

        public Catalogs GetCatalogs(string login, DateTimeOffset lastUpdate)
        {
            IShapingCatalogs shaping = new ShapingCatalogs(dataService, optionService);
            Catalogs brands = shaping.GetItems(login, lastUpdate);
            return brands;
        }

        public void ConfirmUpdateCatalogs(string login, List<long> itemIds)
        {
            IShapingCatalogs shaping = new ShapingCatalogs(dataService, optionService);
            shaping.ConfirmUpdate(login, itemIds);
        }

        #endregion
    }
}
