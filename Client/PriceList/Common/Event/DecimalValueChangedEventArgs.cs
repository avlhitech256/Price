using System;
using Common.Data.Enum;

namespace Common.Event
{
    public class DecimalValueChangedEventArgs : EventArgs
    {
        #region Constructors
        public DecimalValueChangedEventArgs(long id, decimal oldValue, decimal newValue, MenuItemName entry)
        {
            Id = id;
            OldValue = oldValue;
            NewValue = newValue;
            Entry = entry;
        }

        #endregion

        #region Properties

        public long Id { get; }

        public decimal OldValue { get; }

        public decimal NewValue { get; }

        public MenuItemName Entry { get; }

        #endregion#
    }
}
