using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models.MenuItemsViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }


        public IEnumerable<Catagory> Catagory { get; set; }

        public IEnumerable<SubCatagory> GetSubCatagory { get; set; }

        public int AddCount { get; set; }
    }
}
