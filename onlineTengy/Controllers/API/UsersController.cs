using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using onlineTengy.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace onlineTengy.Controllers.API
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private ApplicationDbContext _context;

        public UsersController(ApplicationDbContext d)
        {
            _context = d;
        }


        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(string type,string query=null)
        {
            if (type.Equals("email") && query != null)
            {
                var CustomerQuery = _context.Users.Where(x => x.Email.ToLower().Contains(query.ToLower()));

                return Ok(CustomerQuery.ToList());
            }

            if (type.Equals("phone") && query != null)
            {
                var CustomerQuery = _context.Users.Where(x => x.PhoneNumber.ToLower().Contains(query.ToLower()));

                return Ok(CustomerQuery.ToList());
            }

            return Ok();
        }


        // We are Not Using these APIs Now 
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
