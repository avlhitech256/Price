using System;
using System.Linq;
using System.Windows;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Service;
using Common.ViewModel.Command;
using DatabaseService.DataService;
using Domain.DomainContext;
using Domain.ViewModel;
using Load.Service;
using Load.Service.Implementation;
using Web.Service;
using Web.Service.Implementation;
using Web.WebServiceReference;

namespace Synchronize.ViewModel
{
    public class SynchronizeViewModel : Notifier, IControlViewModel
    {
        private string message;
        private string lastUpdate;
        private bool needPhopos;
        private double valueBrands;
        private double maxBrands;
        private readonly IWebService webService;
        private readonly ILoadService loadService;
        private string brandLabel;
        private bool canDoSynchronize;
        private DateTimeOffset lastUpdateBrands;
        private DateTimeOffset lastUpdateCatalogs;
        private DateTimeOffset lastUpdateDirectories;
        private DateTimeOffset lastUpdateProductDirections;
        private DateTimeOffset lastUpdatePhotos;
        private string directoryLabel;
        private string productDirectionLabel;
        private string photoLabel;
        private string catalogLabel;
        private double maxCatalogs;
        private double maxDirectories;
        private double maxPhotos;
        private double maxProductDirections;
        private double valueCatalogs;
        private double valueDirectories;
        private double valuePhotos;
        private double valueProductDirection;

        public SynchronizeViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            webService = new WebService(domainContext?.OptionService);
            loadService = new LoadService(domainContext?.DataService, 
                                          webService, 
                                          domainContext?.BrandRepository, 
                                          domainContext?.DirectoryRepository);
            CreateCommand();
            RefreshLastUpdate();
            InitProperties();
            canDoSynchronize = true;
        }

        public IDomainContext DomainContext { get; }

        private IDataService DataService => DomainContext?.DataService;

        public IMessenger Messenger => DomainContext?.Messenger;

        private IAsyncOperationService AsyncOperationService => DomainContext?.AsyncOperationService;

        public bool ReadOnly { get; set; }

        public bool Enabled { get; set; }

        public bool IsEditControl { get; set; }

        public bool HasChanges { get; }

        public Action RefreshView { get; set; }

        public Action<bool> SetEnabled { get; set; }

        public Func<bool> HasResultGridErrors { get; set; }

