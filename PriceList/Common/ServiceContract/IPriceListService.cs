using System;
using System.ServiceModel;
using Common.Domain.Implementation;

namespace Common.ServiceContract
{
    [ServiceContract]
    public interface IPriceListService
    {
        [OperationContract(Name = "UpdatePriceList")]
        PriceList UpdatePriceList(string securityString, DateTime? lastUpdateDateTime);
    }
}
