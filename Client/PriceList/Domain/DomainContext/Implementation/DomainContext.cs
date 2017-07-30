using System;
using Common.Annotations;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using DatabaseService.DataService;
using DatabaseService.DataService.Implementation;
using Domain.Service.Precision;
using Domain.Service.Precision.Implementation;
using Domain.ViewModel;
using Media.Color;
using Media.Color.Implementation;
using Media.Image;
using Photo.Service;
using Photo.Service.Implementation;

namespace Domain.DomainContext.Implementation
{
    public class DomainContext : Notifier, IDomainContext
    {
        #region Members

        private string dataBaseServer;
        private IControlViewModel viewModel;
        private bool isEditControl;

        #endregion

        #region Constructors

        public DomainContext()
        {
            Messenger = new Messenger();
            ColorService = new ColorService();
            UserName = Environment.UserName;
            UserDomain = Environment.UserDomainName;
            Workstation = Environment.MachineName;
            DataBaseServer = string.Empty;
            ImageService = new ImageService();
            PhotoService = new PhotoService(Messenger, ImageService);
            PrecisionService = new PrecisionService(2, true);
            DataService = new DataService();
        }

        #endregion

        #region Properties

        public IMessenger Messenger { get; }
        public IColorService ColorService { get; }
        public IControlViewModel ViewModel
        {
            get
            {
                return viewModel;
            }

            set
            {
                if (viewModel != value)
                {
                    viewModel = value;
                    OnPropertyChanged();
                }
            }

        }

        public IImageService ImageService { get; }
        public IPhotoService PhotoService { get; }

        public IPrecisionService PrecisionService { get; }
        public IDataService DataService { get; }

        public bool IsEditControl
        {
            get
            {
                return isEditControl;
            }

            set
            {
                if (isEditControl != value)
                {
                    isEditControl = value;
                    OnPropertyChanged();
                }
            }
        }
        
        [CanBeNull]
        public string UserName { get; }

        [CanBeNull]
        public string UserDomain { get; }

        [CanBeNull]
        public string Workstation { get; }

        [CanBeNull]
        public string DataBaseServer
        {
            get
            {
                return dataBaseServer;
            }

            set
            {
                if (dataBaseServer != value)
                {
                    dataBaseServer = value;
                    OnPropertyChanged();
                }

            }

        }
        
        #endregion
    }
}
