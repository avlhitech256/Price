using System.Windows.Controls;
using System.Windows.Input;
using Catalog.View.Photo;
using Catalog.ViewModel;
using Domain.Data.Object;

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

        private void ResultSearchDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null)
            {
                if (grid.CurrentColumn == PhotoIconColumn)
                {
                    CatalogViewModel viewModel = DataContext as CatalogViewModel;

                    if (viewModel != null)
                    {
                        CatalogItem selectedItem = viewModel.SelectedItem;

                        var view = new PhotoControl();
                    }
                }
            }
        }
    }
}
