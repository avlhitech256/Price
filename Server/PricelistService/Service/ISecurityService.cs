using PricelistService.Service.Contract;

namespace PricelistService.Service
{
    public interface ISecurityService
    {
        bool ValidatePassword(SecurityInfo securityInfo);

        bool ChangePasswodr(SecurityInfo securityInfo, string newPassword);
    }
}
