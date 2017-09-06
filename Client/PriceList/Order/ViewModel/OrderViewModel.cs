using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Common.Data.Notifier;
using Common.Messenger;
using Common.ViewModel.Command;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.ViewModel;
using Order.Model;
using Order.Model.Implementation;
using Order.SearchCriteria;

namespace Order.ViewModel
{
    public class OrderViewModel : Notifier, IControlViewModel
    {
        private IMessenger _messenger;

        #region Members
        #endregion

        #region Constructors

        public OrderViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Model = new OrderModel(domainContext);
            SubscribeEvents();
            InitCommands();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }

        public IMessenger Messenger => DomainContext?.Messenger;

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

        private IOrderModel Model { get; }

        public OrderItem SelectedItem
        {
            get
            {
                return Model?.SelectedItem;
            }
            set
            {
                if (Model != null && Model.SelectedItem != value)
                {
                    Model.SelectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OrderItem> Entities => Model?.Entities;

        public DelegateCommand SearchCommand { get; private set; }

        public DelegateCommand ClearCommand { get; private set; }

        public OrderSearchCriteria SearchCriteria => Model?.SearchCriteria;

        #endregion

        #region Methods

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

        private void SubscribeEvents()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnChangedSelectedItem;
            }
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

        #endregion
    }
}
