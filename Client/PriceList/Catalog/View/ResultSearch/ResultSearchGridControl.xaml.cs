using System.Windows.Controls;
using System.Windows.Input;

namespace Catalog.Presentation.ResultSearch
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

        private void Aa()
        {
            //ResultSearchDataGrid.MouseDoubleClick
        }

        private void ResultSearchDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid grid = sender as DataGrid;

            if (grid != null)
            {
                if (grid.CurrentColumn == PhotoIconColumn)
                {
                    ;
                }
            }
        }
    }
}
