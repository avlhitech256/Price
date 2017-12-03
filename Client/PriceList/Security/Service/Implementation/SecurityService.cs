using System;
using Options.Service;

namespace Security.Service.Implementation
{
    public class SecurityService : ISecurityService
    {
        #region Members

        private readonly IOptionService optionService;

        #endregion

        #region Constructors

        public SecurityService(IOptionService optionService)
        {
            this.optionService = optionService;
        }

        #endregion

        #region Properties

        public string Login => optionService?.Login;

        #endregion

        #region Methods

        public string GetPassword(DateTimeOffset time)
        {
            string password = DecryptPassword();
            string securityPassword = EncryptPassword(password, time);
            return securityPassword;
        }

        private string DecryptPassword()
        {
            string securityPassword = optionService.Password;
            return securityPassword;
        }

        private string EncryptPassword(string password, DateTimeOffset time)
        {
            return password;
        }

        #endregion
    }
}
