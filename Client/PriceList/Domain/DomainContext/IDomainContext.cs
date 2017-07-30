using Common.Annotations;
using Common.Messenger;
using DatabaseService.DataService;
using Domain.Service.Precision;
using Domain.ViewModel;
using Media.Color;
using Media.Image;
using Photo.Service;

namespace Domain.DomainContext
{
    public interface IDomainContext
    {
        [CanBeNull]
        IMessenger Messenger { get; }

        [CanBeNull]
        IColorService ColorService { get; }

        [CanBeNull]
        IControlViewModel ViewModel { get; set; }

        [CanBeNull]
        IImageService ImageService { get; }

        [CanBeNull]
        IPhotoService PhotoService { get; }

        [CanBeNull]
        IPrecisionService PrecisionService { get; }

        [CanBeNull]
        IDataService DataService { get; }

        bool IsEditControl { get; set; }

        string UserName { get; }

        string UserDomain { get; }

        string Workstation { get; }

        string DataBaseServer { get; set; }
    }
}
