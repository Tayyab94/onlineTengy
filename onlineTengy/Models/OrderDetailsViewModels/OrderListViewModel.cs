using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models.OrderDetailsViewModels
{
    public class OrderListViewModel
    {
       public List<OrderDetailViewModel> orders { get; set; }

        public PageInfo  PageInfo { get; set; }

    }
}
