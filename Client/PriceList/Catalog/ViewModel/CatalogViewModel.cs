using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.Service;
using Common.ViewModel.Command;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.ViewModel;
using Photo.Service;

namespace Catalog.ViewModel
{
    public class CatalogViewModel : Notifier, IControlViewModel
    {
        #region Members

        private bool isInited;

        #endregion

        #region Constructors

        public CatalogViewModel(IDomainContext domainContext)
        {
            isInited = false;
            SplitterPosition = 0;
            DomainContext = domainContext;
            HasChanges = false;
            ShowPhotoOnMouseDoubleClick = false;
            RefreshView = delegate { };
            Model = new CatalogModel(domainContext);
            CatalogNavigateViewModel = new CatalogNavigateViewModel(Model);
            CatalogNavigateViewModel.LoadData = LoadData;
            CatalogDirectoryViewModel = new CatalogDirectoryViewModel(domainContext, Model?.SearchCriteria);
            CatalogBrandViewModel = new CatalogBrandViewModel(domainContext, Model?.SearchCriteria);
            SubscribeEvents();
            SubscribeMessenger();
            InitCommands();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }

        public IAsyncOperationService AsyncOperationService => DomainContext?.AsyncOperationService;

        public IMessenger Messenger => DomainContext?.Messenger;

        private IPhotoService PhotoService => DomainContext?.PhotoService;

        private CatalogModel Model { get; }

        public CatalogSearchCriteria SearchCriteria => Model?.SearchCriteria;

        public CatalogNavigateViewModel CatalogNavigateViewModel { get; }

        public CatalogDirectoryViewModel CatalogDirectoryViewModel { get; }

        public CatalogBrandViewModel CatalogBrandViewModel { get; }

        public Action RefreshView { get; set; }
        
        public bool IsWaiting => DomainContext?.IsWaiting ?? false;
        public bool IsLoading => DomainContext?.IsLoading ?? false;

        public CatalogItem SelectedItem
        {
            get
            {
                return Model?.SelectedItem;
            }

            set
            {
                if (Model != null)
                {
                    Model.SelectedItem = value;
                }

            }

        }

        public List<CatalogItem> Entities => Model?.Entities;

        public List<BrandItem> BrandItems => Model?.BrandItems;

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand ClearCommand { get; private set; }

        public bool ReadOnly { get; set; }

        public bool Enabled { get; set; }

        public bool IsEditControl { get; set; }

        public bool HasChanges { get; }

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

        public bool ShowPhotoOnMouseDoubleClick { get; set; }