        public string Message
        {
            get
            {
                return message;
            }
            set
            {
                if (message != value)
                {
                    message = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastUpdate
        {
            get
            {
                return lastUpdate;
            }
            set
            {
                if (lastUpdate != value)
                {
                    lastUpdate = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool NeedPhopos
        {
            get
            {
                return needPhopos;
            }
            set
            {
                if (needPhopos != value)
                {
                    needPhopos = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxBrands
        {
            get
            {
                return maxBrands;
            }
            set
            {
                if (Math.Abs(maxBrands - value) > double.Epsilon)
                {
                    maxBrands = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxCatalogs
        {
            get
            {
                return maxCatalogs;
            }
            set
            {
                if (Math.Abs(maxCatalogs - value) > double.Epsilon)
                {
                    maxCatalogs = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxDirectories
        {
            get
            {
                return maxDirectories;
            }
            set
            {
                if (Math.Abs(maxDirectories - value) > double.Epsilon)
                {
                    maxDirectories = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxPhotos
        {
            get
            {
                return maxPhotos;
            }
            set
            {
                if (Math.Abs(maxPhotos - value) > double.Epsilon)
                {
                    maxPhotos = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MaxProductDirections
        {
            get
            {
                return maxProductDirections;
            }
            set
            {
                if (Math.Abs(maxProductDirections - value) > double.Epsilon)
                {
                    maxProductDirections = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ValueBrands
        {
            get
            {
                return valueBrands;
            }
            set
            {
                if (Math.Abs(valueBrands - value) > double.Epsilon)
                {
                    valueBrands = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ValueCatalogs
        {
            get
            {
                return valueCatalogs;
            }
            set
            {
                if (Math.Abs(valueCatalogs - value) > double.Epsilon)
                {
                    valueCatalogs = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ValueDirectories
        {
            get
            {
                return valueDirectories;
            }
            set
            {
                if (Math.Abs(valueDirectories - value) > double.Epsilon)
                {
                    valueDirectories = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ValuePhotos
        {
            get
            {
                return valuePhotos;
            }
            set
            {
                if (Math.Abs(valuePhotos - value) > double.Epsilon)
                {
                    valuePhotos = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ValueProductDirection
        {
            get
            {
                return valueProductDirection;
            }
            set
            {
                if (Math.Abs(valueProductDirection - value) > double.Epsilon)
                {
                    valueProductDirection = value;
                    OnPropertyChanged();
                }
            }
        }

        public string BrandLabel
        {
            get
            {
                return brandLabel;
            }
            set
            {
                if (brandLabel != value)
                {
                    brandLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DirectoryLabel
        {
            get
            {
                return directoryLabel;
            }
            set
            {
                if (directoryLabel != value)
                {
                    directoryLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProductDirectionLabel
        {
            get
            {
                return productDirectionLabel;
            }
            set
            {
                if (productDirectionLabel != value)
                {
                    productDirectionLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string PhotoLabel
        {
            get
            {
                return photoLabel;
            }
            set
            {
                if (photoLabel != value)
                {
                    photoLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CatalogLabel
        {
            get
            {
                return catalogLabel;
            }
            set
            {
                if (catalogLabel != value)
                {
                    catalogLabel = value;
                    OnPropertyChanged();
                }
            }
        }

        public DelegateCommand SynchronizeCommand { get; private set; }

        private void CreateCommand()
        {
            SynchronizeCommand = new DelegateCommand(DoSynchronize, CanDoSynchronize);
        }


        private void DoSynchronize(object obj)
        {
            canDoSynchronize = false;
            SynchronizeCommand.RiseCanExecute();
            lastUpdateBrands = DataService.DataBaseContext.BrandItemEntities.Any()
                ? DataService.DataBaseContext.BrandItemEntities.Select(x => x.LastUpdated).Max()
                : DateTimeOffset.MinValue;
            lastUpdateCatalogs = DataService.DataBaseContext.CatalogItemEntities.Any()
                ? DataService.DataBaseContext.CatalogItemEntities.Select(x => x.LastUpdated).Max()
                : DateTimeOffset.MinValue;
            lastUpdateDirectories = DataService.DataBaseContext.DirectoryEntities.Any()
                ? DataService.DataBaseContext.DirectoryEntities.Select(x => x.LastUpdated).Max()
                : DateTimeOffset.MinValue;
            lastUpdateProductDirections = DataService.DataBaseContext.ProductDirectionEntities.Any()
                ? DataService.DataBaseContext.ProductDirectionEntities.Select(x => x.LastUpdated).Max()
                : DateTimeOffset.MinValue;
            lastUpdatePhotos = DataService.DataBaseContext.PhotoItemEntities.Any()
                ? DataService.DataBaseContext.PhotoItemEntities.Select(x => x.LastUpdated).Max()
                : DateTimeOffset.MinValue;

            CountInfo countInfo = webService.PrepareToUpdate(lastUpdateBrands, 
                                                             lastUpdateCatalogs, 
                                                             lastUpdateDirectories,
                                                             lastUpdateProductDirections, 
                                                             lastUpdatePhotos, false);

            ValueBrands = 0;
            ValueDirectories = 0;
            ValueProductDirection = 0;
            ValueCatalogs = 0;
            ValuePhotos = 0;
            MaxBrands = countInfo.CountBrands;
            MaxDirectories = countInfo.CountDirectories;
            MaxProductDirections = countInfo.CountProductDirections;
            MaxCatalogs = countInfo.CountCatalogs;
            MaxPhotos = countInfo.CountPhotos;
            SetBrandsLabel();
            SetDirectoriesLabel();
            SetProductDirectionLabel();
            SetCatalogsLabel();
            SetPhotoLabel();
            Updates();
        }

        private void Updates()
        {
            if ((MaxBrands - ValueBrands) > double.Epsilon)
            {
                UpdateBrands(lastUpdateBrands);
            }
            else if ((MaxDirectories - ValueDirectories) > double.Epsilon)
            {
                UpdateDirectories(lastUpdateDirectories);
            }
            else if ((MaxProductDirections - ValueProductDirection) > double.Epsilon)
            {
                UpdateProductDirection(lastUpdateProductDirections);
            }
            else if ((MaxCatalogs - ValueCatalogs) > double.Epsilon)
            {
                UpdateCatalogs(lastUpdateCatalogs);
            }
            else
            {
                canDoSynchronize = true;
                SynchronizeCommand.RiseCanExecute();
            }
        }

        private void UpdateBrands(DateTimeOffset lastUpdate)
        {
            SetBrandsLabel();
            AsyncOperationService.PerformAsyncOperation(AsyncOperationType.LoadFromWeb,
                                                        LoadBrands,
                                                        lastUpdate,
                                                        UpdateInfoAfterLoadBrands);

        }


        private void UpdateCatalogs(DateTimeOffset lastUpdate)
        {
            SetBrandsLabel();
            AsyncOperationService.PerformAsyncOperation(AsyncOperationType.LoadFromWeb,
                                                        LoadCatalogs,
                                                        lastUpdate,
                                                        SaveCatalogsToDatabase);

        }

        private void UpdateDirectories(DateTimeOffset lastUpdate)
        {
            SetDirectoriesLabel();
            AsyncOperationService.PerformAsyncOperation(AsyncOperationType.LoadFromWeb,
                                                        LoadDirectories,
                                                        lastUpdate,
                                                        UpdateInfoAfterLoadDirectories);

        }

        private void UpdateProductDirection(DateTimeOffset lastUpdate)
        {
            SetProductDirectionLabel();
            AsyncOperationService.PerformAsyncOperation(AsyncOperationType.LoadFromWeb,
                                                        LoadProductDirections,
                                                        lastUpdate,
                                                        SaveProductDirectionsToDatabase);

        }

        private void SaveProductDirectionsToDatabase(Exception e, int countUpdated)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ValueProductDirection += countUpdated;
                SetProductDirectionLabel();
                Updates();
            });
        }

        private int LoadProductDirections(DateTimeOffset lastUpdate)
        {
            SetProductDirectionLabel();
            ProductDirections productDirections = webService.GetProductDirections(lastUpdate);
            loadService.DownLoadProductDirections(productDirections);
            webService.ConfirmUpdateCatalogs(productDirections.Items.Select(x => x.Id));
            return productDirections.Items.Count();
        }

        private void SetProductDirectionLabel()
        {
            Application.Current.Dispatcher.Invoke(() => ProductDirectionLabel = $"Каталоги {ValueProductDirection} из {MaxProductDirections}");
        }

        private void SetPhotoLabel()
        {
            Application.Current.Dispatcher.Invoke(() => PhotoLabel = $"Фотографии {ValuePhotos} из {MaxPhotos}");
        }

        private void UpdateInfoAfterLoadDirectories(Exception e, int countUpdated)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ValueDirectories += countUpdated;
                SetDirectoriesLabel();
                Updates();
            });
        }

        private int LoadDirectories(DateTimeOffset lastUpdate)
        {
            SetDirectoriesLabel();
            Directories directories = webService.GetDirectories(lastUpdate);
            return loadService.DownLoadDirectories(directories);
        }

        private void SetDirectoriesLabel()
        {
            Application.Current.Dispatcher.Invoke(() => DirectoryLabel = $"Каталоги {ValueDirectories} из {MaxDirectories}");
        }

        private int LoadCatalogs(DateTimeOffset lastUpdate)
        {
            SetCatalogsLabel();
            Catalogs catalogs = webService.GetCatalogs(lastUpdate);
            loadService.DownLoadCatalogs(catalogs);
            webService.ConfirmUpdateCatalogs(catalogs.Items.Select(x => x.Id));
            return catalogs.Items.Count();
        }

        private void SaveCatalogsToDatabase(Exception e, int countUpdated)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ValueCatalogs += countUpdated;
                SetCatalogsLabel();
                Updates();
            });
        }

        private void SetCatalogsLabel()
        {
            Application.Current.Dispatcher.Invoke(() => CatalogLabel = $"Товары (номенклатура) {ValueCatalogs} из {MaxCatalogs}");
        }

        private void UpdateInfoAfterLoadBrands(Exception e, int countUpdated)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ValueBrands += countUpdated;
                SetBrandsLabel();
                Updates();
            });
        }

        private int LoadBrands(DateTimeOffset lastUpdate)
        {
            SetBrandsLabel();
            Brands brands = webService.GetBrands(lastUpdate);
            return loadService.DownLoadBrands(brands);
        }

        private void RefreshLastUpdate()
        {
            if (DataService != null)
            {
                LastUpdate = $"Дата последнего обновления: {DataService.GetLastUpdate()}";
            }
        }

        private void InitProperties()
        {
            SetBrandsLabel();
            SetCatalogsLabel();
            SetDirectoriesLabel();
            SetProductDirectionLabel();
            SetPhotoLabel();
        }

        private void SetBrandsLabel()
        {
            Application.Current.Dispatcher.Invoke(() => BrandLabel = $"Бренды {ValueBrands} из {maxBrands}");
            
        }

        private bool CanDoSynchronize(object arg)
        {
            return canDoSynchronize; // TODO Проверить наличие Internet (ShortCut)
        }

        public void ApplySearchCriteria()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Add()
        {
            throw new NotImplementedException();
        }

        public void View()
        {
            throw new NotImplementedException();
        }

        public void Edit()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Init()
        {
            throw new NotImplementedException();
        }
    }
}
