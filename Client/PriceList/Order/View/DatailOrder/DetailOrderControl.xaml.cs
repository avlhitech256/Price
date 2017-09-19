using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Domain.DomainContext;
using Order.ViewModel;
using Photo.Service;

namespace Order.View.DatailOrder
{
    /// <summary>
    /// Логика взаимодействия для DetailOrderControl.xaml
    /// </summary>
    public partial class DetailOrderControl : UserControl
    {
        #region Constructors

        public DetailOrderControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private DetailOrderViewModel ViewModel => ((OrderViewModel)DataContext)?.DetailViewModel;

        #endregion

        #region Methods

        private void DetailOrderDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
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
        private void DetailOrderDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
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
            ViewModel.DeleteItem();
        }

        #endregion
    }
}
