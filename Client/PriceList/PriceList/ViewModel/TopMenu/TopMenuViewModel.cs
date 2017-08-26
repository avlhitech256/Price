using System;
using Common.Data.Notifier;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
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

        #endregion

        #region Constructors

        public TopMenuViewModel(IDomainContext domainContext)
        {
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
            decimal sum = DatabaseService.GetSumBasket();
            BasketCaption = PrecisionService?.Convert(sum) + CurrencySuffix;
        }

        private bool CanRefreshBasketCapture(EventArgs args)
        {
            return true;
        }

        #endregion
    }
}
