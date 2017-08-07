using Common.Data.Enum;

namespace Common.Event
{
    public class SetMenuFocusEventArgs
    {
        #region Constructors

        public SetMenuFocusEventArgs(MenuItemName menuItemName)
        {
            MenuItemName = menuItemName;
        }

        #endregion

        #region Properties

        public MenuItemName MenuItemName { get; }

        #endregion
    }
}
