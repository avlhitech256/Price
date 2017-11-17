using System;

namespace Common.Event
{
    public class EnableMenuEventArgs : EventArgs
    {
        #region Constructors

        public EnableMenuEventArgs(bool enable)
        {
            Enable = enable;
        }

        #endregion

        #region Properties

        public bool Enable { get; private set; }

        #endregion
    }
}
