using System;

namespace CommonControl.Service
{
    public interface ILoadingService
    {
        TimeSpan WaitToContinueTime { get; set; }

        TimeSpan StartAnimationTime { get; set; }

        TimeSpan FinishAnimationTime { get; set; }

        TimeSpan AutoEndTime { get; set; }

        bool AutoEnd { get; set; }
    }
}
