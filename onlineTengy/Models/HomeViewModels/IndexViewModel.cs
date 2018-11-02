using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models.HomeViewModels
{
    public class IndexViewModel
    {

        public IEnumerable<MenuItem>  MenuItems { get; set; }

        public IEnumerable<Catagory>  Catagories { get; set; }
        public IEnumerable<Coupons> Coupons { get; set; }

        public string statusMessage { get; set; }
    }
}
