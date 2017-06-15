using Common.Annotations;
using Common.Messenger;

namespace Domain.DomainContext
{
    public interface IDomainContext
    {
        [CanBeNull]
        IMessenger Messenger { get; }
        string UserName { get; }
        string UserDomain { get; }
        string Workstation { get; }
        string DataBaseServer { get; set; }
    }
}
