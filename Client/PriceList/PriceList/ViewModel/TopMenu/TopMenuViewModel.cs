using System;
using System.Windows;
using Async.Service;
using Common.Data.Enum;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Common.Service;
using DatabaseService.DataService;
using Domain.DomainContext;
using Domain.Service.Precision;
using Media.Color;

namespace PriceList.ViewModel.TopMenu
{
    public class TopMenuViewModel : Notifier
    {
        #region Members

        private MenuItemsStyle menuItemsStyle;
        private string basketCaption;
        private const string CurrencySuffix = " грн.";
        private bool databaseIsVerified;

        #endregion

        #region Constructors

        public TopMenuViewModel(IDomainContext domainContext)
        {
            databaseIsVerified = false;
            BasketCaption = "0.00" + CurrencySuffix;
            DomainContext = domainContext;
            menuItemsStyle = new MenuItemsStyle(DomainContext,
                                                ColorService.CreateBrush(0x2B, 0x3E, 0x80), 
                                                ColorService.CreateBrush(0x5B, 0x6E, 0xA0),
                                                ColorService.CreateBrush(0x8B, 0x9E, 0xF0),
                                                ColorService.CreateBrush(0xDB, 0xEE, 0xFF));
            SubscribeMessenger();
            RefreshBasketCapture();
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IAsyncOperationService AsyncOperationService => DomainContext?.AsyncOperationService;

        private IMessenger Messenger => DomainContext?.Messenger;

        private IColorService ColorService => DomainContext?.ColorService;

        private IPrecisionService PrecisionService => DomainContext?.PrecisionService;

        private IDataService DatabaseService => DomainContext?.DataService;

        public MenuItemsStyle MenuItemsStyle
        {
            get
            {
                return menuItemsStyle;
            }

            set
            {
                if (!menuItemsStyle.Equals(value))
                {
                    menuItemsStyle = value;
                    OnPropertyChanged();
                }

            }

        }

        public string BasketCaption
        {
            get
            {
                return basketCaption;
            }
            set
            {
                if (basketCaption != value)
                {
                    basketCaption = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Methods

        private void SubscribeMessenger()
        {
            Messenger?.Register<MenuChangedEventArgs>(CommandName.SelectLeftMenu, SelectTopMenu, CanSelectTopMenu);
            Messenger?.Register<EventArgs>(CommandName.RefreshBasketCapture, RefreshBasketCapture, CanRefreshBasketCapture);
        }

        private void SelectTopMenu(MenuChangedEventArgs args)
        {
            MenuItemsStyle.SetMouseUpMenuItems(args);
        }

        private bool CanSelectTopMenu(MenuChangedEventArgs args)
        {
            return args != null;
        }

        private void RefreshBasketCapture(EventArgs args = null)
        {
            //decimal sum = DatabaseService.GetSumBasket();
            //BasketCaption = PrecisionService?.Convert(sum) + CurrencySuffix;
            AsyncOperationService
                .PerformAsyncOperation(databaseIsVerified ? AsyncOperationType.GetSumBasket : AsyncOperationType.CheckDatabase,
                                       GetSumBasket, !DomainContext?.IsLoading ?? false, ActionAfterGetSumBasket);

        }

        private void ActionAfterGetSumBasket(Exception e, object sum)
        {
            if (e == null)
            {
                Application.Current.Dispatcher.Invoke(() => BasketCaption = (string) sum);
            }
            else
            {
                //throw e;
                DomainContext?.CloseMainWindow?.Invoke();
            }
        }

        private object GetSumBasket(bool allowToGet)
        {
            string result = string.Empty;
            databaseIsVerified = true;

            if (allowToGet)
            {
                decimal sum = DatabaseService.GetSumBasket();
                result = PrecisionService?.Convert(sum) + CurrencySuffix;
            }

            return result;
        }

        private bool CanRefreshBasketCapture(EventArgs args)
        {
            return !DomainContext.IsLoading;
        }

        #endregion
    }
}
