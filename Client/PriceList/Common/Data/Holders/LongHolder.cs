using System.Collections.Generic;

namespace Common.Data.Holders
{
    public class LongHolder : Notifier.Notifier
    {
        #region Members

        private long value;

        #endregion


        #region Constructors

        public LongHolder(long value = 0L)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public long Value
        {
            get
            {
                return value;
            }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion
    }
}
