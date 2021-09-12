using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpicyMarket.Models.ViewModel
{
    public class OrderDetailsViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
