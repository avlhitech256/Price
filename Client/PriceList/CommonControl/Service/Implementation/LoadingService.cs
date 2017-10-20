using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using CommonControl.LoadingControl;
using Domain.ViewModel;

namespace CommonControl.Service.Implementation
{
    public class LoadingService : ILoadingService
    {
        #region Members

        private LoadingStatus loadingStatus;
        private readonly Rectangle background;
        private readonly WaitControl loadControl;
        private readonly DispatcherTimer splashAnimationTimer;
        private readonly IControlViewModel viewModel;

        #endregion

        #region Constructors
        public LoadingService(IControlViewModel viewModel, Rectangle background, WaitControl loadControl)
        {
            this.viewModel = viewModel;
            InitViewModel();
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

        private void InitViewModel()
        {
            viewModel.ShowWaitScreen = ShowWaitScreen;
            viewModel.HideWaitScreen = HideWaitScreen;
        }

        private void ShowWaitScreen(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                loadControl.Text = message;
            }

            BeginUpdate();
        }

        public void HideWaitScreen()
        {
            ComplateUpdate();
        }

        private void SplashAnimationTimer_Tick(object sender, EventArgs e)
        {
            splashAnimationTimer.Stop();

            if (viewModel != null)
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

            if (viewModel != null)
            {
                viewModel.IsLoading = true;
                viewModel.IsWaiting = false;
                loadingStatus = LoadingStatus.BeginLoading;
                splashAnimationTimer.Interval = WaitToContinueTime;
                splashAnimationTimer.Start();

                var animation = new DoubleAnimation
                {
                    From = background.Opacity,
                    To = 0.7,
                    Duration = StartAnimationTime
                };

                background.BeginAnimation(UIElement.OpacityProperty, animation);
            }
        }

        private void ContinueUpdate()
        {
            splashAnimationTimer.Stop();

            if (viewModel != null)
            {
                viewModel.IsLoading = true;
                viewModel.IsWaiting = true;
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

            if (viewModel != null)
            {
                viewModel.IsLoading = true;
                viewModel.IsWaiting = true;
                loadingStatus = LoadingStatus.ComplateLoading;
                splashAnimationTimer.Interval = FinishAnimationTime;
                splashAnimationTimer.Start();

                var animation = new DoubleAnimation
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

            if (viewModel != null)
            {
                viewModel.IsLoading = false;
                viewModel.IsWaiting = false;
            }
        }

        #endregion
    }
}
