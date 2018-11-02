using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models
{
    public class PageInfo
    {
        public int TotalItem { get; set; }

        public int  ItemPerPage { get; set; }

        public int CurrentPage { get; set; }


        //This will always fetch the TotalNo of pages show onto the bottom of the Page
        public int TotalPage => (int)Math.Ceiling((decimal)TotalItem / ItemPerPage);
    }
}
