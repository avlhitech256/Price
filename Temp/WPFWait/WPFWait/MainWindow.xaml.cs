using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using WPFWait.Annotations;

namespace WPFWait
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer splashAnimationTimer;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MyClass();
            splashAnimationTimer = new DispatcherTimer();
            splashAnimationTimer.Interval = TimeSpan.FromMilliseconds(500);
            splashAnimationTimer.Tick += splashAnimationTimer_Tick;
            //splashAnimationTimer.Start();

        }

        private void splashAnimationTimer_Tick(object sender, EventArgs e)
        {
            splashAnimationTimer.Stop();
            var viewModel = DataContext as MyClass;

            if (viewModel != null)
            {
                switch (viewModel.LoadingStatus)
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
            var viewModel = DataContext as MyClass;

            if (viewModel != null)
            {
                viewModel.IsLoading = true;
                viewModel.IsWaiting = false;
                viewModel.LoadingStatus = LoadingStatus.BeginLoading;
                splashAnimationTimer.Interval = TimeSpan.FromMilliseconds(300);
                splashAnimationTimer.Start();

                var animation = new DoubleAnimation
                {
                    From = LoadingBackgroung.Opacity,
                    To = 0.7,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                LoadingBackgroung.BeginAnimation(OpacityProperty, animation);
            }
        }

        private void ContinueUpdate()
        {
            splashAnimationTimer.Stop();
            var viewModel = DataContext as MyClass;

            if (viewModel != null)
            {
                viewModel.IsLoading = true;
                viewModel.IsWaiting = true;
                viewModel.LoadingStatus = LoadingStatus.ContinueLoading;
                splashAnimationTimer.Interval = TimeSpan.FromMilliseconds(2000);
                splashAnimationTimer.Start();

                var animation = new DoubleAnimation
                {
                    From = WaitControl.Opacity,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(200)
                };

                WaitControl.BeginAnimation(OpacityProperty, animation);
            }
        }

        private void ComplateUpdate()
        {
            splashAnimationTimer.Stop();
            var viewModel = DataContext as MyClass;

            if (viewModel != null)
            {
                viewModel.IsLoading = true;
                viewModel.IsWaiting = true;
                viewModel.LoadingStatus = LoadingStatus.ComplateLoading;
                splashAnimationTimer.Interval = TimeSpan.FromMilliseconds(300);
                splashAnimationTimer.Start();

                var animation = new DoubleAnimation
                {
                    From = WaitControl.Opacity,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(300)
                };

                LoadingBackgroung.BeginAnimation(OpacityProperty, animation);
                WaitControl.BeginAnimation(OpacityProperty, animation);
            }
        }

        private void EndUpdate()
        {
            splashAnimationTimer.Stop();
            var viewModel = DataContext as MyClass;

            if (viewModel != null)
            {
                viewModel.IsLoading = false;
                viewModel.IsWaiting = false;
            }
        }

        sealed class MyClass : INotifyPropertyChanged
        {
            private bool isWaiting;
            private bool isLoading;
            private LoadingStatus loadingStatus;

            public bool IsWaiting
            {
                get
                {
                   return isWaiting; 
                }
                set
                {
                    if (isWaiting != value)
                    {
                        isWaiting = value;
                        OnPropertyChanged1();
                    }
                }
            }

            public bool IsLoading
            {
                get
                {
                    return isLoading;
                }
                set
                {
                    if (isLoading != value)
                    {
                        isLoading = value;
                        OnPropertyChanged1();
                    }
                }
            }

            public LoadingStatus LoadingStatus
            {
                get
                {
                    return loadingStatus;
                }
                set
                {
                    if (loadingStatus != value)
                    {
                        loadingStatus = value;
                        OnPropertyChanged1();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            private void OnPropertyChanged1([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            BeginUpdate();
        }

        public enum LoadingStatus
        {
            BeginLoading,
            ContinueLoading,
            ComplateLoading,
            EndLoading
        }
    }
}
