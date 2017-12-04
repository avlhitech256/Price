using DataBase.Service;
using Option.Service;
using PricelistService.Service.Contract;

namespace PricelistService.Service.Implementation
{
    public class SecurityService : ISecurityService
    {
        #region Members

        private readonly IDataService dataService;
        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public SecurityService(IDataService dataService, IOptionService optionService)
        {
            this.dataService = dataService;
            this.optionService = optionService;
        }

        #endregion

        #region Methods

        public bool ValidatePassword(SecurityInfo securityInfo)
        {
            return true;
        }

        public bool ChangePasswodr(SecurityInfo securityInfo, string newPassword)
        {
            return true;
        }

        #endregion
    }
}
