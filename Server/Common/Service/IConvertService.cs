using System;
using Common.Data.Enum;

namespace Common.Service
{
    public interface IConvertService
    {
        string Convert(OrderStatus status);

        OrderStatus Convert(string status);

        DateTimeOffset Convert(DateTime date);

        DateTime Convert(DateTimeOffset date);

        bool ConvertToGuid(string stringValue, out Guid value);

        Guid? ConvertToNullableGuid(string stringValue);

        bool ConvertToDecimal(string stringValue, out decimal value);

        decimal? ConvertToNullableDecimal(string stringValue);

        bool ConvertToDouble(string stringValue, out double value);

        double? ConvertToNullableDouble(string stringValue);
    }
}
