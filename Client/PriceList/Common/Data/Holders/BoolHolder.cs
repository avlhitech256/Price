namespace Common.Data.Holders
{
    public class BoolHolder : Notifier.Notifier
    {
        #region Members

        private bool value;

        #endregion


        #region Constructors

        public BoolHolder(bool value = false)
        {
            Value = value;
        }

        #endregion

        #region Properties

        public bool Value
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