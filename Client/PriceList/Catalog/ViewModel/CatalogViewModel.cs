using System.Collections.ObjectModel;
using System.ComponentModel;
using Catalog.Model;
using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.Event;
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
            Model = new CatalogModel(domainContext);
            SubscribeEvents();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }
        public IMessenger Messenger => DomainContext?.Messenger;

        private CatalogModel Model { get; }

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
    }
}
