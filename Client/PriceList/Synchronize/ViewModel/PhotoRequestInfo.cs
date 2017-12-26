using System;
using System.Collections.Generic;

namespace Synchronize.ViewModel
{
    public class PhotoRequestInfo
    {
        #region Constructors

        public PhotoRequestInfo(DateTimeOffset lastUpdate, IEnumerable<long> ids)
        {
            LastUpdate = lastUpdate;
            Ids = ids;
        }

        #endregion

        #region Properties

        public DateTimeOffset LastUpdate { get; }

        public IEnumerable<long> Ids { get; }

        #endregion
    }
}
