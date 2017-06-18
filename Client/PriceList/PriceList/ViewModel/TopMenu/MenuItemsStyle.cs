using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common.Data.Notifier;
using Domain.Data.Enum;
using Domain.Event;

namespace PriceList.ViewModel.TopMenu
{
    public class MenuItemsStyle : Notifier
    {
        #region Members

        private string notSelectedAndMouseIsNotOverBackgroundColor;
        private string notSelectedAndMouseIsOverBackgroundColor;
        private string selectedAndMouseIsNotOverBackgroundColor;
        private string selectedAndMouseIsOverBackgroundColor;

        private List<MenuItemStyle> menuItems;
        private MenuItemStyle priceListMenuItemStyle;
        private MenuItemStyle ordersMenuItemStyle;
        private MenuItemStyle documentsMenuItemStyle;
        private MenuItemStyle syncMenuItemStyle;
        private MenuItemStyle settingsMenuItemStyle;

        #endregion

        #region Constructors

        public MenuItemsStyle() : this(null, null, null, null)
        {
        }

        public MenuItemsStyle(string notSelectedAndMouseIsNotOverColorString,
                             string notSelectedAndMouseIsOverColorString,
                             string selectedAndMouseIsNotOverColorString,
                             string selectedAndMouseIsOverColorString)
        {
            notSelectedAndMouseIsNotOverBackgroundColor = notSelectedAndMouseIsNotOverColorString;
            notSelectedAndMouseIsOverBackgroundColor = notSelectedAndMouseIsOverColorString;
            selectedAndMouseIsNotOverBackgroundColor = selectedAndMouseIsNotOverColorString;
            selectedAndMouseIsOverBackgroundColor = selectedAndMouseIsOverColorString;

            priceListMenuItemStyle = new MenuItemStyle(MenuItemName.PriceList,
                                                       notSelectedAndMouseIsNotOverColorString,
                                                       notSelectedAndMouseIsOverColorString,
                                                       selectedAndMouseIsNotOverColorString,
                                                       selectedAndMouseIsOverColorString);
            ordersMenuItemStyle = new MenuItemStyle(MenuItemName.Orders,
                                                    notSelectedAndMouseIsNotOverColorString,
                                                    notSelectedAndMouseIsOverColorString,
                                                    selectedAndMouseIsNotOverColorString,
                                                    selectedAndMouseIsOverColorString);
            documentsMenuItemStyle = new MenuItemStyle(MenuItemName.Documents,
                                                       notSelectedAndMouseIsNotOverColorString,
                                                       notSelectedAndMouseIsOverColorString,
                                                       selectedAndMouseIsNotOverColorString,
                                                       selectedAndMouseIsOverColorString);
            syncMenuItemStyle = new MenuItemStyle(MenuItemName.Sync,
                                                  notSelectedAndMouseIsNotOverColorString,
                                                  notSelectedAndMouseIsOverColorString,
                                                  selectedAndMouseIsNotOverColorString,
                                                  selectedAndMouseIsOverColorString);
            settingsMenuItemStyle = new MenuItemStyle(MenuItemName.Settings,
                                                            notSelectedAndMouseIsNotOverColorString,
                                                            notSelectedAndMouseIsOverColorString,
                                                            selectedAndMouseIsNotOverColorString,
                                                            selectedAndMouseIsOverColorString);

            menuItems = new List<MenuItemStyle>();

            AddMenuItem(priceListMenuItemStyle);
            AddMenuItem(ordersMenuItemStyle);
            AddMenuItem(documentsMenuItemStyle);
            AddMenuItem(syncMenuItemStyle);
            AddMenuItem(settingsMenuItemStyle);
        }

        #endregion

        #region Properties

        public string NotSelectedAndMouseIsNotOverBackgroundColor
        {
            get
            {
                return notSelectedAndMouseIsNotOverBackgroundColor;
            }

            set
            {
                if (notSelectedAndMouseIsNotOverBackgroundColor == null || notSelectedAndMouseIsNotOverBackgroundColor.Equals(value))
                {
                    notSelectedAndMouseIsNotOverBackgroundColor = value;
                    OnPropertyChanged();
                    menuItems.ForEach(
                        item =>
                        {
                            item.NotSelectedAndMouseIsNotOverBackgroundColor = NotSelectedAndMouseIsNotOverBackgroundColor;
                        });
                }
            }
        }

        public string NotSelectedAndMouseIsOverBackgroundColor
        {
            get
            {
                return notSelectedAndMouseIsOverBackgroundColor;
            }

            set
            {
                if (notSelectedAndMouseIsOverBackgroundColor != value)
                {
                    notSelectedAndMouseIsOverBackgroundColor = value;
                    OnPropertyChanged();
                    menuItems.ForEach(
                        item =>
                        {
                            item.NotSelectedAndMouseIsOverBackgroundColor = NotSelectedAndMouseIsOverBackgroundColor;
                        });
                }

            }

        }

        public string SelectedAndMouseIsNotOverBackgroundColor
        {
            get
            {
                return selectedAndMouseIsNotOverBackgroundColor;
            }

            set
            {
                if (selectedAndMouseIsNotOverBackgroundColor == null || selectedAndMouseIsNotOverBackgroundColor.Equals(value))
                {
                    selectedAndMouseIsNotOverBackgroundColor = value;
                    OnPropertyChanged();
                    menuItems.ForEach(
                        item =>
                        {
                            item.SelectedAndMouseIsNotOverBackgroundColor = SelectedAndMouseIsNotOverBackgroundColor;
                        });
                }

            }

        }

