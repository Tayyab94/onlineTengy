using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace onlineTengy.Models
{
    public class MenuItem
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }



        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Spicyness { get; set; }

        public enum ESpicyness { NA=0,Spicy=1,VerySpicy=2}


        [Range(1,int.MaxValue,ErrorMessage ="Price Should be Greater than $-{1}")]
        public double Price { get; set; }

        [Display(Name ="Catagory Id")]
        public int CatagoryId { get; set; }

        [ForeignKey("CatagoryId")]

        public virtual Catagory Catagory { get; set; }

        [Display(Name = "SubCatagory Id")]
        public int SubCatagoryId { get; set; }

        [ForeignKey("SubCatagoryId")]

        public virtual SubCatagory   SubCatagory { get; set; }

        
    }


}
