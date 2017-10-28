using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Data.Enum;
using Common.Event;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.DomainContext;
using PriceList.ViewModel.TopMenu;

namespace PriceList.View.TopMenu
{
    /// <summary>
    /// Логика взаимодействия для TopMenuControl.xaml
    /// </summary>
    public partial class TopMenuControl : UserControl
    {
        #region Members

        private readonly Dictionary<MenuItem, MenuItemName> menuItemMap;
        private IDomainContext context;

        #endregion

        #region Constructors

        public TopMenuControl()
        {
            InitializeComponent();
            FocusManager.SetIsFocusScope(MainGrid, true);
            menuItemMap =
                new Dictionary<MenuItem, MenuItemName>
                {
                    { PricelistMenuItem, MenuItemName.PriceList },
                    { BasketMenuItem, MenuItemName.Basket},
                    { OrdersMenuItem, MenuItemName.Orders},
                    { DocumentsMenuItem, MenuItemName.Documents },
                    { SyncMenuItem, MenuItemName.Sync },
                    { SettingsMenuItem, MenuItemName.Settings }
                };
        }

        #endregion

        #region Properties

        public IDomainContext DomainContext
        {
            get
            {
                return context;
            }

            set
            {
                if (context == null || context != value)
                {
                    context = value;
                    InitializeDataContext();
                }
            }

        }
        private IMessenger Messenger => DomainContext?.Messenger;
        public TopMenuViewModel ViewModel { get; private set; }

        #endregion

        #region Methods

        private void InitializeDataContext()
        {
            ViewModel = new TopMenuViewModel(DomainContext);
            DataContext = ViewModel;
            SubscribeMessenger();
        }

        private TopMenuMouseOverEventArgs CreateTopMenuMouseOverEventArgs(MenuItem menuItem, bool isMouseOver)
        {
            TopMenuMouseOverEventArgs args = null;

            if (menuItem != null && menuItemMap != null && menuItemMap.ContainsKey(menuItem))
            {
                args = new TopMenuMouseOverEventArgs(menuItemMap[menuItem], isMouseOver);
            }

            return args;
        }

        private TopMenuMouseOverEventArgs CreateTopMenuMouseOverEventArgs(object sender, bool isMouseOver)
        {
            TopMenuMouseOverEventArgs args = null;

            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                args = CreateTopMenuMouseOverEventArgs(menuItem, isMouseOver);
            }

            return args;
        }

        private void SetMouseOver(object sender, bool isMouseOver)
        {

            TopMenuMouseOverEventArgs args = CreateTopMenuMouseOverEventArgs(sender, isMouseOver);

            if (args != null)
            {
                ViewModel.MenuItemsStyle.SetMouseOverMenuItems(args);
            }

        }

        private MenuChangedEventArgs CreateMenuChangedEventArgs(MenuItem menuItem)
        {
            MenuChangedEventArgs args = null;

            if (menuItem != null && menuItemMap != null && menuItemMap.ContainsKey(menuItem))
            {
                args = new MenuChangedEventArgs(menuItemMap[menuItem]);
            }

            return args;
        }

        private MenuChangedEventArgs CreateMenuChangedEventArgs(object sender)
        {
            MenuChangedEventArgs args = null;
            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                args = CreateMenuChangedEventArgs(menuItem);
            }

            return args;
        }

        private void SetMouseUp(object sender)
        {
            MenuChangedEventArgs args = CreateMenuChangedEventArgs(sender);

            if (args != null)
            {
                Messenger?.Send(CommandName.SetEntryControl, args);
            }

        }

        public void Dispose()
        {
            menuItemMap.Clear();
        }

        private void MenuItem_OnMouseEnter(object sender, MouseEventArgs e)
        {
            SetMouseOver(sender, true);
        }

        private void MenuItem_OnMouseLeave(object sender, MouseEventArgs e)
        {
            SetMouseOver(sender, false);
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            SetMouseUp(sender);
        }

        private void SetFocusTopMenu(SetMenuFocusEventArgs args)
        {
            if (args != null)
            {
                MenuItemName menuItemName = args.MenuItemName;
                MenuItem menuItem = menuItemMap.FirstOrDefault(x => x.Value == menuItemName).Key;

                if (menuItem != null)
                {
                    MenuItemAutomationPeer peer = new MenuItemAutomationPeer(menuItem);
                    IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    invokeProv?.Invoke();
                }
            }
        }

        private bool CanSetFocusTopMenu(SetMenuFocusEventArgs args)
        {
            return true;
        }

        private void EnableMenu(EnableMenuEventArgs args)
        {
            Menu.IsEnabled = args.Enable;
        }

        private bool CanEnableMenu(EnableMenuEventArgs args)
        {
            return Menu != null && args != null;
        }

        private void SubscribeMessenger()
        {
            if (Messenger != null)
            {
                Messenger.Register<SetMenuFocusEventArgs>(CommandName.SetFocusTopMenu, 
                                                          SetFocusTopMenu,
                                                          CanSetFocusTopMenu);
                Messenger.Register<EnableMenuEventArgs>(CommandName.EnableMenu,
                                                        EnableMenu,
                                                        CanEnableMenu);
            }
        }

        #endregion
    }
}
