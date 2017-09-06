using System;
using System.Globalization;
using System.Windows.Data;
using Common.Data.Enum;

namespace Common.Convert.Implementation
{
    public class ConvertService : IConvertService
    {
        public string Convert(OrderStatus status)
        {
            string result;

            switch (status)
            {
                case OrderStatus.All:
                    result = "Любой статус";
                    break;
                case OrderStatus.New:
                    result = "Новый";
                    break;
                case OrderStatus.SentOut:
                    result = "Отправлен на сервер";
                    break;
                case OrderStatus.Adopted:
                    result = "Принятый в обработку";
                    break;
                case OrderStatus.Approved:
                    result = "Утвержден";
                    break;
                case OrderStatus.Cancel:
                    result = "Отклонен";
                    break;
                case OrderStatus.InWork:
                    result = "В работе";
                    break;
                case OrderStatus.Shipped:
                    result = "Отправлен заказчику";
                    break;
                case OrderStatus.InTransit:
                    result = "В процессе доставки";
                    break;
                case OrderStatus.Fulfilled:
                    result = "Выполнен";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public DateTimeOffset Convert(DateTime date)
        {
            DateTimeOffset result; 

            try
            {
                result = new DateTimeOffset(date);
            }
            catch (Exception)
            {
                result = DateTimeOffset.MinValue;
            }

            return result;
        }

        public DateTime Convert(DateTimeOffset date)
        {
            DateTime result;

            try
            {
                result = date.DateTime;
            }
            catch (Exception)
            {
                result = DateTime.MinValue;
            }

            return result;
        }
    }
}
