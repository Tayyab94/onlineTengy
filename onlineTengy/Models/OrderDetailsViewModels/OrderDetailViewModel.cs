using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models.OrderDetailsViewModels
{
    public class OrderDetailViewModel
    {
        public OrderHeader  OrderHeader { get; set; }

        public List<OrderDetails> OrderDetails { get; set; }
    }
}
