using System;

namespace Common.Event
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
