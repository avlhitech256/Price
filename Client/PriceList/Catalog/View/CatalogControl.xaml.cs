using System;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Catalog.ViewModel;
using Common.Messenger.Implementation;
using CommonControl.SearchControl;
using Domain.Data.Object;
using Domain.Event;

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
            byte[] photo = item.Photo[0];
            DetailControl.DetailImage.Source = LoadImage(photo);
        }

        private bool CanSetImage(CatalogItem item)
        {
            return item != null;
        }

        private BitmapImage LoadImage(byte[] imageData)
        {
            BitmapImage image = null;

            if (imageData != null && imageData.Length > 0)
            {
                image = new BitmapImage();

                using (var mem = new MemoryStream(imageData))
                {
                    mem.Position = 0;
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = mem;
                    image.EndInit();
                }

                image.Freeze();
            }

            return image;
        }
    }
}
