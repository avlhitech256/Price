using System;
using System.Collections.Generic;
using DatabaseService.Objects.Enum;

namespace DatabaseService.DataBaseContext.Entities
{
    public class OrderEntity
    {
        public long Id { get; set; }

        public string OrderNumber { get; set; }

        public DateTimeOffset DateOfCreation { get; set; }

        public decimal Sum { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public virtual List<OrderItemEntity> OrderItems { get; set; }
    }
}
