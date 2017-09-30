using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Catalog.ViewModel;
using Common.Data.Enum;
using Domain.Data.Object;

namespace Catalog.View.ResultSearch
{
    /// <summary>
    /// Логика взаимодействия для ResultSearchGridControl.xaml
    /// </summary>
    public partial class ResultSearchGridControl : UserControl
    {
        #region Members

        private SolidColorBrush lightWhiteBrush;
        private SolidColorBrush lightBlueBrush;
        private SolidColorBrush lightRadBrush;
        private SolidColorBrush lightGreenBrush;


        #endregion

        #region Constructors

        public ResultSearchGridControl()
        {
            InitializeComponent();
            InitBrushs();
        }

        #endregion

        #region Properties

        private CatalogViewModel ViewModel => (CatalogViewModel) DataContext;

        #endregion

        #region Methods

        private void ResultSearchDataGrid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null && ViewModel != null && !ViewModel.ShowPhotoOnMouseDoubleClick)
            {
                if (Equals(grid.CurrentColumn, PhotoIconColumn) && ViewModel.SelectedItem.HasPhotos)
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
                if (Equals(grid.CurrentColumn, PhotoIconColumn) && ViewModel.SelectedItem.HasPhotos)
                {
                    ShowPicture();
                }
            }
        }

        private void ShowPicture()
        {
            ViewModel.ShowPicture();
        }

        private void ResultSearchDataGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //CatalogItem selectedItem = e.AddedItems.Cast<CatalogItem>().FirstOrDefault();

            //if (ViewModel.Count > ViewModel.Entities.Count &&
            //    selectedItem != null &&
            //    selectedItem == ViewModel.Entities.LastOrDefault())
            //{
            //    ViewModel.SelectNexPageData();
            //}
        }

        #endregion

        private void ResultSearchDataGrid_OnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            CatalogItem item = (CatalogItem) e.Row.DataContext;
            e.Row.Background = lightWhiteBrush;


            switch (item.Status)
            {
                case CatalogItemStatus.Old:
                    e.Row.Background = lightWhiteBrush;
                    break;
                case CatalogItemStatus.New:

                    if (item.Entity.DateOfCreation >= DateTimeOffset.Now.AddDays(-14))
                    {
                        e.Row.Background = lightBlueBrush;
                    }

                    break;
                case CatalogItemStatus.PriceIsUp:

                    if (item.Entity.LastUpdatedStatus >= DateTimeOffset.Now.AddDays(-7))
                    {
                        e.Row.Background = lightRadBrush;
                    }

                    break;
                case CatalogItemStatus.PriceIsDown:

                    if (item.Entity.LastUpdatedStatus >= DateTimeOffset.Now.AddDays(-7))
                    {
                        e.Row.Background = lightGreenBrush;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitBrushs()
        {
            lightWhiteBrush = new SolidColorBrush(Colors.White);
            lightBlueBrush = new SolidColorBrush(Colors.LightSkyBlue);
            lightRadBrush = new SolidColorBrush(Colors.LightPink);
            lightGreenBrush = new SolidColorBrush(Colors.PaleGreen);
        }
    }
}
