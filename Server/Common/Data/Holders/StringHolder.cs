namespace Common.Data.Holders
{
    public class StringHolder : Notifier.Notifier
    {
        #region Members

        private string value;

        #endregion

        #region Constructors

        public StringHolder(string value = "")
        {
            Value = value;
        }

        #endregion

        #region Properties

        public string Value
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
