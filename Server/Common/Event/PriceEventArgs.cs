using System;

namespace Common.Event
{
    public class PriceEventArgs<T> : EventArgs
    {
        public PriceEventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
