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
        private bool enabledSaveSplitPosition;

        #endregion

        #region Constructors

        public CatalogControl()
        {
            InitializeComponent();
            enabledSaveSplitPosition = false;
            Unloaded += CatalogControl_Unloaded;
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
                var viewModel = ViewModel as CatalogViewModel;

                if (viewModel != null)
                {
                    viewModel.RefreshDirectoryView = LeftAdvanceSearchControl.DirectorySearchControl.Refresh;
                    viewModel.RefreshBrandView = LeftAdvanceSearchControl.BrandSearchControl.Refresh;
                    viewModel.RefreshCatalogView = ResultSearchGridControl.Refresh;
                }

                ViewModel.SetEnabled = SetEnabled;
                ViewModel.HasResultGridErrors = ResultSearchGridControl.HasResultGridErrors;
                ViewModel.Init();
                InitSplitterPosition();
            }
        }

        private void InitSplitterPosition()
        {
            CatalogViewModel viewModel = ViewModel as CatalogViewModel;

            if (viewModel != null)
            {
                lastWindth = viewModel.SplitterPosition;
                double width = viewModel.SearchCriteria.EnabledAdvancedSearch ? lastWindth : 0;
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

        private void SetEnabled(bool enable)
        {
            LeftAdvanceSearchControl.IsEnabled = enable;
            MainSearchControl.IsEnabled = enable;
            ResultSearchGridControl.GridNavigateControl.IsEnabled = enable;
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
            enabledSaveSplitPosition = args.To > 0;
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

        private void SaveSplitterPosition(bool force = false)
        {
            if (enabledSaveSplitPosition || force)
            {
                CatalogViewModel viewModel = ViewModel as CatalogViewModel;

                if (viewModel != null && viewModel.SearchCriteria != null &&
                    viewModel.SearchCriteria.EnabledAdvancedSearch)
                {
                    viewModel.SplitterPosition = LeftColumn.ActualWidth;
                }

                enabledSaveSplitPosition = false;
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

        private void Splitter_OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            SaveSplitterPosition();
        }

        private void CatalogControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveSplitterPosition(true);
        }

        #endregion

    }

}
