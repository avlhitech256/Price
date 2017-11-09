using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using Domain.Data.Object;

namespace Domain.Data.Validation
{
    public class BatchOfSalesRowRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = ValidationResult.ValidResult;
            CatalogItem item = (value as BindingGroup)?.Items[0] as CatalogItem;

            if (item != null)
            {
                if (item.Count % item.BatchOfSales != 0)
                {
                    result = new ValidationResult(false,
                        "Количество заказываемого товара должно быть кратно значению \"Партия продажи\" - " +
                        item.BatchOfSales + ".");
                }
            }

            return result;
        }
    }
}