        public string SelectedAndMouseIsOverBackgroundColor
        {
            get
            {
                return selectedAndMouseIsOverBackgroundColor;
            }

            set
            {
                if (selectedAndMouseIsOverBackgroundColor == null || selectedAndMouseIsOverBackgroundColor.Equals(value))
                {
                    selectedAndMouseIsOverBackgroundColor = value;
                    OnPropertyChanged();
                    menuItems.ForEach(
                        item =>
                        {
                            item.SelectedAndMouseIsOverBackgroundColor = SelectedAndMouseIsOverBackgroundColor;
                        });
                }

            }

        }

        public MenuItemStyle PriceListMenuItemStyle
        {
            get
            {
                return priceListMenuItemStyle;
            }

            set
            {
                if (priceListMenuItemStyle == null || !priceListMenuItemStyle.Equals(value))
                {
                    priceListMenuItemStyle = ReplaceMenuItem(priceListMenuItemStyle, value);
                    OnPropertyChanged();
                }

            }

        }

        public MenuItemStyle OrdersMenuItemStyle
        {
            get
            {
                return ordersMenuItemStyle;
            }

            set
            {
                if (ordersMenuItemStyle == null || !ordersMenuItemStyle.Equals(value))
                {
                    ordersMenuItemStyle = ReplaceMenuItem(ordersMenuItemStyle, value);
                    OnPropertyChanged();
                }

            }

        }

        public MenuItemStyle DocumentsMenuItemStyle
        {
            get
            {
                return documentsMenuItemStyle;
            }

            set
            {
                if (documentsMenuItemStyle == null || !documentsMenuItemStyle.Equals(value))
                {
                    documentsMenuItemStyle = ReplaceMenuItem(documentsMenuItemStyle, value);
                    OnPropertyChanged();
                }

            }

        }

        public MenuItemStyle SyncMenuItemStyle
        {
            get
            {
                return syncMenuItemStyle;
            }

            set
            {
                if (syncMenuItemStyle == null || !syncMenuItemStyle.Equals(value))
                {
                    syncMenuItemStyle = ReplaceMenuItem(syncMenuItemStyle, value);
                    OnPropertyChanged();
                }

            }

        }

        public MenuItemStyle SettingsMenuItemStyle
        {
            get
            {
                return settingsMenuItemStyle;
            }

            set
            {
                if (settingsMenuItemStyle == null || !settingsMenuItemStyle.Equals(value))
                {
                    settingsMenuItemStyle = ReplaceMenuItem(settingsMenuItemStyle, value);
                    OnPropertyChanged();
                }

            }

        }

        #endregion

        #region Methods

        private MenuItemStyle ReplaceMenuItem(MenuItemStyle oldMenuItem, MenuItemStyle newMenuItem)
        {
            DeleteMenuItem(oldMenuItem);
            AddMenuItem(newMenuItem);

            return newMenuItem;
        }

        private void AddMenuItem(MenuItemStyle menuItem)
        {
            menuItems.Add(menuItem);
            SubscribeToEventOnChangeMenuItem(menuItem);
        }

        private void DeleteMenuItem(MenuItemStyle menuItem)
        {
            UnsubscribeToEventOnChangeMenuItem(menuItem);
            menuItems.Remove(menuItem);
        }

        private void SubscribeToEventOnChangeMenuItem(MenuItemStyle menuItem)
        {
            menuItem.PropertyChanged += RefreshMenuItems;
        }

        private void UnsubscribeToEventOnChangeMenuItem(MenuItemStyle menuItem)
        {
            menuItem.PropertyChanged -= RefreshMenuItems;
        }

        private void RefreshMenuItems(object sender, PropertyChangedEventArgs args)
        {
            if (args != null && args.PropertyName == "Selected")
            {
                MenuItemStyle menuItem = sender as MenuItemStyle;

                if (menuItem != null && menuItem.Selected)
                {
                    menuItems.Where(item => item != menuItem).ToList().ForEach(item => item.Selected = false);
                    OnMenuChanged(menuItem.Name);
                }

            }

        }

        public void SetMouseOverMenuItems(TopMenuMouseOverEventArgs args)
        {
            MenuItemName menuItemName = args.MenuItemName;

            switch (menuItemName)
            {
                case MenuItemName.PriceList:
                    PriceListMenuItemStyle.IsMouseOver = args.IsMouseOver;
                    break;
                case MenuItemName.Orders:
                    OrdersMenuItemStyle.IsMouseOver = args.IsMouseOver;
                    break;
                case MenuItemName.Documents:
                    DocumentsMenuItemStyle.IsMouseOver = args.IsMouseOver;
                    break;
                case MenuItemName.Sync:
                    SyncMenuItemStyle.IsMouseOver = args.IsMouseOver;
                    break;
                case MenuItemName.Settings:
                    SettingsMenuItemStyle.IsMouseOver = args.IsMouseOver;
                    break;
            }

        }

        public void SetMouseUpMenuItems(MenuChangedEventArgs args)
        {
            MenuItemName menuItemName = args.MenuItemName;

            switch (menuItemName)
            {
                case MenuItemName.PriceList:
                    PriceListMenuItemStyle.Selected = true;
                    break;
                case MenuItemName.Orders:
                    OrdersMenuItemStyle.Selected = true;
                    break;
                case MenuItemName.Documents:
                    DocumentsMenuItemStyle.Selected = true;
                    break;
                case MenuItemName.Sync:
                    SyncMenuItemStyle.Selected = true;
                    break;
                case MenuItemName.Settings:
                    SettingsMenuItemStyle.Selected = true;
                    break;
            }

        }

        #endregion

        #region Events

        public delegate void MenuChangedEventHandler(object sender, MenuChangedEventArgs args);

        public event MenuChangedEventHandler MenuChanged;

        private void OnMenuChanged(MenuItemName menuName)
        {
            MenuChanged?.Invoke(this, new MenuChangedEventArgs(menuName));
        }

        #endregion
    }
}
