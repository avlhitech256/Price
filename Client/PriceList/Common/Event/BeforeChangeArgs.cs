using System;

namespace Common.Event
{
    public class BeforeChangeArgs : EventArgs
    {
        public BeforeChangeArgs(bool enableAdvancedSearch)
        {
            EnableAdvancedSearch = enableAdvancedSearch;
        }

        public bool EnableAdvancedSearch { get; }
    }
}
