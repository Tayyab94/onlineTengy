using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineTengy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace onlineTengy.ViewComponents
{
    public class UserNameViewComponent:ViewComponent
    {
        private ApplicationDbContext _context;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _context = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claim = (ClaimsIdentity)this.User.Identity;
            var cl = claim.FindFirst(ClaimTypes.NameIdentifier);

            var UserDb = await _context.Users.Where(x => x.Id == cl.ValueType.ToString()).FirstOrDefaultAsync();

            return View(UserDb);
        }
    }
}
