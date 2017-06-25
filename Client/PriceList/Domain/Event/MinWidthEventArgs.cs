namespace Domain.Event
{
    public class MinWidthEventArgs
    {
        #region Constructors

        public MinWidthEventArgs(int minWidth)
        {
            MinWidth = minWidth;
        }

        #endregion

        #region Properties

        public int MinWidth { get; }

        #endregion
    }
}
