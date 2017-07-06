using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.Data.Enum;
using Domain.DomainContext;
using Domain.Event;
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
            menuItemMap =
                new Dictionary<MenuItem, MenuItemName>
                {
                    { PricelistMenuItem, MenuItemName.PriceList },
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
        }

        private TopMenuMouseOverEventArgs CreateLeftMenuMouseOverEventArgs(MenuItem menuItem, bool isMouseOver)
        {
            TopMenuMouseOverEventArgs args = null;

            if (menuItem != null && menuItemMap != null && menuItemMap.ContainsKey(menuItem))
            {
                args = new TopMenuMouseOverEventArgs(menuItemMap[menuItem], isMouseOver);
            }

            return args;
        }

        private TopMenuMouseOverEventArgs CreateLeftMenuMouseOverEventArgs(object sender, bool isMouseOver)
        {
            TopMenuMouseOverEventArgs args = null;

            MenuItem menuItem = sender as MenuItem;

            if (menuItem != null)
            {
                args = CreateLeftMenuMouseOverEventArgs(menuItem, isMouseOver);
            }

            return args;
        }

        private void SetMouseOver(object sender, bool isMouseOver)
        {

            TopMenuMouseOverEventArgs args = CreateLeftMenuMouseOverEventArgs(sender, isMouseOver);

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

        #endregion

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
    }
}
