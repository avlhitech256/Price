using System.Collections.ObjectModel;
using System.Windows;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Messenger;
using Domain.Data.Object;
using Domain.DomainContext;
using Order.Model;
using Order.Model.Implementation;

namespace Order.ViewModel
{
    public class DetailOrderViewModel : Notifier
    {
        #region Members

        private Visibility allowDelete;

        #endregion

        #region Constructors

        public DetailOrderViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Model = new DetailOrderModel(domainContext);
            SubscribeEvents();
            allowDelete = Visibility.Hidden;
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }

        public IMessenger Messenger => DomainContext?.Messenger;

        private IDetailOrderModel Model { get; }

        public Visibility AllowDelete
        {
            get
            {
                return allowDelete;
            }
            set
            {
                if (allowDelete != value)
                {
                    allowDelete = value;
                    OnPropertyChanged();
                }
            }
        }

        public OrderItem CurrentOrder
        {
            get
            {
                return Model?.CurrentOrder;
            }
            set
            {
                if (Model != null && Model.CurrentOrder != value)
                {
                    Model.CurrentOrder = value;
                    RiseAllowDelete();
                    OnPropertyChanged();
                } 
            }
        }

        public string Capture => Model?.Capture;

        public BasketItem SelectedItem
        {
            get { return Model?.SelectedItem; }
            set
            {
                if (Model != null && Model.SelectedItem != value)
                {
                    Model.SelectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<BasketItem> Entities => Model?.Entities;

        #endregion

        #region Methods

        private void SubscribeEvents()
        {
            Model.PropertyChanged += ModelOnPropertyChanged;
        }

        private void ModelOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) || 
                e.PropertyName == nameof(Entities) || 
                e.PropertyName == nameof(Capture))
            {
                OnPropertyChanged(e.PropertyName);
            }
        }

        public void Refresh()
        {
            Model.SelectEntities();
        }

        private void RiseAllowDelete()
        {
            AllowDelete = CurrentOrder != null && CurrentOrder.OrderStatus == OrderStatus.New ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion#
    }
}
