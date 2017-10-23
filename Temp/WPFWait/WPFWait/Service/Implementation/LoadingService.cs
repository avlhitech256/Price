using System;
using System.Windows.Controls;

namespace WPFWait.Service.Implementation
{
    public class LoadingService : ILoadingService
    {
        #region Members

        private readonly UserControl parent;

        #endregion

        #region Constructors
        public LoadingService(UserControl parent)
        {
            this.parent = parent;
            WaitToContinueTime = TimeSpan.FromMilliseconds(300);
            StartAnimationTime = TimeSpan.FromMilliseconds(300);
            FinishAnimationTime = TimeSpan.FromMilliseconds(300);
            AutoEndTime = TimeSpan.FromMilliseconds(2000);
            AutoEnd = false;
        }

        public TimeSpan WaitToContinueTime { get; set; }

        public TimeSpan StartAnimationTime { get; set; }

        public TimeSpan FinishAnimationTime { get; set; }

        public TimeSpan AutoEndTime { get; set; }

        public bool AutoEnd { get; set; }

        public void Begin()
        {
            throw new NotImplementedException();
        }

        public void Complate()
        {
            throw new NotImplementedException();
        }
    }
}
