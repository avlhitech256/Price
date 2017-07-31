using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Basket.ViewModel;
using Domain.DomainContext;
using Photo.Service;

namespace Basket.View
{
    /// <summary>
    /// Логика взаимодействия для BasketGridControl.xaml
    /// </summary>
    public partial class BasketGridControl : UserControl
    {
        #region Constructor

        public BasketGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private BasketViewModel ViewModel => (BasketViewModel)DataContext;

        private IDomainContext DomainContext => ViewModel?.DomainContext;

        private IPhotoService PhotoService => DomainContext?.PhotoService;

        #endregion

        #region Methods

        private void BasketDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
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
        private void BasketDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                IEnumerable<byte[]> photos = ViewModel.SelectedItem.Photos;
                PhotoService.ShowPhotos(photos);
            }
        }

        #endregion
    }
}