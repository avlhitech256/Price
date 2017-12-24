using System.ComponentModel;
using Common.Annotations;
using Common.Messenger;
using Common.Service;
using DatabaseService.DataService;
using Domain.Service.Precision;
using Domain.ViewModel;
using Media.Color;
using Media.Image;
using Options.Service;
using Photo.Service;
using Repository.Repository;
using Template.Service;

namespace Domain.DomainContext
{
    public interface IDomainContext : ILoadingContext, INotifyPropertyChanged
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

        [CanBeNull]
        IAsyncOperationService AsyncOperationService { get; }

        bool IsEditControl { get; set; }

        string UserName { get; }

        string UserDomain { get; }

        string Workstation { get; }

        string DataBaseServer { get; set; }

        string OverdueAccountsReceivable { get; set; }

        string Debd { get; set; }

        IOptionService OptionService { get; }

        ITemplateService TemplateService { get; }

        IConvertService ConvertService { get; }

        IBrandRepository BrandRepository { get; }
        IDirectoryRepository DirectoryRepository { get; }

        void Refresh();
    }
}
