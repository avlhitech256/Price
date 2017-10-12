using System.Windows;
using System.Windows.Controls;
using Catalog.ViewModel;
using Domain.Data.Object;

namespace Catalog.View.Search
{
    /// <summary>
    /// Логика взаимодействия для CatalogDirectorySearchControl.xaml
    /// </summary>
    public partial class CatalogDirectorySearchControl : UserControl
    {
        #region Constructors

        public CatalogDirectorySearchControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private CatalogDirectoryViewModel ViewModel => (DataContext as CatalogViewModel)?.CatalogDirectoryViewModel;

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
            DirectoryItem item = presenter?.Content as DirectoryItem;

            if (item != null)
            {
                ViewModel.OnCheck(item);
            }
        }

        #endregion
    }
}
