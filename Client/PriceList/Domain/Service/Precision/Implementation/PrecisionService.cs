using System;
using System.Collections.Generic;
using System.Linq;
using Common.Data.Notifier;

namespace Domain.Service.Precision.Implementation
{
    public class PrecisionService : Notifier, IPrecisionService
    {
        #region Members

        private string format;
        private int precision;
        private bool separator;

        #endregion

        #region Constructors

        public PrecisionService() : this(2) { }

        public PrecisionService(int precision) : this(precision, true) { }

        public PrecisionService(int precision, bool separator)
        {
            Precision = precision;
            Separator = separator;
        }

        #endregion

        #region Properties

        public int Precision
        {
            get
            {
                return precision;
            }
            set
            {
                if (precision != value)
                {
                    precision = value;
                    Format = GetFormat(Precision, Separator);
                    OnPropertyChanged();
                }
            }

        }

        public bool Separator
        {
            get
            {
                return separator;
            }
            set
            {
                if (separator != value)
                {
                    separator = value;
                    Format = GetFormat(Precision, Separator);
                    OnPropertyChanged();
                }
            }

        }

        public string Format
        {
            get
            {
                return format;
            }
            private set
            {
                if (format != value)
                {
                    format = value;
                    OnPropertyChanged();
                }
            }

        }

        #endregion

        #region Methods

        public string NormalizeValue(string value)
        {
            string normalizeValue = NormalizeValue(value, Precision);
            return normalizeValue;
        }

        public string NormalizeValue(string value, int valuesPrecision)
        {
            string normalizeValue = NormalizeValue(value, valuesPrecision, Separator);
            return normalizeValue;
        }

        public string NormalizeValue(string value, int valuesPrecision, bool separatorIsVisible)
        {
            string stringValue = Convert(Convert(value), valuesPrecision, separatorIsVisible);
            return stringValue;
        }

        public string RemoveSymbols(string value)
        {
            List<char> listValue = value.ToCharArray().ToList();
            char[] validSymbols = {',', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            listValue.RemoveAll(x => !validSymbols.Contains(x));
            string result = new string(listValue.ToArray());
            int lastIndexPoint = result.LastIndexOf('.');
            int lastIndexComa = result.LastIndexOf(',');
            int lastIndex = Math.Max(lastIndexComa, lastIndexPoint);

            if (lastIndex >= 0)
            {
                string fraction = lastIndex + 1 < result.Length && lastIndex >= 0
                    ? result.Substring(lastIndex + 1)
                    : string.Empty;
                string whole = result.Substring(0, lastIndex + 1).Replace(".", "").Replace(",", "");
                result = whole + (string.IsNullOrWhiteSpace(whole) ? string.Empty : "," + fraction);
            }

            return result;
        }

        public decimal Convert(string value)
        {
            string stringValue = RemoveSymbols(value);
            decimal result;
            if (!decimal.TryParse(stringValue, out result))
            {
                result = 0;
            }

            return result;
        }

        public string Convert(decimal value)
        {
            string result = Convert(value, Precision);
            return result;
        }

        public string Convert(decimal value, int valuesPrecision)
        {
            string result = Convert(value, valuesPrecision, Separator);
            return result;
        }

        public string Convert(decimal value, int valuesPrecision, bool separatorIsVisible)
        {
            Format = GetFormat(valuesPrecision, separatorIsVisible);
            value = Math.Round(value, valuesPrecision);
            string result = value.ToString(format);
            return result;
        }

        public string GetFormat()
        {
            return Format;
        }

        public string GetFormat(int valuesPrecision)
        {
            return GetFormat(valuesPrecision, Separator);
        }

        public string GetFormat(int valuesPrecision, bool separatorIsVisible)
        {
            string stringFormat = separatorIsVisible ? "#,0" : "0";
            string precisionString = string.Empty;

            if (valuesPrecision > 0)
            {
                precisionString = ".";

                for (int i = 0; i < valuesPrecision; i++)
                {
                    precisionString += "0";
                }
            }

            stringFormat += precisionString;
            Format = stringFormat;

            return stringFormat;
        }

        #endregion
    }

}
