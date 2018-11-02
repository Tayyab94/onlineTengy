using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models.SubCatagoryViewModel
{
    public class SubCatagoryAndCatagoryViewModel
    {
        public SubCatagory subCatagory { get; set; }

        public IEnumerable<Catagory> catagoriesList { get; set; }


        public List<String> SubCatagoryList { get; set; }

        [Display(Name = "new sub Catagory")]
        public bool IsNew { get; set; }


        public string statusMessage
        {
            get; set;
        }

    }
}

