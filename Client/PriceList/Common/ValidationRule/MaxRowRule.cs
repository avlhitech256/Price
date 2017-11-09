using System.Globalization;
using System.Windows.Controls;

namespace Common.ValidationRule
{
    public class MaxRowRule : System.Windows.Controls.ValidationRule
    {
        #region Constructors

        public MaxRowRule()
        {
            Min = 5;
            Max = 50;
        }

        #endregion

        #region Properties

        public int Min { get; set; }

        public int Max { get; set; }

        #endregion

        #region Methods

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            int maxRow;

            if (int.TryParse(value as string, out maxRow))
            {
                if ((maxRow < Min) || (maxRow > Max))
                {
                    result = new ValidationResult(false,
                      "Количество записей на странице не входит в диапазон " + Min + " до " + Max + ".");
                }
            }
            else
            {
                result = new ValidationResult(false, "Недопустимые символы.");
            }

            return result;
        }

        #endregion
    }
}
