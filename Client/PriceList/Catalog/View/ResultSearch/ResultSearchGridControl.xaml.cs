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
using Media.Image;

namespace Catalog.View.ResultSearch
{
    /// <summary>
    /// Логика взаимодействия для ResultSearchGridControl.xaml
    /// </summary>
    public partial class ResultSearchGridControl : UserControl
    {
        #region Constructors

        public ResultSearchGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private CatalogViewModel ViewModel => (CatalogViewModel) DataContext;

        private IDomainContext DomainContext => ViewModel?.DomainContext;

        private IMessenger Messenger => DomainContext?.Messenger;

        private IImageService ImageService => DomainContext?.ImageService;

        #endregion

        #region Methods


        private void ResultSearchDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null && ViewModel != null && !ViewModel.ShowPhotoOnMouseDoubleClick)
            {
                if (Equals(grid.CurrentColumn, PhotoIconColumn))
                {
                    ShowPicture();
                }
            }
        }
        private void ResultSearchDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null && ViewModel != null && ViewModel.ShowPhotoOnMouseDoubleClick)
            {
                if (Equals(grid.CurrentColumn, PhotoIconColumn))
                {
                    ShowPicture();
                }
            }
        }

        private void ShowPicture()
        {
            if (ViewModel != null)
            {
                CatalogItem selectedItem = ViewModel.SelectedItem;
                var photoModel = new PhotoModel(Assemble(selectedItem.Photo));
                var photoViewModel = new PhotoViewModel(ViewModel.DomainContext, photoModel);
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
                photos.ForEach(x => result.Add(ImageService?.ConvertToBitmapImage(x)));
            }

            return result;
        }

        #endregion
    }
}
