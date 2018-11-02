using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineTengy.Data;
using System.IO;
using onlineTengy.Models;
using Microsoft.AspNetCore.Authorization;
using onlineTengy.Utility;

namespace onlineTengy.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]

    public class CouponController : Controller
    {
        private ApplicationDbContext _context;

        public CouponController(ApplicationDbContext db)
        {
            _context = db;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Coupons.ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupons model)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                // this line of Code is use to Store the Image into  the Database Not into the server

                if (files[0] != null && files[0].Length > 0)
                {
                    byte[] p1 = null;

                    using (var fs1 = files[0].OpenReadStream())  // using System.IO
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);

                            p1 = ms1.ToArray();
                        }

                    }

                    model.Pitcher = p1;

                    _context.Coupons.Add(model);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var couponobj =await _context.Coupons.SingleOrDefaultAsync(x => x.Id == id);

            if (couponobj == null)
            {
                return NotFound();
            }

            return View(couponobj);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Coupons model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var couponFromDB = await _context.Coupons.Where(x => x.Id == model.Id).FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                //this Code is Check either the user have change the image or Not
                if (files[0] != null && files[0].Length > 0)
                {
                    byte[] p1 = null;

                    using (var fs1 = files[0].OpenReadStream())  // using System.IO
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);

                            p1 = ms1.ToArray();
                        }

                    }

                    couponFromDB.Pitcher = p1;

                }
                couponFromDB.MinimumAmount = model.MinimumAmount;
                couponFromDB.Name = model.Name;
                couponFromDB.Discount = model.Discount;
                couponFromDB.MinimumAmount = model.MinimumAmount;
                couponFromDB.CouponType = model.CouponType;
                couponFromDB.isActive = model.isActive;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();

            }

            var couponobj = await _context.Coupons.SingleOrDefaultAsync(x => x.Id == id);

            if (couponobj == null)
            {
                return NotFound();
            }

            return View(couponobj);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCoupon(int? id)
        {
            var Coupon =await _context.Coupons.SingleOrDefaultAsync(x => x.Id.Equals(id));

            _context.Coupons.Remove(Coupon);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}