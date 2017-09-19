using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.Data.Object;
using Domain.DomainContext;
using Options.Service;
using Order.Model;
using Order.Model.Implementation;
using Photo.Service;
using Photo.Service.Implementation;

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

        private IOptionService OptionService => DomainContext?.OptionService;

        private IPhotoService PhotoService => DomainContext?.PhotoService;

        public bool ShowPhotoOnMouseDoubleClick => OptionService != null && OptionService.ShowPhotoOnMouseDoubleClick;

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
            Model.CountChanged += OnCountChanged;
            Model.PropertyChanged += ModelOnPropertyChanged;
        }

        private void OnCountChanged(object sender, DecimalValueChangedEventArgs e)
        {
            DecimalValueChangedEventArgs args =
                new DecimalValueChangedEventArgs(e.Id, e.OldValue, e.NewValue, MenuItemName.Orders);
            Messenger?.Send(CommandName.RefreshCount, args);
        }

        public void Refresh()
        {
            Model.SelectEntities();
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

        private void RiseAllowDelete()
        {
            AllowDelete = CurrentOrder != null && CurrentOrder.OrderStatus == OrderStatus.New ? Visibility.Visible : Visibility.Hidden;
        }

        public void ShowPicture()
        {
            if (SelectedItem != null)
            {
                List<byte[]> photos = SelectedItem?.Photos;
                PhotoService.ShowPhotos(photos);
            }
        }

        public void DeleteItem()
        {
            if (SelectedItem != null && SelectedItem.Entity.Order.OrderStatus == OrderStatus.New)
            {
                SelectedItem.Count = 0;
            }
        }

        #endregion

        #region Events

        public event EventHandler OrderChanged
        {
            add
            {
                if (Model != null)
                {
                    Model.OrderChanged += value;
                }
            }
            remove
            {
                if (Model != null)
                {
                    Model.OrderChanged -= value;
                }
            }
        }

        public event EventHandler DeletedOrder
        {
            add
            {
                if (Model != null)
                {
                    Model.DeletedOrder += value;
                }
            }
            remove
            {
                if (Model != null)
                {
                    Model.DeletedOrder -= value;
                }
            }
        }


        #endregion
    }
}
