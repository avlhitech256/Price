using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Basket.ViewModel;
using Domain.DomainContext;
using Photo.Service;
using Photo.Service.Implementation;

namespace Basket.View
{
    /// <summary>
    /// Логика взаимодействия для BasketComplatedOrdersGridControl.xaml
    /// </summary>
    public partial class BasketComplatedOrdersGridControl : UserControl
    {
        #region Constructors

        public BasketComplatedOrdersGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private BasketViewModel ViewModel => (BasketViewModel)DataContext;

        #endregion

        #region Methods

        private void OrderDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null && ViewModel != null && !ViewModel.ShowPhotoOnMouseDoubleClick)
            {
                if (Equals(grid.CurrentColumn, DeleteIconColumn))
                {
                    DeleteItem();
                }
            }
        }
        private void OrderDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null && ViewModel != null && ViewModel.ShowPhotoOnMouseDoubleClick)
            {
                if (Equals(grid.CurrentColumn, DeleteIconColumn))
                {
                    DeleteItem();
                }
            }
        }

        private void DeleteItem()
        {
            ViewModel.OrderViewModel.DeleteOrder();
        }

        #endregion

    }
}
