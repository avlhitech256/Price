using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Basket.Model;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Messenger;
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
                    UnsubscribeSelectedItemEvent();
                    Model.SelectedItem = value;
                    SubscribeSelectedItemEvent();
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

        private void SubscribeSelectedItemEvent()
        {
            if (Model != null && Model.SelectedItem != null)
            {
                Model.SelectedItem.PropertyChanged += SelectedItem_PropertyChanged;
            }
        }

        private void UnsubscribeSelectedItemEvent()
        {
            if (Model != null && Model.SelectedItem != null)
            {
                Model.SelectedItem.PropertyChanged -= SelectedItem_PropertyChanged;
            }
        }

        private void SelectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Model.SelectedItem.Comment))
            {
                DataService.DataBaseContext.SaveChanges();
            }
        }

        private void InitCommands()
        {
            SendCommand = new DelegateCommand(DoSend, CanDoSend);
            RevertCommand = new DelegateCommand(DoRevertBasket, CanDoRevertBasket);
        }

        private void DoSend(object parametr)
        {
            Model.SendOut();
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
            SubscribeSelectedItemEvent();
        }

        private void ModelOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) || e.PropertyName == nameof(Entities))
            {
                OnPropertyChanged(e.PropertyName);
                SendCommand.RiseCanExecute(new object());
                RevertCommand.RiseCanExecute(new object());
            }
        }

        public void CreateOrder(IEnumerable<BasketItem> basketItems, string comment)
        {
            Model.CreateOrder(basketItems, comment);
        }

        public void Refresh()
        {
            Model.SelectEntities();
        }

        private void OnRevertOrder()
        {
            RevertedOrder?.Invoke(this, new EventArgs());
        }

        private void OnDeleteOrder()
        {
            DeletedOrder?.Invoke(this, new EventArgs());
        }

        public void DeleteOrder()
        {
            Model.DeleteOrder();
            OnDeleteOrder();
        }

        #endregion

        #region Events

        public event EventHandler RevertedOrder;
        public event EventHandler DeletedOrder;

        #endregion
    }
}
