using System.Globalization;
using System.Windows.Controls;

namespace Domain.Data.Validation
{
    public class BatchOfSalesRule : ValidationRule
    {
        #region Properties

        public BatchValue ValidValue { get; set; }

        #endregion

        #region Methods

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;

            if (ValidValue.SelectedItem != null)
            {
                decimal count;
                decimal batch = ValidValue.SelectedItem.BatchOfSales; //(ValidValue.BatchOfSales as CatalogItem)?.BatchOfSales ?? 0;

                if (decimal.TryParse(value as string, out count))
                {
                    if (batch != 0 && count % batch != 0)
                    {
                        result = new ValidationResult(false,
                            "Количество заказываемого товара должно быть кратно значению " + batch +
                            ", увазанному в поле \"Партия продажи\".");
                    }
                }
                else
                {
                    result = new ValidationResult(false, "Недопустимые символы.");
                }
            }

            ValidValue.HasError.Value = !result.IsValid;
            return result;
        }

        #endregion
    }
}
