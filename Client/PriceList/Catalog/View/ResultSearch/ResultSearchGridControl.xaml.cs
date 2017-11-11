using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;
using Catalog.ViewModel;
using Common.Data.Enum;
using Domain.Data.Object;
using Domain.DomainContext;
using Options.Service;
using Options.Service.Implementation;

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
        private readonly Dictionary<DataGridColumn, string> columns;

        #endregion

        #region Constructors

        public ResultSearchGridControl()
        {
            InitializeComponent();
            InitBrushs();
            columns = new Dictionary<DataGridColumn, string>
            {
                { NumberColumn, "" },
                { CodeColumn, "" },
                { ArticleColumn, "" },
                { NameColumn, "" },
                { BrandColumn, "" },
                { UnitColumn, "" },
                { EnterpriceNormPackColumn, "" },
                { BatchOfSalesColumn, "" },
                { BalanceColumn, "" },
                { PriceColumn, "" },
                { CountColumn, "" }
            };
        }

        #endregion

        #region Properties

        private CatalogViewModel ViewModel => (CatalogViewModel) DataContext;

        private IDomainContext DomainContext => ViewModel?.DomainContext;

        private IOptionService OptionService => DomainContext.OptionService;

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

        public void Refresh()
        {
            CollectionViewSource.GetDefaultView(ResultSearchDataGrid.ItemsSource).Refresh(); ResultSearchDataGrid.
        } 

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

        public bool HasResultGridErrors()
        {
            var sb = new StringBuilder();
            GetErrors(sb, ResultSearchDataGrid);
            return !string.IsNullOrWhiteSpace(sb.ToString());
        }

        private void GetErrors(StringBuilder sb, DependencyObject obj)
        {
            foreach (object child in LogicalTreeHelper.GetChildren(obj))
            {
                TextBox element = child as TextBox;
                if (element == null) continue;

                if (Validation.GetHasError(element))
                {
                    sb.Append(element.Text + " найдена ошибка:\r\n");
                    foreach (ValidationError error in Validation.GetErrors(element))
                    {
                        sb.Append("  " + error.ErrorContent.ToString());
                        sb.Append("\r\n");
                    }
                }

                GetErrors(sb, element);
            }
        }

        #endregion
    }
}
