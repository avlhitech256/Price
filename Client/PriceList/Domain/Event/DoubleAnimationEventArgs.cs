using System;

namespace Domain.Event
{
    public class DoubleAnimationEventArgs : EventArgs
    {
        #region Constructors

        public DoubleAnimationEventArgs(double from, double to)
        {
            From = from;
            To = to;
        }

        #endregion

        #region Properties
        
        public double From { get; }
        public double To { get; }

        #endregion
    }
}
