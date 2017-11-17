using System;
using Common.Data.Enum;

namespace Common.Service.Implementation
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

        public OrderStatus Convert(string status)
        {
            OrderStatus result;

            switch (status)
            {
                case "Любой статус":
                    result = OrderStatus.All;
                    break;
                case "Новый":
                    result = OrderStatus.New;
                    break;
                case "Отправлен на сервер":
                    result = OrderStatus.SentOut;
                    break;
                case "Принятый в обработку":
                    result = OrderStatus.Adopted;
                    break;
                case "Утвержден":
                    result = OrderStatus.Approved;
                    break;
                case "Отклонен":
                    result = OrderStatus.Cancel;
                    break;
                case "В работе":
                    result = OrderStatus.InWork;
                    break;
                case "Отправлен заказчику":
                    result = OrderStatus.Shipped;
                    break;
                case "В процессе доставки":
                    result = OrderStatus.InTransit;
                    break;
                case "Выполнен":
                    result = OrderStatus.Fulfilled;
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

        public bool ConvertToGuid(string stringValue, out Guid value)
        {
            return stringValue.ConvertToGuid(out value);
        }

        public Guid? ConvertToNullableGuid(string stringValue)
        {
            return stringValue.ConvertToNullableGuid();
        }

        public bool ConvertToDecimal(string stringValue, out decimal value)
        {
            return stringValue.ConvertToDecimal(out value);
        }

        public decimal? ConvertToNullableDecimal(string stringValue)
        {
            return stringValue.ConvertToNullableDecimal();
        }

        public bool ConvertToDouble(string stringValue, out double value)
        {
            return stringValue.ConvertToDouble(out value);
        }

        public double? ConvertToNullableDouble(string stringValue)
        {
            return stringValue.ConvertToNullableDouble();
        }
    }
}
