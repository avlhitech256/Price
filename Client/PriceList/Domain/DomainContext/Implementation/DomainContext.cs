using System;
using System.Linq;
using System.Windows;
using Common.Annotations;
using Common.Convert;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.Service;
using Common.Service.Implementation;
using Common.Thread;
using DatabaseService.DataService;
using DatabaseService.DataService.Implementation;
using Domain.Service.Precision;
using Domain.Service.Precision.Implementation;
using Domain.ViewModel;
using Media.Color;
using Media.Color.Implementation;
using Media.Image;
using Options.Service;
using Options.Service.Implementation;
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
        private IOptionService _optionService;
        private string overdueAccountsReceivable;
        private string debd;
        private bool isLoading;
        private bool isWaiting;
        private readonly AsyncOperationType[] waitFormSupported;

        #endregion

        #region Constructors

        public DomainContext()
        {
            IsLoading = false;
            IsWaiting = false;
            waitFormSupported = new[]
{
                AsyncOperationType.LoadCatalog,
                AsyncOperationType.LoadBrands,
                AsyncOperationType.LoadDirectories,
                AsyncOperationType.CheckDatabase, 
                AsyncOperationType.GetSumBasket
            };

            AsyncOperationService = new AsyncOperationService(UIContext.Current);
            Messenger = new Messenger();
            ColorService = new ColorService();
            ImageService = new ImageService();
            PhotoService = new PhotoService(Messenger, ImageService);
            PrecisionService = new PrecisionService(2, true);
            DataService = new DataService();
            OptionService = new OptionService(DataService);
            ConvertService = new ConvertService();
            Init();
            SubscribeEvents();
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

        public IAsyncOperationService AsyncOperationService { get; }

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
        public string UserName { get; private set; }

        [CanBeNull]
        public string UserDomain { get; private set; }

        [CanBeNull]
        public string Workstation { get; private set; }

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

        public string OverdueAccountsReceivable
        {
            get
            {
                return overdueAccountsReceivable;
            }
            set
            {
                if (overdueAccountsReceivable != value)
                {
                    overdueAccountsReceivable = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Debd
        {
            get
            {
                return debd;
            }
            set
            {
                if (debd != value)
                {
                    debd = value;
                    OnPropertyChanged();
                }
            }
        }

        public IOptionService OptionService { get; }

        public IConvertService ConvertService { get; }

        public bool IsLoading
        {
            get
            {
                return isLoading;
            }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsWaiting
        {
            get
            {
                return isWaiting;
            }
            set
            {
                if (isWaiting != value)
                {
                    isWaiting = value;
                    OnPropertyChanged();
                }
            }
        }

        public Action<string> ShowWaitScreen { get; set; }

        public Action<string> SetWaitScreenMessage { get; set; }

        public Action HideWaitScreen { get; set; }

        #endregion

        #region Methods

        private void Init()
        {
            UserName = OptionService.Login; //Environment.UserName;
            UserDomain = Environment.UserDomainName;
            Workstation = Environment.MachineName;
            Refresh();
        }

        public void Refresh()
        {
            DataBaseServer = string.Empty;
            OverdueAccountsReceivable = OptionService.OverdueAccountsReceivable;
            Debd = OptionService.Debt;
        }

        public void SetWaitMessage(AsyncOperationType type)
        {
            SetWaitScreenMessage?.Invoke(GetDescription(type));
        }

        private void SubscribeEvents()
        {
            if (AsyncOperationService != null)
            {
                AsyncOperationService.OperationStarted += (s, args) =>
                {
                    if (waitFormSupported.Contains(args.Value))
                    {
                        ShowWaitScreenWithType(args.Value);
                    }

                    Application.Current.Dispatcher.Invoke(
                        () => Messenger?.Send(CommandName.EnableMenu, new EnableMenuEventArgs(false)));
                };

                AsyncOperationService.OperationCompleted += (s, args) =>
                {
                    if (waitFormSupported.Contains(args.Value))
                    {
                        HideWaitScreen?.Invoke();
                    }

                    Application.Current.Dispatcher.Invoke(
                        () => Messenger?.Send(CommandName.EnableMenu, new EnableMenuEventArgs(true)));
                };
            }
        }

        private void ShowWaitScreenWithType(AsyncOperationType type)
        {
            ShowWaitScreen?.Invoke(GetDescription(type));
        }

        private static string GetDescription(AsyncOperationType type)
        {
            string description;

            switch (type)
            {
                case AsyncOperationType.LoadCatalog:
                    description = "Подождите, идет формирование страницы прайс листа";
                    break;
                case AsyncOperationType.LoadBrands:
                    description = "Подождите, идет формирование списка брендов";
                    break;
                case AsyncOperationType.LoadDirectories:
                    description = "Подождите, идет формирование списка каталогов";
                    break;
                default:
                    description = "Подождите, идет загрузка данных";
                    break;
            }

            return description;
        }

        #endregion
    }

}
