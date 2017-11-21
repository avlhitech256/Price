using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Common.Data.Enum;
using DataBase.Context.Object;

namespace DataBase.Context.Entities
{
    public class OrderEntity
    {
        public long Id { get; set; }

        [MaxLength(30)]
        public string OrderNumber { get; set; }

        public DateTimeOffset DateOfCreation { get; set; }

        public decimal Sum { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }

        public virtual List<BasketItemEntity> BasketItems { get; set; }

        [MaxLength(1024)]
        public string Comment { get; set; }
    }
}
