using Common.Domain;
using DataService.Service;

namespace ApplicationContext.Domain.Implementation
{
    public class DomainContext : IDomainContext
    {
        public IDataService DataService { get; }
        public ILogService LogService { get; }
    }
}
