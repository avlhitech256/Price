using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Basket.Model;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.ViewModel.Command;
using DatabaseService.DataService;
using Domain.Data.Object;
using Domain.DomainContext;

namespace Basket.ViewModel
{
    public class OrderViewModel : Notifier
    {
        #region Constructors

        public OrderViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Model = new OrderModel(domainContext);
            HasChanges = false;
            InitCommands();
            SubscribeEvents();
            SubscribeMessenger();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }

        public IMessenger Messenger => DomainContext?.Messenger;

        public IDataService DataService => DomainContext?.DataService;

        private OrderModel Model { get; }

        public OrderItem SelectedItem
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
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<OrderItem> Entities => Model?.Entities;

        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; }
        public bool IsEditControl { get; set; }
        public bool HasChanges { get; }

        public DelegateCommand SendCommand { get; private set; }

        public DelegateCommand RevertCommand { get; private set; }

        #endregion

        #region Methods

        private void InitCommands()
        {
            SendCommand = new DelegateCommand(DoSend, CanDoSend);
            RevertCommand = new DelegateCommand(DoRevertBasket, CanDoRevertBasket);
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
            OnRevertOrder();
        }

        private bool CanDoRevertBasket(object parametr)
        {
            return SelectedItem != null && SelectedItem.OrderStatus == OrderStatus.New && SelectedItem.BasketItems.Any();
        }

        private void SubscribeEvents()
        {
            Model.PropertyChanged += ModelOnPropertyChanged;
        }

        private void ModelOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) || e.PropertyName == nameof(Entities))
            {
                OnPropertyChanged(e.PropertyName);
                SendCommand.RiseCanExecute(new object());
                RevertCommand.RiseCanExecute(new object());

                if (e.PropertyName == nameof(Entities))
                {
                    OnChangeOrder();
                }
            }
        }

        public void CreateOrder(IEnumerable<BasketItem> basketItems, string comment)
        {
            Model.CreateOrder(basketItems, comment);
            OnChangeOrder();
        }

        public void Refresh()
        {
            Model.SelectEntities();
        }

        private void OnRevertOrder()
        {
            RevertedOrder?.Invoke(this, new EventArgs());
            OnChangeOrder();
        }

        private void OnDeleteOrder()
        {
            DeletedOrder?.Invoke(this, new EventArgs());
            OnChangeOrder();
        }

        public void DeleteOrder()
        {
            Model.DeleteOrder();
            OnDeleteOrder();
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
            return args != null && args.Entry != MenuItemName.Basket;
        }

        public  void OnChangeOrder()
        {
            if (SelectedItem != null)
            {
                var args = new DecimalValueChangedEventArgs(SelectedItem.Id, 0, 0, MenuItemName.Basket);
                Messenger?.Send(CommandName.RefreshOrder, args);
            }
        }


        #endregion

        #region Events

        public event EventHandler RevertedOrder;
        public event EventHandler DeletedOrder;

        #endregion
    }
}
