using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Catalog.ViewModel;
using Domain.Data.Object;

namespace Catalog.View.Search
{
    /// <summary>
    /// Логика взаимодействия для BrandControl.xaml
    /// </summary>
    public partial class BrandControl : UserControl
    {
        #region Constructors

        public BrandControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private CatalogBrandViewModel ViewModel => (DataContext as CatalogViewModel)?.CatalogBrandViewModel;

        #endregion

        #region Methods

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            OnCheck(e);
        }

        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            //OnCheck(e);
        }

        private void OnCheck(RoutedEventArgs e)
        {
            CheckBox checkBox = e.OriginalSource as CheckBox;
            ContentPresenter presenter = checkBox?.TemplatedParent as ContentPresenter;
            BrandItem item = presenter?.Content as BrandItem;

            if (item != null)
            {
                ViewModel.OnCheck(item);
            }
        }

        public void Refresh()
        {
            CollectionViewSource.GetDefaultView(TreeView.ItemsSource).Refresh();
        }

        #endregion
    }
}
