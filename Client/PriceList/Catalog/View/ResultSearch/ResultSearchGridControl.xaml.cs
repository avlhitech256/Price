using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Catalog.ViewModel;
using Domain.DomainContext;
using Photo.Service;

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

        private IPhotoService PhotoService => DomainContext?.PhotoService;

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
                IEnumerable<byte[]> photos = ViewModel.SelectedItem.Photo;
                PhotoService.ShowPhotos(photos);
            }
        }

        #endregion
    }
}
