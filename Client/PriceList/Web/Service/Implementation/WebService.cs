﻿using System;
using System.Collections.Generic;
using System.Linq;
using Options.Service;
using Security.Service;
using Security.Service.Implementation;
using Web.WebServiceReference;

namespace Web.Service.Implementation
{
    public class WebService : IWebService
    {
        #region Members

        private readonly ISecurityService securityService;
        private long attempt;

        #endregion

        #region Constructors

        public WebService(IOptionService optionService)
        {
            securityService = new SecurityService(optionService);
            attempt = 0;
        }

        #endregion

        #region Properties

        #endregion

        #region Methods

        private PricelistServiceClient GetWebService()
        {
            var webService = new PricelistServiceClient("BasicHttpBinding_IPricelistService");
            return webService;
        }

        public bool CheckPassword()
        {
            return Hello().IsAuthorized;
        }

        public CompanyInfo Hello()
        {
            CompanyInfo companyInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                companyInfo = webService.Hello(securityInfo);
                webService.Close();
            }

            return companyInfo;
        }

        public ShortcutInfo Shortcut()
        {
            ShortcutInfo shortcutInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                shortcutInfo = webService.Shortcut(attempt++, DateTimeOffset.Now);
                webService.Close();
            }

            return shortcutInfo;
        }

        private SecurityInfo CreateSecurityInfo()
        {
            DateTimeOffset time = DateTimeOffset.Now;
            string login = securityService.Login;
            string password = securityService.GetPassword(time);

            SecurityInfo securityInfo = new SecurityInfo
            {
                Login = login,
                Password = password,
                TimeRequest = time,
                TypeSecurity = "N",
                Version = Environment.Version.ToString(),
                OSLogin = Environment.UserName,
                Workstation = Environment.MachineName,
                Domain = Environment.UserDomainName,
                OSVersion = Environment.OSVersion.ToString()
            };

            return securityInfo;
        }

        public BrandInfo GetBrandInfo(long id)
        {
            BrandInfo brandInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                brandInfo = webService.GetBrand(securityInfo, id);
                webService.Close();
            }

            return brandInfo;
        }

        public Brands GetBrands(DateTimeOffset lastUpdate)
        {
            Brands brands;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                brands = webService.GetBrands(securityInfo, lastUpdate);
                webService.Close();
            }

            return brands;
        }

        public CatalogInfo GetCatalogInfo(long id)
        {
            CatalogInfo catalogInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                catalogInfo = webService.GetCatalog(securityInfo, id);
                webService.Close();
            }

            return catalogInfo;
        }

        public Catalogs GetCatalogs(DateTimeOffset lastUpdate)
        {
            Catalogs catalogs;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                catalogs = webService.GetCatalogs(securityInfo, lastUpdate);
                webService.Close();
            }

            return catalogs;
        }

        public CountInfo PrepareToUpdate(DateTimeOffset lastUpdateBrands, 
                                         DateTimeOffset lastUpdateCatalogs, 
                                         DateTimeOffset lastUpdateDirectories, 
                                         DateTimeOffset lastUpdateProductDirections, 
                                         DateTimeOffset lastUpdatePhotos, 
                                         bool needLoadPhotos,
                                         List<long> ids)
        {
            CountInfo countInfo;

            int maxPhotos = 1000;

            if (ids.Count() > maxPhotos)
            {
                int countPages = ids.Count / maxPhotos;

                for (int i = 0; i < countPages; i++)
                {
                    List<long> idsList = ids.Skip(maxPhotos * i).Take(maxPhotos).ToList();
                    PrepareToUpdatePhotos(lastUpdatePhotos, needLoadPhotos, idsList);
                }

                ids = ids.Skip(maxPhotos * countPages).Take(maxPhotos).ToList();
            }

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                countInfo = webService.PrepareToUpdate(securityInfo, 
                                                       lastUpdateBrands, 
                                                       lastUpdateCatalogs, 
                                                       lastUpdateDirectories, 
                                                       lastUpdateProductDirections, 
                                                       lastUpdatePhotos, 
                                                       needLoadPhotos,
                                                       ids.ToArray()); 
                webService.Close();
            }

            return countInfo;
        }

        private CountPhotosInfo PrepareToUpdatePhotos(DateTimeOffset lastUpdatePhotos,
                                                     bool needLoadPhotos,
                                                     IEnumerable<long> ids)
        {
            CountPhotosInfo countPhotosInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                countPhotosInfo = webService.PrepareToUpdatePhotos(securityInfo, 
                                                                   lastUpdatePhotos, 
                                                                   needLoadPhotos, 
                                                                   ids.ToArray());
            }

            return countPhotosInfo;
        }

        public DirectoryInfo GetDirectoryInfo(long id)
        {
            DirectoryInfo directory;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                directory = webService.GetDirectory(securityInfo, id);
                webService.Close();
            }

            return directory;
        }

        public Directories GetDirectories(DateTimeOffset lastUpdate)
        {
            Directories directories;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                directories = webService.GetDirectories(securityInfo, lastUpdate);
                webService.Close();
            }

            return directories;
        }

        public PhotoInfo GetPhotoInfo(long id)
        {
            PhotoInfo photoInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                photoInfo = webService.GetPhoto(securityInfo, id);
                webService.Close();
            }

            return photoInfo;
        }

        public Photos GetPhotos(DateTimeOffset lastUpdate, IEnumerable<long> ids)
        {
            Photos photos;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                photos = webService.GetPhotos(securityInfo, lastUpdate, ids.ToArray());
                webService.Close();
            }

            return photos;
        }

        public ProductDirectionInfo GetProductDirectionInfo(long id)
        {
            ProductDirectionInfo productDirectionInfo;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                productDirectionInfo = webService.GetProductDirection(securityInfo, id);
                webService.Close();
            }

            return productDirectionInfo;
        }

        public ProductDirections GetProductDirections(DateTimeOffset lastUpdate)
        {
            ProductDirections productDirections;

            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                productDirections = webService.GetProductDirections(securityInfo, lastUpdate);
                webService.Close();
            }

            return productDirections;
        }

        public void ConfirmUpdateBrands(IEnumerable<long> itemIds)
        {
            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                webService.ConfirmUpdateBrands(securityInfo, itemIds.ToArray());
                webService.Close();
            }
        }

        public void ConfirmUpdateCatalogs(IEnumerable<long> itemIds)
        {
            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                webService.ConfirmUpdateCatalogs(securityInfo, itemIds.ToArray());
                webService.Close();
            }
        }

        public void ConfirmUpdateDirectories(IEnumerable<long> itemIds)
        {
            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                webService.ConfirmUpdateDirectories(securityInfo, itemIds.ToArray());
                webService.Close();
            }
        }

        public void ConfirmUpdatePhotos(IEnumerable<long> itemIds)
        {
            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                webService.ConfirmUpdatePhotos(securityInfo, itemIds.ToArray());
                webService.Close();
            }
        }

        public void ConfirmUpdateProductDirections(IEnumerable<long> itemIds)
        {
            using (PricelistServiceClient webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                webService.ConfirmUpdateProductDirections(securityInfo, itemIds.ToArray());
                webService.Close();
            }
        }

        #endregion
    }
}
