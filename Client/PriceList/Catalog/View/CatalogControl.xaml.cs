using System;
using System.Windows.Media.Animation;
using Catalog.ViewModel;
using Common.Event;
using Common.Messenger.Implementation;
using CommonControl.SearchControl;
using Domain.Data.Object;

namespace Catalog.View
{
    /// <summary>
    /// Логика взаимодействия для CatalogControl.xaml
    /// </summary>
    public partial class CatalogControl : SearchControl
    {
        public CatalogControl()
        {
            InitializeComponent();
        }

        protected override void SetDomainContext()
        {
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
            int commonTime = (LeftAdvanceSearchControl.ActualWidth - args.To) > 0 ? 300 : 150;
            int time = Math.Abs((int)((LeftAdvanceSearchControl.ActualWidth - args.To) * commonTime / 110));

            var animation = new DoubleAnimation
            {
                From = LeftAdvanceSearchControl.ActualWidth,
                To = args.To,
                Duration = TimeSpan.FromMilliseconds(time)
            };

            LeftAdvanceSearchControl.BeginAnimation(WidthProperty, animation);
        }


        private bool CanShowAdvanceSearchControl(DoubleAnimationEventArgs args)
        {
            return args != null;
        }

        private void SetImage(CatalogItem item)
        {
            byte[] photo = item.Photos[0];
            DetailControl.DetailImage.Source = ImageService?.ConvertToBitmapImage(photo);
        }

        private bool CanSetImage(CatalogItem item)
        {
            return item != null;
        }
    }
}
