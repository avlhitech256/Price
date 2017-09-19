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
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.ViewModel;
using Options.Service;
using Photo.Service;
using Photo.Service.Implementation;

namespace Basket.ViewModel
{
    public class BasketViewModel : Notifier, IControlViewModel
    {
        #region Constructors

        public BasketViewModel(IDomainContext domainContext)
        {
            DomainContext = domainContext;
            Model = new BasketModel(domainContext);
            OrderViewModel = new OrderViewModel(domainContext);
            HasChanges = false;
            InitCommands();
            SubscribeMessenger();
            SubscribeEvents();
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext { get; }

        public IMessenger Messenger => DomainContext?.Messenger;

        private IPhotoService PhotoService => DomainContext?.PhotoService;

        private IOptionService OptionService => DomainContext?.OptionService;

        public bool ShowPhotoOnMouseDoubleClick => OptionService != null && OptionService.ShowPhotoOnMouseDoubleClick;

        private BasketModel Model { get; }

        public OrderViewModel OrderViewModel { get; }

        public BasketItem SelectedItem
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

        public ObservableCollection<BasketItem> Entities => Model?.Entities;

        public bool ReadOnly { get; set; }
        public bool Enabled { get; set; }
        public bool IsEditControl { get; set; }
        public bool HasChanges { get; }

        public DelegateCommand AddCommand { get; private set; }

        public DelegateCommand ClearCommand { get; private set; }

        public DelegateCommand CreateOrderCommand { get; private set; }

        #endregion

        #region Methods
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

        private void InitCommands()
        {
            AddCommand = new DelegateCommand(DoAdd);
            ClearCommand = new DelegateCommand(DoClearBasket, CanDoClearBasket);
            CreateOrderCommand = new DelegateCommand(DoCreateOrder, CanDoCreateOrder);
        }

        private void DoAdd(object parametr)
        {
            Messenger?.Send(CommandName.SetFocusTopMenu, new SetMenuFocusEventArgs(MenuItemName.PriceList));
        }

        private void DoClearBasket(object parametr)
        {
            while (Entities != null && Entities.Any())
            {
                Entities.First().Count = 0;
            }
        }

        private bool CanDoClearBasket(object parametr)
        {
            return Entities != null && Entities.Any();
        }

        private void DoCreateOrder(object parametr)
        {
            OrderViewModel.CreateOrder(Entities, string.Empty);
            Refresh();
            OrderViewModel.Refresh();
            OrderViewModel.OnChangeOrder();
            Messenger?.Send(CommandName.RefreshPriceList, new EventArgs());
        }

        private void Refresh()
        {
            Model.SelectEntities();
            Messenger?.Send(CommandName.RefreshBasketCapture, new EventArgs());
            Messenger?.Send(CommandName.RefreshPriceList, new EventArgs());
        }

        private bool CanDoCreateOrder(object parametr)
        {
            return Entities != null && Entities.Any();
        }

        private void SubscribeEvents()
        {
            Model.CountChanged += OnCountChanged;
            Model.PropertyChanged += ModelOnPropertyChanged;
            OrderViewModel.RevertedOrder += OrderViewModelRevertedOrder;
            OrderViewModel.DeletedOrder += OrderViewModel_DeletedOrder;
        }

        private void OrderViewModel_DeletedOrder(object sender, EventArgs e)
        {
            Refresh();
            OrderViewModel.OnChangeOrder();
        }

        private void OrderViewModelRevertedOrder(object sender, EventArgs e)
        {
            DoRevertOrder(e);
        }

        private void DoRevertOrder(EventArgs args)
        {
            Refresh();
            OrderViewModel.OnChangeOrder();
        }

        private bool CanDoRevertOrder(EventArgs args)
        {
            return args != null;
        }

        private void ModelOnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedItem) || e.PropertyName == nameof(Entities))
            {
                OnPropertyChanged(e.PropertyName);
                AddCommand.RiseCanExecute(new object());
                ClearCommand.RiseCanExecute(new object());
                CreateOrderCommand.RiseCanExecute(new object());
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
                                                                       RefreshCount,
                                                                       CanRefreshCount);
                Messenger.Register<EventArgs>(CommandName.RevertOrder, DoRevertOrder, CanDoRevertOrder);
            }
        }

        private void RefreshCount(DecimalValueChangedEventArgs args)
        {
            if (args.NewValue == 0 || Entities.All(x => x.Entity.CatalogItem.Id != args.Id))
            {
                Model.SelectEntities();
            }
            else
            {
                Entities.FirstOrDefault(x => x.Entity.CatalogItem.Id == args.Id)?.Refresh();
            }
        }

        private bool CanRefreshCount(DecimalValueChangedEventArgs args)
        {
            return args.Entry != MenuItemName.Basket;
        }

        public void ShowPicture()
        {
            if (SelectedItem != null)
            {
                List<byte[]> photos = SelectedItem.Photos;
                PhotoService.ShowPhotos(photos);
            }
        }

        public void DeleteItem()
        {
            if (SelectedItem != null)
            {
                SelectedItem.Count = 0;
            }
        }

        #endregion
    }
}
