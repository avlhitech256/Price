using System.Windows;
using Common.Data.Holders;
using Domain.Data.Object;

namespace Domain.Data.Validation
{
    public class BatchValue : DependencyObject
    {
        #region Properties

        public static readonly DependencyProperty BatchProperty = 
            DependencyProperty.Register("SelectedItem", typeof(CatalogItem), typeof(BatchValue));

        public static readonly DependencyProperty HasErrorProperty =
            DependencyProperty.Register("HasError", typeof (BoolHolder), typeof (BatchValue));

        public CatalogItem SelectedItem
        {
            get
            {
                return (CatalogItem) GetValue(BatchProperty);
            }
            set
            {
                SetValue(BatchProperty, value);
            }
        }

        public BoolHolder HasError
        {
            get
            {
                return (BoolHolder) GetValue(HasErrorProperty);
            }
            set
            {
                SetValue(HasErrorProperty, value);
            }
        }

        #endregion
    }
}
