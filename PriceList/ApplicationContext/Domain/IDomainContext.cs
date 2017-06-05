using Common.Domain;
using DataService.Service;

namespace ApplicationContext.Domain
{
    public interface IDomainContext
    {
        IDataService DataService { get; }
        ILogService LogService { get; }
    }
}
