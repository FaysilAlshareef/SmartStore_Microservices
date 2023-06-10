﻿using System.ComponentModel.DataAnnotations;

namespace SmartStore.OrdersAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int OrderHeaderId { get; set; }
        

        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }

        public int Count { get; set; }
    }
}
