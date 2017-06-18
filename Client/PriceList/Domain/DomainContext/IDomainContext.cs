using Common.Annotations;
using Common.Messenger;
using Media.Color;

namespace Domain.DomainContext
{
    public interface IDomainContext
    {
        [CanBeNull]
        IMessenger Messenger { get; }

        [CanBeNull]
        IColorService ColorService { get; }

        string UserName { get; }

        string UserDomain { get; }

        string Workstation { get; }

        string DataBaseServer { get; set; }
    }
}
