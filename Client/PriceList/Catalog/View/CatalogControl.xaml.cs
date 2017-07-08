using System.IO;
using System.Windows.Media.Imaging;
using Catalog.ViewModel;
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
            Messenger?.Register<CatalogItem>(CommandName.SetImage, SetImage, CanSetImage);
            var viewModel = DataContext as CatalogViewModel;

            if (viewModel != null)
            {
                CatalogItem item = viewModel.SelectedItem;
                Messenger?.Send(CommandName.SetImage, item);
            }
        }

        protected override void UnsubscribeMessenger()
        {
            Messenger?.Unregister(CommandName.SetImage);
        }

        private void SetImage(CatalogItem item)
        {
            byte[] photo = item.Photo;
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
