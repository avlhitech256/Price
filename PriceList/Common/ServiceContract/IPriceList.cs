using System;
using System.ServiceModel;
using Common.Domain.Implementation;

namespace Common.ServiceContract
{
    [ServiceContract]
    public interface IPriceList
    {
        [OperationContract(Name = "UpdatePriceList")]
        PriceList UpdatePriceList(string securityString, DateTime? lastUpdateDateTime);
    }
}
