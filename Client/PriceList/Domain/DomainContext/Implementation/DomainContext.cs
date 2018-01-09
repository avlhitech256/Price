using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Async.Service;
using Common.Annotations;
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
using Media.Image.Implementation;
using Options.Service;
using Options.Service.Implementation;
using Photo.Service;
using Photo.Service.Implementation;
using Repository.Repository;
using Repository.Repository.Implementation;
using Template.Service;
using Template.Service.Implementation;
using UserDecisions.Service;
using UserDecisions.Service.Implementation;
using Web.Service;
using Web.Service.Implementation;
using Web.WebServiceReference;

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
        private bool accessToInternet;
        private readonly DispatcherTimer pingTimer;
        private string pingTime;
        private bool isSynchronizeEntry;

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

            Messenger = new Messenger();
            ColorService = new ColorService();
            ImageService = new ImageService();
            PhotoService = new PhotoService(Messenger, ImageService);
            PrecisionService = new PrecisionService(2, true);
            DataService = new DataService();
            UserDecisionsService = new UserDecisionsService();
            AsyncOperationService = new AsyncOperationService(UserDecisionsService, UIContext.Current);
            OptionService = new OptionService();
            WebService = new WebService(OptionService);
            TemplateService = new TemplateService(OptionService);
            ConvertService = new ConvertService();
            BrandRepository = new BrandRepository(DataService);
            BrandRepository.Load();
            DirectoryRepository = new DirectoryRepository(DataService);
            DirectoryRepository.Load();
            Init();
            SubscribeEvents();
            pingTimer = new DispatcherTimer();
            pingTimer.Interval = TimeSpan.FromMilliseconds(1000);
            pingTimer.Tick += PingTimerOnTick;
            pingTimer.Start();
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

        public IUserDecisionsService UserDecisionsService { get; }

        public IAsyncOperationService AsyncOperationService { get; }

        public IWebService WebService { get; }

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

        public ITemplateService TemplateService { get; }

        public IConvertService ConvertService { get; }

        public IBrandRepository BrandRepository { get; }

        public IDirectoryRepository DirectoryRepository { get; }

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

        public Action CloseMainWindow { get; set; }

        public bool AccessToInternet
        {
            get
            {
                return accessToInternet;
            }
            set
            {
                if (accessToInternet != value)
                {
                    accessToInternet = value;
                    OnChangeAccessToInternet(value);
                    OnPropertyChanged();
                }
            }
        }

        public string PingTime
        {
            get
            {
                return pingTime;
            }
            set
            {
                if (pingTime != value)
                {
                    pingTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSynchronizeEntry
        {
            get
            {
                return isSynchronizeEntry;
            }
            set
            {
                if (isSynchronizeEntry != value)
                {
                    isSynchronizeEntry = value;
                    SetPingInterval();
                    OnPropertyChanged();
                }
            }
        }

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

        private void OnChangeAccessToInternet(bool enabled)
        {
            var args = new AccessToInternetEventArgs(enabled);
            AccessToInternetChanged?.Invoke(this, args);
        }

        private void PingTimerOnTick(object sender, EventArgs eventArgs)
        {
            pingTimer.Stop();

            try
            {
                if (WebService != null)
                {
                    ShortcutInfo shortCutIfo = WebService.Shortcut();
                    DateTimeOffset now = DateTimeOffset.Now;
                    AccessToInternet = shortCutIfo != null &&
                                       shortCutIfo.ResponceTime > DateTimeOffset.MinValue &&
                                       shortCutIfo.RequestTime > DateTimeOffset.MinValue &&
                                       shortCutIfo.RequestTime < now;

                    PingTime = shortCutIfo != null 
                        ? (now - shortCutIfo.RequestTime).Milliseconds + " ms"
                        : string.Empty;

                }
            }
            catch (Exception)
            {
                AccessToInternet = false;
                PingTime = string.Empty;
            }

            SetPingInterval();
            pingTimer.Start();
        }

        private void SetPingInterval()
        {
            pingTimer.Interval = AccessToInternet && !IsSynchronizeEntry
                ? TimeSpan.FromMilliseconds(15000)
                : TimeSpan.FromMilliseconds(1000);
        }

        #endregion

        #region Events

        public event AccessToInternetEventHandler AccessToInternetChanged;

        #endregion
    }

}
