namespace Domain.Event
{
    public class ChildWindowScaleEventArgs
    {
        #region Constructors

        public ChildWindowScaleEventArgs(bool fullScale)
        {
            FullScale = fullScale;
        }

        #endregion

        #region Properties

        public bool FullScale { get; }

        #endregion
    }
}
