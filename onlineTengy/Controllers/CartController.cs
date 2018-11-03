using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using onlineTengy.Data;
using onlineTengy.Models;
using onlineTengy.Models.OrderDetailsViewModels;
using onlineTengy.Utility;

namespace onlineTengy.Controllers
{
    public class CartController : Controller
    {
        public readonly ApplicationDbContext _context;

        [BindProperty]
        public OrderDetailsCart detailsCart { get; set; }

        public CartController(ApplicationDbContext db)
        {
            _context = db;
        }
        public IActionResult Index()
        {
            detailsCart = new OrderDetailsCart
            {
                OrderHeader = new OrderHeader()
            };


            detailsCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var Claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _context.ShoppingCarts.Where(c => c.ApplicationUserId == Claim.Value);

            if (cart != null)
            {
                detailsCart.ListCart = cart.ToList();
            }

            foreach (var list in detailsCart.ListCart)
            {
                list.MenuItem = _context.MenuItems.Where(x => x.Id == list.MenuItemId).FirstOrDefault();

                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.count);

                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "---";
                }

            }

            detailsCart.OrderHeader.PickUpDate = DateTime.Now;

            return View(detailsCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexCreate()
        {

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var Claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCartList = _context.ShoppingCarts.Where(c => c.ApplicationUserId == Claim.Value).ToList();

            detailsCart.ListCart = shoppingCartList;

            detailsCart.OrderHeader.OrderDate = DateTime.Now;
            detailsCart.OrderHeader.UserId = Claim.Value;
            detailsCart.OrderHeader.Status = SD.StatusSubmitted;

            OrderHeader orderHeader = detailsCart.OrderHeader;


            _context.OrderHeaders.Add(orderHeader);
            _context.SaveChanges();

            foreach (var item in detailsCart.ListCart)
            {
                item.MenuItem = _context.MenuItems.Where(c => c.Id == item.MenuItemId).FirstOrDefault();


                OrderDetails orderDetails = new OrderDetails
                {
                    OrderId = detailsCart.OrderHeader.Id,
                    MenuItemId = item.MenuItemId,
                    Name = item.MenuItem.Name,
                    Description = item.MenuItem.Description,
                    Price = item.MenuItem.Price,
                    Count = item.count
                };
                
                _context.OrderDetails.Add(orderDetails);
            }

            _context.ShoppingCarts.RemoveRange(detailsCart.ListCart);

            _context.SaveChanges();

            HttpContext.Session.SetInt32("CartCount", 0);


            return RedirectToAction("Confirm", "Order", new { id = orderHeader.Id });

        }


        public IActionResult plus(int cartId)
        {
            var cart = _context.ShoppingCarts.Where(x => x.Id.Equals(cartId)).FirstOrDefault();

            cart.count += 1;
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult minus(int cartId)
        {
            var cart = _context.ShoppingCarts.Where(x => x.Id.Equals(cartId)).FirstOrDefault();

            try
            {


                if (cart.count == 1)
                {
                    _context.ShoppingCarts.Remove(cart);

                    _context.SaveChanges();

                    var cn = _context.ShoppingCarts.Where(x => x.ApplicationUserId.Equals(cart.ApplicationUserId)).ToList().Count();

                    HttpContext.Session.SetInt32("CartCount", cn);
                }
                else
                {

                    cart.count -= 1;
                    _context.SaveChanges();
                }
            }
            catch(Exception e)
            {

            }

            return RedirectToAction(nameof(Index));
        }
    }
}