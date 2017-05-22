using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Domain;

namespace Common.ServiceContract
{
    [ServiceContract]
    public interface IPriceList
    {
        [OperationContract]
        IEnumerable<IPriceListItem> UpdatePriceList(string securityString, DateTime? lastUpdateDateTime);
    }
}
