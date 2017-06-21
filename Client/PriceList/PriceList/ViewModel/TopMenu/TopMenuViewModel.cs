using Common.Data.Notifier;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.DomainContext;
using Domain.Event;
using Media.Color;

namespace PriceList.ViewModel.TopMenu
{
    public class TopMenuViewModel : Notifier
    {
        #region Members

        private MenuItemsStyle menuItemsStyle;

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
        }

        #endregion

        #region Properties

        private IDomainContext DomainContext { get; }

        private IMessenger Messenger => DomainContext?.Messenger;

        private IColorService ColorService => DomainContext?.ColorService;
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

        #endregion

        #region Methods

        private void SubscribeMessenger()
        {
            Messenger?.Register<MenuChangedEventArgs>(CommandName.SelectLeftMenu, SelectTopMenu, CanSelectTopMenu);
        }

        private void SelectTopMenu(MenuChangedEventArgs args)
        {
            MenuItemsStyle.SetMouseUpMenuItems(args);
        }

        private bool CanSelectTopMenu(MenuChangedEventArgs args)
        {
            return args != null;
        }

        #endregion
    }
}
