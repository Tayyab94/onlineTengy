using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineTengy.Data;
using onlineTengy.Models;
using onlineTengy.Models.HomeViewModels;
using onlineTengy.Models.MenuItemsViewModels;

namespace onlineTengy.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        public HomeController(ApplicationDbContext db)
        {
            _context = db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel indexView = new IndexViewModel()
            {
                //    MenuItems = await _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory).ToListAsync(),
                //    Catagories = _context.Catagories.OrderBy(c => c.DisplayOrder).ToList(),
                //    Coupons = _context.Coupons.Where(c => c.isActive == true).ToList()

                MenuItems = await _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory).ToListAsync(),
                Catagories = _context.Catagories.OrderBy(c => c.DisplayOrder),
                Coupons = _context.Coupons.Where(c => c.isActive == true).ToList()
            };
            return View(indexView);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //MenuItemViewModel menuItemView = new MenuItemViewModel();

            //menuItemView.MenuItem =await _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory).Where(x => x.Id == id).SingleOrDefaultAsync();

            //menuItemView.GetSubCatagory = await _context.SubCatagories.Where(x => x.CatagoryId.Equals(menuItemView.MenuItem.CatagoryId)).ToListAsync();

            //if(menuItemView==null)
            //{
            //    return NotFound();
            //}

            var MenuItmeDb = await _context.MenuItems.Include(c => c.Catagory).Include(c => c.SubCatagory).Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

            ShoppingCart shoppingCart = new ShoppingCart
            {
                MenuItem = MenuItmeDb,
                MenuItemId = MenuItmeDb.Id
            };

            return View(shoppingCart);
        }

        //this Method should be acccess when the user logedIn into our Site 
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;

            if(ModelState.IsValid)
            {
                var ClaimIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                shoppingCart.ApplicationUserId = claim.Value;

                ShoppingCart shoppingCartDb = await _context.ShoppingCarts.
                    Where(x => x.ApplicationUserId == shoppingCart.ApplicationUserId
                   && x.MenuItemId.Equals(shoppingCart.MenuItemId)).FirstOrDefaultAsync();

                if(shoppingCartDb==null)
                {
                    //THis Item does not exist

                    _context.ShoppingCarts.Add(shoppingCart);
                }
                else
                {
                    shoppingCartDb.count = shoppingCartDb.count + shoppingCart.count;
                }

                await _context.SaveChangesAsync();

                var countNo = _context.ShoppingCarts.Where(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count();


                HttpContext.Session.SetInt32("CartCount", countNo);
                //We will check the  session in two places 1 is form the new register user and  the 2nd is at the time of login 

                return RedirectToAction(nameof(Index));
                
            }
            else
            {

                var MenuItmeDb = await _context.MenuItems.Include(c => c.Catagory).Include(c => c.SubCatagory).Where(x => x.Id.Equals(shoppingCart.MenuItemId)).FirstOrDefaultAsync();

                ShoppingCart shoppingcart = new ShoppingCart
                {
                    MenuItem = MenuItmeDb,
                    MenuItemId = MenuItmeDb.Id
                };

                return View(shoppingcart);
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
