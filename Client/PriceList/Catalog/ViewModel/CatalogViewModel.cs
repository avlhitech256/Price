using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.ViewModel.Command;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.ViewModel;
using Photo.Service;

namespace Catalog.ViewModel
{
    public class CatalogViewModel : Notifier, IControlViewModel
    {
        #region Constructors

        public CatalogViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            HasChanges = false;
            ShowPhotoOnMouseDoubleClick = false;
            Model = new CatalogModel(domainContext);
            CatalogNavigateViewModel = new CatalogNavigateViewModel(Model);
            CatalogEdvanceSearchViewModel = new CatalogEdvanceSearchViewModel(domainContext, Model?.SearchCriteria);
            SubscribeEvents();
            SubscribeMessenger();
            InitCommands();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }

        public IMessenger Messenger => DomainContext?.Messenger;

        private IPhotoService PhotoService => DomainContext?.PhotoService;

        private CatalogModel Model { get; }

        public CatalogSearchCriteria SearchCriteria => Model?.SearchCriteria;

        public CatalogNavigateViewModel CatalogNavigateViewModel { get; }

        public CatalogEdvanceSearchViewModel CatalogEdvanceSearchViewModel { get; }

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

        public ObservableCollection<CatalogItem> Entities => Model?.Entities;

        public ObservableCollection<BrandItem> BrandItems => Model?.BrandItems;

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand ClearCommand { get; private set; }

        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; }
        public bool IsEditControl { get; set; }
        public bool HasChanges { get; }
        public void ApplySearchCriteria()
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public void Add()
        {
            throw new System.NotImplementedException();
        }

        public void View()
        {
            throw new System.NotImplementedException();
        }

        public void Edit()
        {
            throw new System.NotImplementedException();
        }

        public bool Save()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
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

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnChangedSelectedItem;
                Model.CountChanged += OnCountChanged;
                if (Model.SearchCriteria != null)
                {
                    Model.SearchCriteria.EnabledEdvanceSearchChanged += OnShowAdvanceSearchControl;
                    Model.SearchCriteria.EdvanceSearchWidthChanged += OnEdvanceSearchWidthChanged;
                }
            }

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
                                                                       RefreshCount, CanRefreshCount);
                Messenger.Register<EventArgs>(CommandName.RefreshPriceList, DoRefreshPriceList, CanDoRefreshPriceList);
            }
        }

        private void DoRefreshPriceList(EventArgs args)
        {
            Model.SelectEntities();
        }

        private bool CanDoRefreshPriceList(EventArgs args)
        {
            return Entities != null && Entities.Any();
        }

        private void RefreshCount(DecimalValueChangedEventArgs args)
        {
            if (Entities.All(x => x.Entity.Id != args.Id))
            {
                Model.SelectEntities();
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
                SearchCriteria.SearchCriteriaChanged += OnCanDoSearchChanged;
                SearchCriteria.SearchCriteriaCleared += OnCanDoClear;
            }
        }

        private void DoSearch(object parametr)
        {
            Model.SelectEntities();
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
            Model?.Clear();
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
            CatalogEdvanceSearchViewModel.Refresh();
            Messenger?.Send(CommandName.ShowAdvanceSearchControl, e);
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
            Model.SelectEntities();
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
