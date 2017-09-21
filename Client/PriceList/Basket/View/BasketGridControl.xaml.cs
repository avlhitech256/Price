using System.Windows.Controls;
using System.Windows.Input;
using Basket.ViewModel;

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
                else if (Equals(grid.CurrentColumn, DeleteIconColumn))
                {
                    DeleteItem();
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
                else if (Equals(grid.CurrentColumn, DeleteIconColumn))
                {
                    DeleteItem();
                }
            }
        }

        private void ShowPicture()
        {
            ViewModel?.ShowPicture();
        }

        private void DeleteItem()
        {
            ViewModel?.DeleteItem();
        }

        #endregion
    }
}