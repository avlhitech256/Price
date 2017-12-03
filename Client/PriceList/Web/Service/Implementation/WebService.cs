using System;
using System.Net.Mime;
using DatabaseService.DataService;
using Options.Service;
using Security.Service;
using Security.Service.Implementation;
using Web.WebServiceReference;

namespace Web.Service.Implementation
{
    public class WebService : IWebService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IPricelistService pricelistServiceClient;
        private readonly IOptionService optionService;
        private readonly ISecurityService securityService;

        #endregion

        #region Constructors

        public WebService(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
            pricelistServiceClient = new PricelistServiceClient();
            securityService = new SecurityService(optionService);
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
            bool result;

            using (var webService = GetWebService())
            {
                SecurityInfo securityInfo = CreateSecurityInfo();
                CompanyInfo companyInfo = webService.Hello(securityInfo);
                result = companyInfo.IsAuthorized;
                webService.Close();
            }

            return result;
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

        #endregion
    }
}
