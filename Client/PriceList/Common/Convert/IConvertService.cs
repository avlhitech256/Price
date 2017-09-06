using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Data.Enum;

namespace Common.Convert
{
    public interface IConvertService
    {
        string Convert(OrderStatus status);

        DateTimeOffset Convert(DateTime date);

        DateTime Convert(DateTimeOffset date);
    }
}
