using System;
using Common.Data.Enum;

namespace Common.Event
{
    public class TopMenuMouseOverEventArgs : EventArgs
    {
        #region Constructors

        public TopMenuMouseOverEventArgs(MenuItemName menuItemName, bool isMouseOver)
        {
            MenuItemName = menuItemName;
            IsMouseOver = isMouseOver;
        }

        #endregion

        #region Properties

        public MenuItemName MenuItemName { get; }
        public bool IsMouseOver { get; }

        #endregion

        #region Methods

        public bool Equals(TopMenuMouseOverEventArgs other)
        {
            return MenuItemName == other.MenuItemName &&
                   IsMouseOver == other.IsMouseOver;
        }

        #endregion
    }
}
