using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Catalog.ViewModel;
using Common.Data.Enum;
using Common.Event;
using Common.Messenger.Implementation;
using CommonControl.Animation;
using CommonControl.LoadingControl;
using CommonControl.SearchControl;
using Domain.Data.Object;
using static System.Double;

namespace Catalog.View
{
    /// <summary>
    /// Логика взаимодействия для CatalogControl.xaml
    /// </summary>
    public partial class CatalogControl : SearchControl
    {
        #region Members

        private double lastWindth;
        private WaitWindow waitScreen;

        #endregion

        #region Constructors

        public CatalogControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected override void SetDomainContext()
        {
            var viewModel = DataContext as CatalogViewModel;

            if (viewModel != null)
            {
                viewModel.ShowWaitScreen = ShowWaitScreen;
                viewModel.HideWaitScreen = HideWaitScreen;
            }
        }

        protected override void SubscribeMessenger()
        {
            if (Messenger != null)
            {
                Messenger.Register<CatalogItem>(CommandName.SetImage, SetImage, CanSetImage);
                Messenger.Register<DoubleAnimationEventArgs>(CommandName.ShowAdvanceSearchControl, 
                                                             ShowAdvanceSearchControl, 
                                                             CanShowAdvanceSearchControl);
                var viewModel = DataContext as CatalogViewModel;

                if (viewModel != null)
                {
                    CatalogItem item = viewModel.SelectedItem;
                    Messenger.Send(CommandName.SetImage, item);
                }
            }
        }

        protected override void UnsubscribeMessenger()
        {
            Messenger?.Unregister(CommandName.SetImage);
        }

        private void ShowAdvanceSearchControl(DoubleAnimationEventArgs args)
        {
            int commonTime = (LeftColumn.ActualWidth - args.To) > 0 ? 300 : 200;
            int time = Math.Abs((int)((LeftColumn.ActualWidth - args.To) * commonTime / 110));

            if (Math.Abs(lastWindth) < Epsilon)
            {
                lastWindth = args.To;
            }

            double to;

            if (Math.Abs(args.To) < Epsilon)
            {
                lastWindth = LeftColumn.ActualWidth;
                to = 0;
            }
            else
            {
                to = lastWindth;
            }
            
            var animation = new GridLengthAnimation
            {
                From = new GridLength(LeftColumn.ActualWidth, GridUnitType.Pixel),
                To = new GridLength(to, GridUnitType.Pixel),
                Duration = TimeSpan.FromMilliseconds(time)
            };

            animation.Completed += delegate
            {
                bool enable = args.To > 0;
                Splitter.IsEnabled = enable;
                Splitter.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
                Splitter.Width = enable ? 3 : 0;
            };

            LeftColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
        }
         

        private bool CanShowAdvanceSearchControl(DoubleAnimationEventArgs args)
        {
            return args != null;
        }

        private void SetImage(CatalogItem item)
        {
            if (item.HasPhotos && item.Photos.Any())
            {
                byte[] photo = item.Photos[0];
                DetailControl.DetailImage.Source = ImageService?.ConvertToBitmapImage(photo);
            }
        }

        private bool CanSetImage(CatalogItem item)
        {
            return item != null;
        }

        private void ShowWaitScreen(string message)
        {
            if (waitScreen == null)
            {
                waitScreen = new WaitWindow();
                waitScreen.Text = message;
                waitScreen.ShowDialog();
            }
            else
            {
                waitScreen.Text = message;
            }
        }

        private void HideWaitScreen()
        {
            if (waitScreen != null)
            {
                waitScreen.Close();
                waitScreen = null;
            }
        }

        #endregion
    }
}
