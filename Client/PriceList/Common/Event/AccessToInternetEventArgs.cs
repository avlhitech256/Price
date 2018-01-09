using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Event
{
    public class AccessToInternetEventArgs : EventArgs
    {
        #region Constructors

        public AccessToInternetEventArgs(bool enable)
        {
            Enable = enable;
        }

        #endregion

        #region Properties

        public bool Enable { get; private set; }

        #endregion
    }
}
