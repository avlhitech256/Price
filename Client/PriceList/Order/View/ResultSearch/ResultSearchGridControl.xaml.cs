using System.Windows.Controls;
using Order.ViewModel;

namespace Order.View.ResultSearch
{
    /// <summary>
    /// Логика взаимодействия для ResultSearchGridControl.xaml
    /// </summary>
    public partial class ResultSearchGridControl : UserControl
    {
        #region Constructors

        public ResultSearchGridControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        private OrderViewModel ViewModel => DataContext as OrderViewModel;

        #endregion

        #region Methods

        private void ResultSearchDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = !Equals(e.Column, CommentColumn) || 
                       ViewModel?.SelectedItem == null ||
                       !ViewModel.SelectedItem.AllowChangeComment;
        }

        private void ResultSearchDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion
    }
}