        public int StartRowIndex
        {
            get
            {
                return Model.StartRowIndex;
            }
            set
            {
                if (Model.StartRowIndex != value)
                {
                    Model.StartRowIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaximumRows
        {
            get
            {
                return Model.MaximumRows;
            }
            set
            {
                if (Model.MaximumRows != value)
                {
                    Model.MaximumRows = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Count => Model.Count;

        public double SplitterPosition { get; set; }

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnChangedSelectedItem;
                Model.CountChanged += OnCountChanged;

                if (SearchCriteria != null)
                {
                    SearchCriteria.EnabledEdvanceSearchChanged += OnShowAdvanceSearchControl;
                    SearchCriteria.EdvanceSearchWidthChanged += OnEdvanceSearchWidthChanged;
                    SearchCriteria.PropertyChanged += SearchCriteria_PropertyChanged;
                    SearchCriteria.EnabledEdvanceSearchChanged += SearchCriteria_EnabledEdvanceSearchChanged;
                    SearchCriteria.DirectoryItemsChanged += SearchCriteria_DirectoryItemsChanged;
                    SearchCriteria.BrandItemsChanged += SearchCriteria_BrandItemsChanged;
                }
            }

        }

        public void Init()
        {
            if (!isInited)
            {
                isInited = true;
                LoadData(LoadingType.ChangedSearchCriteria);
            }
        }

        private void LoadData(LoadingType loadingType)
        {
            if (!IsLoading)
            {
                switch (loadingType)
                {
                    case LoadingType.ChangedDataInCurrentPage:
                    case LoadingType.ChangedSelectedPage:
                    case LoadingType.ChangedSearchCriteria:
                    case LoadingType.ChangedBrandItems:
                        AsyncOperationService
                            .PerformAsyncOperation(AsyncOperationType.LoadCatalog, LoadPricelist, !IsLoading,
                                ActionAfterLoadDaata);
                        break;
                    case LoadingType.ChangedDirectoryItems:
                        AsyncOperationService
                            .PerformAsyncOperation(AsyncOperationType.LoadBrands, LoadBrands, !IsLoading,
                                ActionAfterLoadDaata);
                        break;
                    case LoadingType.СhangedAdvancedSearch:
                        AsyncOperationService
                            .PerformAsyncOperation(AsyncOperationType.LoadDirectories, LoadDirectories, !IsLoading,
                                ActionAfterLoadDaata);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private object LoadPricelist(bool needToUpdate)
        {
            if (needToUpdate)
            {
                Model.SelectEntities();
            }

            return needToUpdate;
        }

        private object LoadBrands(bool needToUpdate)
        {
            if (needToUpdate)
            {
                CatalogBrandViewModel.Refresh();
                DomainContext?.SetWaitMessage(AsyncOperationType.LoadCatalog);
                LoadPricelist(true);
            }

            return needToUpdate;
        }

        private object LoadDirectories(bool needToUpdate)
        {
            if (needToUpdate)
            {
                CatalogDirectoryViewModel.Refresh();
                DomainContext?.SetWaitMessage(AsyncOperationType.LoadBrands);

                CatalogBrandViewModel.Refresh();
                DomainContext?.SetWaitMessage(AsyncOperationType.LoadCatalog);
                LoadPricelist(true);
            }

            return needToUpdate;
        }

        private void ActionAfterLoadDaata(Exception e, object needToUpdate)
        {
            if ((bool) needToUpdate)
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        SearchCriteria?.SearchComplited();
                        SearchCommand?.RiseCanExecute();
                        ClearCommand?.RiseCanExecute();
                        CatalogNavigateViewModel.FirstCommand.RiseCanExecute();
                        CatalogNavigateViewModel.PreviousCommand.RiseCanExecute();
                        CatalogNavigateViewModel.NextCommand.RiseCanExecute();
                        CatalogNavigateViewModel.LastCommand.RiseCanExecute();
                        OnPropertyChanged(nameof(Entities));
                        OnPropertyChanged(nameof(SelectedItem));
                        RefreshView();
                    });
            }
        }

        private void SearchCriteria_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SearchCriteria != null && 
                (e.PropertyName == nameof(SearchCriteria.Vaz) ||
                 e.PropertyName == nameof(SearchCriteria.Gaz) ||
                 e.PropertyName == nameof(SearchCriteria.Zaz) ||
                 e.PropertyName == nameof(SearchCriteria.Chemistry) ||
                 e.PropertyName == nameof(SearchCriteria.Battery) ||
                 e.PropertyName == nameof(SearchCriteria.Gas) ||
                 e.PropertyName == nameof(SearchCriteria.Instrument) ||
                 e.PropertyName == nameof(SearchCriteria.IsNew) ||
                 e.PropertyName == nameof(SearchCriteria.PriceIsUp) ||
                 e.PropertyName == nameof(SearchCriteria.PriceIsDown)))
            {
                LoadData(SearchCriteria.EnabledEdvanceSearch
                    ? LoadingType.СhangedAdvancedSearch
                    : LoadingType.ChangedSearchCriteria);
            }
            else if (SearchCriteria != null && !SearchCriteria.EnabledEdvanceSearch &&
                     e.PropertyName == nameof(SearchCriteria.BrandId))
            {
                LoadData(LoadingType.ChangedSearchCriteria);
            }
        }

        private void SearchCriteria_DirectoryItemsChanged(object sender, EventArgs e)
        {
            LoadData(LoadingType.ChangedDirectoryItems);
        }

        private void SearchCriteria_BrandItemsChanged(object sender, EventArgs e)
        {
            LoadData(LoadingType.ChangedSearchCriteria);
        }

        private void SearchCriteria_EnabledEdvanceSearchChanged(object sender, DoubleAnimationEventArgs e)
        {
            LoadData(SearchCriteria.EnabledEdvanceSearch
                ? LoadingType.СhangedAdvancedSearch
                : LoadingType.ChangedSearchCriteria);
        }

        private void OnCountChanged(object sender, DecimalValueChangedEventArgs e)
        {
            if (Messenger != null)
            {
                Messenger.Send(CommandName.RefreshBasketCapture, new EventArgs());
                Messenger.Send(CommandName.RefreshCount, e);
            }
        }

        private void SubscribeMessenger()
        {
            if (Messenger != null)
            {
                Messenger?.MultiRegister<DecimalValueChangedEventArgs>(CommandName.RefreshCount,
                                                                       RefreshCount, 
                                                                       CanRefreshCount);
                Messenger.Register<EventArgs>(CommandName.RefreshPriceList, 
                                              DoRefreshPriceList, 
                                              CanDoRefreshPriceList);
            }
        }

        private void DoRefreshPriceList(EventArgs args)
        {
            LoadData(LoadingType.ChangedDataInCurrentPage);
        }

        private bool CanDoRefreshPriceList(EventArgs args)
        {
            return Entities != null && Entities.Any();
        }

        private void RefreshCount(DecimalValueChangedEventArgs args)
        {
            if (Entities.All(x => x.Entity.Id != args.Id))
            {
                LoadData(LoadingType.ChangedDataInCurrentPage);
            }
            else
            {
                Entities.FirstOrDefault(x => x.Entity.Id == args.Id)?.Refresh();
            }
        }

        private bool CanRefreshCount(DecimalValueChangedEventArgs args)
        {
            return args.Entry != MenuItemName.PriceList;
        }



        private void InitCommands()
        {
            CreateCommand();
            SubscribeCommand();
        }

        private void CreateCommand()
        {
            SearchCommand = new DelegateCommand(DoSearch, CanDoSearch);
            ClearCommand = new DelegateCommand(DoClear, CanDoClear);
        }

        private void SubscribeCommand()
        {
            if (SearchCriteria != null)
            {
                SearchCriteria.SearchCriteriaChanged += (sender, e) => 
                    Application.Current.Dispatcher.Invoke(() => OnCanDoSearchChanged(sender, e));
                SearchCriteria.SearchCriteriaCleared += (sender, e) => 
                    Application.Current.Dispatcher.Invoke(() => OnCanDoClear(sender, e));
            }
        }

        private void DoSearch(object parametr)
        {
            LoadData(SearchCriteria.EnabledEdvanceSearch
                ? LoadingType.СhangedAdvancedSearch
                : LoadingType.ChangedSearchCriteria);
        }

        private bool CanDoSearch(object parametr)
        {
            return SearchCriteria.IsModified;
        }

        private void OnCanDoSearchChanged(object sender, EventArgs e)
        {
            SearchCommand?.RiseCanExecute();
        }

        private void DoClear(object parametr)
        {
            SearchCriteria.Clear();
            LoadData(LoadingType.ChangedSearchCriteria);
        }

        private bool CanDoClear(object parametr)
        {
            return !SearchCriteria?.IsEmpty ?? false;
        }
        private void OnCanDoClear(object sender, EventArgs e)
        {
            ClearCommand?.RiseCanExecute();
        }

        private void OnChangedSelectedItem(object sender, PropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                switch (e.PropertyName)
                {
                    case nameof(Model.SelectedItem):
                        SendSetImageMessage();
                        OnPropertyChanged(nameof(SelectedItem));
                        break;
                    case nameof(Model.Entities):
                        OnPropertyChanged(nameof(Entities));
                        break;
                }

            }

        }

        private void OnShowAdvanceSearchControl(object sender, DoubleAnimationEventArgs e)
        {
            Messenger?.Send(CommandName.ShowAdvanceSearchControl, e);

            if (SearchCriteria != null && !SearchCriteria.EnabledEdvanceSearch)
            {
                LoadData(LoadingType.ChangedSearchCriteria);
            }
        }

        private void OnEdvanceSearchWidthChanged(object sender, MinWidthEventArgs e)
        {
            Messenger?.Send(CommandName.SetMinWidth, e);
        }

        private void SendSetImageMessage()
        {
            Messenger?.Send(CommandName.SetImage, SelectedItem);
        }

        public void ShowPicture()
        {
            if (SelectedItem != null)
            {
                List<byte[]> photos = SelectedItem.Photos;
                PhotoService.ShowPhotos(photos);
            }
        }

        public void SelectNexPageData()
        {
            StartRowIndex += MaximumRows;
            LoadData(LoadingType.ChangedSelectedPage);
        }

        #endregion

        #region Events

        public event EventHandler SearchCriteriaChanged
        {
            add { SearchCriteria.SearchCriteriaChanged += value; }
            remove { SearchCriteria.SearchCriteriaChanged -= value; }
        }

        #endregion
    }
}
