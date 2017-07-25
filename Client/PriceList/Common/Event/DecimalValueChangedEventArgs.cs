using System;

namespace Common.Event
{
    public class DecimalValueChangedEventArgs : EventArgs
    {
        #region Constructors
        public DecimalValueChangedEventArgs(long id, decimal oldValue, decimal newValue)
        {
            Id = id;
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion

        #region Properties

        public long Id { get; }

        public decimal OldValue { get; }

        public decimal NewValue { get; }

        #endregion#
    }
}
