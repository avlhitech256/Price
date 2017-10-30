using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonControl.LoadingControl;
using Domain.DomainContext;

namespace CommonControl.Service.Implementation
{
    public class LoadingService : ILoadingService
    {
        #region Members

        private readonly double maxBackgroundOpacity;
        private LoadingStatus loadingStatus;
        private readonly Rectangle background;
        private readonly WaitControl loadControl;
        private readonly DispatcherTimer splashAnimationTimer;
        private readonly ILoadingContext loadingContext;

        #endregion

        #region Constructors
        public LoadingService(ILoadingContext loadingContext, Rectangle background, WaitControl loadControl)
        {
            this.loadingContext = loadingContext;
            InitLoadingContext();
            maxBackgroundOpacity = 0.6;
            this.background = background;
            this.loadControl = loadControl;
            WaitToContinueTime = TimeSpan.FromMilliseconds(300);
            StartAnimationTime = TimeSpan.FromMilliseconds(300);
            FinishAnimationTime = TimeSpan.FromMilliseconds(300);
            AutoEndTime = TimeSpan.FromMilliseconds(2000);
            AutoEnd = false;
            loadingStatus = LoadingStatus.EndLoading;
            splashAnimationTimer = new DispatcherTimer();
            splashAnimationTimer.Interval = WaitToContinueTime;
            splashAnimationTimer.Tick += SplashAnimationTimer_Tick;
        }

        #endregion

        #region Properties

        public TimeSpan WaitToContinueTime { get; set; }

        public TimeSpan StartAnimationTime { get; set; }

        public TimeSpan FinishAnimationTime { get; set; }

        public TimeSpan AutoEndTime { get; set; }

        public bool AutoEnd { get; set; }

        #endregion

        #region Methods

        private void InitLoadingContext()
        {
            loadingContext.ShowWaitScreen = (x) => Application.Current.Dispatcher.Invoke(() => ShowWaitScreen(x));
            loadingContext.SetWaitScreenMessage = (x) => Application.Current.Dispatcher.Invoke(() => SetWaitScreenMessage(x));
            loadingContext.HideWaitScreen = () => Application.Current.Dispatcher.Invoke(HideWaitScreen);
        }

        private void ShowWaitScreen(string message)
        {
            SetWaitScreenMessage(message);
            BeginUpdate();
        }

        private void SetWaitScreenMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                loadControl.Text = message;
            }
        }

        public void HideWaitScreen()
        {
            ComplateUpdate();
        }

        private void SplashAnimationTimer_Tick(object sender, EventArgs e)
        {
            splashAnimationTimer.Stop();

            if (loadingContext != null)
            {
                switch (loadingStatus)
                {
                    case LoadingStatus.BeginLoading:
                        ContinueUpdate();
                        break;
                    case LoadingStatus.ContinueLoading:
                        ComplateUpdate();
                        break;
                    case LoadingStatus.ComplateLoading:
                        EndUpdate();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void BeginUpdate()
        {
            splashAnimationTimer.Stop();

            if (loadingContext != null)
            {
                loadingContext.IsLoading = true;
                loadingContext.IsWaiting = false;
                loadingStatus = LoadingStatus.BeginLoading;
                splashAnimationTimer.Interval = WaitToContinueTime;
                splashAnimationTimer.Start();

                var animation = new DoubleAnimation
                {
                    From = background.Opacity,
                    To = maxBackgroundOpacity,
                    Duration = StartAnimationTime
                };

                background.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private void ContinueUpdate()
        {
            splashAnimationTimer.Stop();

            if (loadingContext != null)
            {
                loadingContext.IsLoading = true;
                loadingContext.IsWaiting = true;
                loadingStatus = LoadingStatus.ContinueLoading;

                if (AutoEnd)
                {
                    splashAnimationTimer.Interval = AutoEndTime;
                    splashAnimationTimer.Start();
                }

                var animation = new DoubleAnimation
                {
                    From = loadControl.Opacity,
                    To = 1,
                    Duration = StartAnimationTime
                };

                loadControl.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private void ComplateUpdate()
        {
            splashAnimationTimer.Stop();

            if (loadingContext != null)
            {
                loadingContext.IsLoading = true;
                loadingContext.IsWaiting = true;
                loadingStatus = LoadingStatus.ComplateLoading;
                splashAnimationTimer.Interval = FinishAnimationTime;
                splashAnimationTimer.Start();

                var animation =  new DoubleAnimation
                {
                    From = loadControl.Opacity,
                    To = 0,
                    Duration = FinishAnimationTime
                };

                background.BeginAnimation(UIElement.OpacityProperty, animation);
                loadControl.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private void EndUpdate()
        {
            splashAnimationTimer.Stop();

            if (loadingContext != null)
            {
                loadingContext.IsLoading = false;
                loadingContext.IsWaiting = false;
            }
        }

        #endregion
    }
}
