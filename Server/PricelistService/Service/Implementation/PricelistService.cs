using System;
using System.Linq;
using DataBase.Context;
using DataBase.Context.Entities;
using DataBase.Context.Object;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "PricelistService" в коде и файле конфигурации.
    public class PricelistService : IPricelistService
    {
        #region Methods

        public CompanyInfo Hello(string login, DateTimeOffset timeRequest)
        {
            DataBaseContext dataBaseContext = new DataBaseContext();
            CreateProductDirection(dataBaseContext);

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

                dataBaseContext.SaveChanges();
            }
            catch (Exception e)
            {
                ;//throw;
            }
        }

        #endregion
    }
}
