using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Catalog.ViewModel;

namespace Catalog.View.ResultSearch
{
    /// <summary>
    /// Логика взаимодействия для ResultSearchGridNavigateControl.xaml
    /// </summary>
    public partial class ResultSearchGridNavigateControl : UserControl
    {
        #region Constructors

        public ResultSearchGridNavigateControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private CatalogViewModel ViewModel => (CatalogViewModel)DataContext;

        #endregion

        #region Methods


        private void CurrentPage_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.CatalogNavigateViewModel.ValidateCurrentPage();
                ViewModel.CatalogNavigateViewModel.LoadCurrentPage();
            }
        }

        #endregion

        private void CurrentPage_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.CatalogNavigateViewModel.ValidateCurrentPage();
            ViewModel.CatalogNavigateViewModel.LoadCurrentPage();
        }

        private void MaxRow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.CatalogNavigateViewModel.ValidateMaximumRows();
                ViewModel.CatalogNavigateViewModel.LoadCurrentPage();
            }
        }

        private void MaxRow_OnLostFocus(object sender, RoutedEventArgs e)
        {
            ViewModel.CatalogNavigateViewModel.ValidateMaximumRows();
            ViewModel.CatalogNavigateViewModel.LoadCurrentPage();
        }
    }
}
