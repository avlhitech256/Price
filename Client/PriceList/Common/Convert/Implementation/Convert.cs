﻿using System;
using System.Globalization;
using System.Linq;

namespace Common.Convert.Implementation
{
    public static class Convert
    {
        public static Guid? ConvertToNullableGuid(this string stringValue)
        {
            Guid value;
            Guid? result = stringValue.ConvertToGuid(out value) ? value : (Guid?) null;
            return result;
        }

        public static Guid ConvertToGuid(this string stringValue)
        {
            Guid value;
            stringValue.ConvertToGuid(out value);
            return value;
        }

        public static bool ConvertToGuid(this string stringValue, out Guid value)
        {
            value = Guid.Empty;
            bool result = !string.IsNullOrWhiteSpace(stringValue) && Guid.TryParse(stringValue, out value);
            return result;
        }

        public static bool ConvertToDecimal(this string stringValue, out decimal value)
        {
            string[] separator = {" ", ",", "."};
            string valueToConvert =
                stringValue.Trim().Replace(CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator, string.Empty);

            separator.ToList()
                .ForEach(
                    x =>
                        valueToConvert =
                            valueToConvert.Replace(x, CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator));

            value = 0;
            bool result = !string.IsNullOrWhiteSpace(valueToConvert) && decimal.TryParse(valueToConvert, out value);

            return result;
        }

        public static decimal? ConvertToNullableDecimal(this string stringValue)
        {
            decimal value;
            decimal? result = stringValue.ConvertToDecimal(out value) ? value : (decimal?) null;
            return result;
        }

        public static decimal ConvertToDecimal(this string stringValue)
        {
            decimal value;
            stringValue.ConvertToDecimal(out value);
            return value;
        }
    }
}
