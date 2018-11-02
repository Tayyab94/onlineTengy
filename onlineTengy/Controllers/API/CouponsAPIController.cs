using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using onlineTengy.Data;
using onlineTengy.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace onlineTengy.Controllers.API
{
    [Route("api/[controller]")]
    public class CouponsAPIController : Controller
    {
        public readonly ApplicationDbContext _context;

        public CouponsAPIController(ApplicationDbContext d)
        {
            _context = d;
        }
        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(double totalorder, string CouponCode = null)
        {
            var rtn = "";

            if (CouponCode == null)
            {
                rtn = totalorder + ":E";

                return Ok(rtn);
            }


            var CouponFromDB = _context.Coupons.Where(c => c.Name == CouponCode).FirstOrDefault();

            if (CouponFromDB == null)
            {
                rtn = totalorder + ":E";

                return Ok(rtn);
            }

            //Check the Total Order must be greater than the Coupon_MinOrder 
            if (CouponFromDB.MinimumAmount > totalorder)
            {
                rtn = totalorder + ":E";

                return Ok(rtn);
            }

            if (Convert.ToInt32(CouponFromDB.CouponType) == (int)Coupons.ECouponsType.Dollar)
            {
                totalorder = totalorder - CouponFromDB.Discount;
                rtn = totalorder + ":S";

                return Ok(rtn);

            }
            else
            {
                if (Convert.ToInt32(CouponFromDB.CouponType) == (int)Coupons.ECouponsType.Percentage)
                {
                    totalorder = totalorder - (totalorder * CouponFromDB.Discount / 100);
                    rtn = totalorder + ":S";

                    return Ok(rtn);

                }
            }

            return Ok();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
