using System;
using Common.Data.Enum;

namespace Common.Convert
{
    public interface IConvertService
    {
        string Convert(OrderStatus status);

        OrderStatus Convert(string status);

        DateTimeOffset Convert(DateTime date);

        DateTime Convert(DateTimeOffset date);
    }
}
