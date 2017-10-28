using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Catalog.ViewModel;
using Common.Event;
using Common.Messenger.Implementation;
using CommonControl.Animation;
using CommonControl.SearchControl;
using CommonControl.Service;
using CommonControl.Service.Implementation;
using Domain.Data.Object;
using Domain.ViewModel;
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
        private ILoadingService loadService;

        #endregion

        #region Constructors

        public CatalogControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        IControlViewModel ViewModel => DataContext as IControlViewModel;

        #endregion

        #region Methods

        protected override void SetDomainContext()
        {
            if (ViewModel != null)
            {
                ViewModel.RefreshView = RefreshView;
                loadService = new LoadingService(ViewModel, LoadingBackgroung, WaitControl);
                ViewModel.Init();
                InitSplitterPosition();
            }
        }

        private void InitSplitterPosition()
        {
            CatalogViewModel viewModel = ViewModel as CatalogViewModel;

            if (viewModel != null)
            {
                double width = viewModel.SplitterPosition;
                LeftColumn.Width = new GridLength(width, GridUnitType.Pixel);
                SetSplitter(width);
            }
        }

        private void SetSplitter(double width)
        {
            bool enable = width > 0;
            Splitter.IsEnabled = enable;
            Splitter.Visibility = enable ? Visibility.Visible : Visibility.Collapsed;
            Splitter.Width = enable ? 3 : 0;
        }

        private void RefreshView()
        {
            LeftAdvanceSearchControl.DirectorySearchControl.Refresh();
            LeftAdvanceSearchControl.BrandSearchControl.Refresh();
            ResultSearchGridControl.Refresh();
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
                SetSplitter(args.To);

                if (args.To > 0)
                {
                    SaveSplitterPosition();
                }
            };

            LeftColumn.BeginAnimation(ColumnDefinition.WidthProperty, animation);
        }

        private void SaveSplitterPosition()
        {
            CatalogViewModel viewModel = ViewModel as CatalogViewModel;

            if (viewModel != null)
            {
                viewModel.SplitterPosition = LeftColumn.ActualWidth;
            }
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

        #endregion

        private void Splitter_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            SaveSplitterPosition();
        }
    }
}
