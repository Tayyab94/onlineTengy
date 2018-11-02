using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models
{
    public class SubCatagory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Sub Catagory")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Catagory Id")]
        public int CatagoryId { get; set; }


        [ForeignKey("CatagoryId")]
        public virtual Catagory Catagory { get; set; }
    }
}
