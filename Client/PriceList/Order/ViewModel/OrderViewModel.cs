using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
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
        #region Members

        private bool isWaiting;
        private bool isLoading;

        #endregion

        #region Constructors

        public OrderViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Model = new OrderModel(domainContext);
            DetailViewModel = new DetailOrderViewModel(domainContext);
            DetailViewModel.CurrentOrder = SelectedItem;
            SubscribeEvents();
            SubscribeMessenger();
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

        public DetailOrderViewModel DetailViewModel { get; }

        public Action<string> ShowWaitScreen { get; set; }

        public Action HideWaitScreen { get; set; }

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

        public DelegateCommand SendCommand { get; private set; }

        public DelegateCommand RevertCommand { get; private set; }

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
            SendCommand = new DelegateCommand(DoSend, CanDoSend);
            RevertCommand = new DelegateCommand(DoRevertBasket, CanDoRevertBasket);
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

        private void DoSend(object parametr)
        {
            Model.SendOut();
            OnChangeOrder();
        }

        private bool CanDoSend(object parametr)
        {
            return SelectedItem != null && SelectedItem.OrderStatus == OrderStatus.New;
        }

        private void DoRevertBasket(object parametr)
        {
            Model.Revert();
            Messenger?.Send(CommandName.RevertOrder, new EventArgs());
            OnChangeOrder();
        }

        private bool CanDoRevertBasket(object parametr)
        {
            return SelectedItem != null && SelectedItem.OrderStatus == OrderStatus.New && SelectedItem.BasketItems.Any();
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

            if (DetailViewModel != null)
            {
                DetailViewModel.OrderChanged += DetailViewModel_OrderChanged;
                DetailViewModel.DeletedOrder += DetailViewModel_DeletedOrder;
            }
        }

        private void SubscribeMessenger()
        {
            Messenger?.MultiRegister<DecimalValueChangedEventArgs>(CommandName.RefreshOrder, DoExternalRefresh, CanDoExternalRefresh);
        }

        private void DoExternalRefresh(DecimalValueChangedEventArgs args)
        {
            Model?.SelectEntities();
        }

        private bool CanDoExternalRefresh(DecimalValueChangedEventArgs args)
        {
            return args != null && args.Entry != MenuItemName.Orders;
        }

        private void DetailViewModel_DeletedOrder(object sender, EventArgs e)
        {
            Refresh();
        }

        private void DetailViewModel_OrderChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            OnChangeOrder();
            Model?.SelectEntities();
        }

        private void OnChangeOrder()
        {
            if (SelectedItem != null)
            {
                var args = new DecimalValueChangedEventArgs(SelectedItem.Id, 0, 0, MenuItemName.Orders);
                Messenger?.Send(CommandName.RefreshOrder, args);
            }
        }

        private void OnChangedSelectedItem(object sender, PropertyChangedEventArgs e)
        {
            if (Model != null)
            {
                if (e.PropertyName == nameof(Model.SelectedItem) || 
                    e.PropertyName == nameof(Model.Entities))
                {
                    DetailViewModel.CurrentOrder = SelectedItem;
                    OnPropertyChanged(e.PropertyName);
                    SendCommand.RiseCanExecute(new object());
                    RevertCommand.RiseCanExecute(new object());
                }
            }
        }

        #endregion
    }
}
