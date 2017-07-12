using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Catalog.Model;
using Catalog.View.Photo;
using Catalog.ViewModel;
using Common.Messenger;
using Common.Messenger.Implementation;
using Domain.Data.Object;
using Domain.DomainContext;
using Domain.Event;
using Domain.ViewModel;

namespace Catalog.View.ResultSearch
{
    /// <summary>
    /// Логика взаимодействия для ResultSearchGridControl.xaml
    /// </summary>
    public partial class ResultSearchGridControl : UserControl
    {
        public ResultSearchGridControl()
        {
            InitializeComponent();
        }

        private IDomainContext DomainContext => ((IControlViewModel) DataContext)?.DomainContext;

        private IMessenger Messenger => DomainContext?.Messenger;

        private void ResultSearchDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null)
            {
                if (Equals(grid.CurrentColumn, PhotoIconColumn))
                {
                    ShowPicture();
                }
            }
        }

        private void ShowPicture()
        {
            CatalogViewModel viewModel = DataContext as CatalogViewModel;

            if (viewModel != null)
            {
                CatalogItem selectedItem = viewModel.SelectedItem;
                var photoModel = new PhotoModel(Assemble(selectedItem.Photo));
                var photoViewModel = new PhotoViewModel(viewModel.DomainContext, photoModel);
                var photoView = new PhotoControl();
                var args = new ChildWindowEventArg(photoView, photoViewModel);
                Messenger.Send(CommandName.ShowImages, args);
            }
        }

        private ObservableCollection<BitmapSource> Assemble(List<byte[]> photos)
        {
            var result = new ObservableCollection<BitmapSource>();

            if (photos != null && photos.Any())
            {
                photos.ForEach(x => result.Add(Convert(x)));
            }

            return result;
        }

        private BitmapSource Convert(Bitmap imageData)
        {
            IntPtr hBitmap = imageData.GetHbitmap();
            BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
            BitmapSource image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero,
                Int32Rect.Empty, sizeOptions);

            return image;
        }

        private BitmapImage Convert(byte[] imageData)
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
