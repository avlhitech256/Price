using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Catalog.SearchCriteria;
using Catalog.ViewModel;
using Common.Data.Constant;
using Common.Data.Enum;
using Domain.Data.Object;
using Domain.DomainContext;
using Template.Service;
using Template.Service.Implementation;

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
        private bool isUpdateTemplate;

        #endregion

        #region Constructors

        public ResultSearchGridControl()
        {
            isUpdateTemplate = false;
            InitializeComponent();
            InitBrushs();
            columns = new Dictionary<DataGridColumn, string>
            {
                { NumberColumn, CatalogColumnNames.NumberColumnName },
                { PhotoIconColumn, CatalogColumnNames.PhotoColumnName },
                { CodeColumn, CatalogColumnNames.CodeColumnName },
                { ArticleColumn, CatalogColumnNames.ArticleColumn },
                { NameColumn, CatalogColumnNames.NameColumn },
                { BrandColumn, CatalogColumnNames.BrandColumn },
                { UnitColumn, CatalogColumnNames.UnitColumn },
                { EnterpriceNormPackColumn, CatalogColumnNames.EnterpriceNormPackColumn },
                { BatchOfSalesColumn, CatalogColumnNames.BatchOfSalesColumn },
                { BalanceColumn, CatalogColumnNames.BalanceColumn },
                { PriceColumn, CatalogColumnNames.PriceColumn },
                { CountColumn, CatalogColumnNames.CountColumn }
            };
        }

        #endregion

        #region Properties

        private CatalogViewModel ViewModel => (CatalogViewModel) DataContext;

        private CatalogSearchCriteria SearchCriteria => ViewModel?.SearchCriteria;

        private IDomainContext DomainContext => ViewModel?.DomainContext;

        private ITemplateService TemplateService => DomainContext?.TemplateService;

        #endregion

        #region Methods

        public void GetTemplate()
        {
            if (TemplateService.IsExistCatalogTemplate(SearchCriteria.EnabledAdvancedSearch))
            {
                CatalogTemplate template = TemplateService.GetCatalogTemplate(SearchCriteria.EnabledAdvancedSearch);
                isUpdateTemplate = true;

                foreach (KeyValuePair<DataGridColumn, string> item in columns)
                {
                    double width;

                    if (!Equals(item.Key, PhotoIconColumn) && template.GridColumnsWidth.TryGetValue(item.Value, out width))
                    {
                        item.Key.Width = width;
                    }
                }

                isUpdateTemplate = false;
            }
            else
            {
                SetTemplate();
            }
        }

        private void SetTemplate()
        {
            if (!isUpdateTemplate)
            {
                var columnsWidth = new Dictionary<string, double>();

                ResultSearchDataGrid.Columns.ToList().ForEach(
                    x =>
                    {
                        string columnName;

                        if (columns.TryGetValue(x, out columnName))
                        {
                            columnsWidth.Add(columnName, x.ActualWidth);
                        }
                    });

                var template = new CatalogTemplate();
                template.GridColumnsWidth = columnsWidth;
                template.IsAdvanceSearch = SearchCriteria.EnabledAdvancedSearch;
                TemplateService?.SetCatalogTemplate(template);
            }
        }

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
            CollectionViewSource.GetDefaultView(ResultSearchDataGrid.ItemsSource).Refresh();
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

        private void ResultSearchDataGrid_OnColumnDisplayIndexChanged(object sender, DataGridColumnEventArgs e)
        {
            SetTemplate();
        }

        private void ResultSearchDataGrid_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            GetTemplate();
        }

        private void ResultSearchDataGrid_OnLayoutUpdated(object sender, EventArgs e)
        {
            SetTemplate();
        }

        private void ResultSearchDataGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            //SetTemplate();
        }
    }
}
