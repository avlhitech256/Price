using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Catalog.Model;
using Catalog.SearchCriteria;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.ViewModel.Command;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.ViewModel;

namespace Catalog.ViewModel
{
    public class CatalogViewModel : Notifier, IControlViewModel
    {
        #region Members

        private bool vaz;
        private bool gaz;
        private bool zaz;
        private bool chemistry;
        private bool battery;
        private bool gas;
        private bool instrument;
        private int edvanceSearchWidth;
        private bool enabledEdvanceSearch;

        #endregion

        #region Constructors

        public CatalogViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            HasChanges = false;
            Vaz = false;
            Gaz = false;
            Zaz = false;
            Chemistry = false;
            Battery = false;
            Gas = false;
            Instrument = false;
            edvanceSearchWidth = 0;
            enabledEdvanceSearch = false;
            ShowPhotoOnMouseDoubleClick = false;
            Model = new CatalogModel(domainContext);
            SubscribeEvents();
            InitCommands();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }
        public IMessenger Messenger => DomainContext?.Messenger;

        private CatalogModel Model { get; }

        public CatalogSearchCriteria SearchCriteria => Model.SearchCriteria;

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

        public bool Vaz
        {
            get
            {
                return vaz;
            }
            set
            {
                if (vaz != value)
                {
                    vaz = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Gaz
        {
            get
            {
                return gaz;
            }
            set
            {
                if (gaz != value)
                {
                    gaz = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Zaz
        {
            get
            {
                return zaz;
            }
            set
            {
                if (zaz != value)
                {
                    zaz = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Chemistry
        {
            get
            {
                return chemistry;
            }
            set
            {
                if (chemistry != value)
                {
                    chemistry = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Battery
        {
            get
            {
                return battery;
            }
            set
            {
                if (battery != value)
                {
                    battery = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Gas
        {
            get
            {
                return gas;
            }
            set
            {
                if (gas != value)
                {
                    gas = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool Instrument
        {
            get
            {
                return instrument;
            }
            set
            {
                if (instrument != value)
                {
                    instrument = value;
                    OnPropertyChanged();
                    CalculateEdvanceSearchWidth();
                }
            }
        }

        public bool EnabledEdvanceSearch
        {
            get
            {
                return enabledEdvanceSearch;
            }
            set
            {
                if (enabledEdvanceSearch != value)
                {
                    enabledEdvanceSearch = value;
                    OnPropertyChanged();
                    var args = value ? new DoubleAnimationEventArgs(0, 150) : new DoubleAnimationEventArgs(150, 0); 
                    Messenger?.Send(CommandName.ShowAdvanceSearchControl, args);
                }
            }
        }

        public int EdvanceSearchWidth
        {
            get
            {
                return edvanceSearchWidth;
            }
            set
            {
                if (edvanceSearchWidth != value)
                {
                    edvanceSearchWidth = value;
                    OnPropertyChanged();
                    int minWidth = 1070 + EdvanceSearchWidth;
                    Messenger?.Send(CommandName.SetMinWidth, new MinWidthEventArgs(minWidth));
                }
            }
        }

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnChangedSelectedItem;
            }

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
                SearchCriteria.SearchCriteriaChanged += OnCanDoSearchCanged;
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

        private void OnCanDoSearchCanged(object sender, EventArgs e)
        {
            SearchCommand?.RiseCanExecute();
        }

        private void DoClear(object parametr)
        {
            SearchCriteria?.Clear();
        }

        private bool CanDoClear(object parametr)
        {
            return !SearchCriteria?.IsEmpty ?? false;
        }
        private void OnCanDoClear(object sender, EventArgs e)
        {
            ClearCommand?.RiseCanExecute();
        }

        private void CalculateEdvanceSearchWidth()
        {
            EnabledEdvanceSearch = Vaz || Gaz || Zaz || Chemistry || Battery || Gas || Instrument;
            EdvanceSearchWidth = EnabledEdvanceSearch ? 150 : 0;
        }

        private void OnChangedSelectedItem(object sender, PropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                switch (e.PropertyName)
                {
                    case nameof(Model.SelectedItem):
                        OnPropertyChanged(nameof(SelectedItem));
                        break;
                    case nameof(Model.Entities):
                        OnPropertyChanged(nameof(Entities));
                        break;
                }

            }

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
