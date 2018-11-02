using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using onlineTengy.Data;
using onlineTengy.Models;
using onlineTengy.Models.SubCatagoryViewModel;
using onlineTengy.Utility;

namespace onlineTengy.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class SubCagtagoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        [TempData]
        public String statusMessage { get; set; }
        public SubCagtagoriesController(ApplicationDbContext db)
        {
            _context = db;
        }
        public async Task<IActionResult> Index()
        {
            var SubCatagoryList = _context.SubCatagories.Include(x => x.Catagory);
            return View(await SubCatagoryList.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            SubCatagoryAndCatagoryViewModel model = new SubCatagoryAndCatagoryViewModel()
            {
                catagoriesList = _context.Catagories.ToList(),
                subCatagory = new SubCatagory(),
                SubCatagoryList = _context.SubCatagories.OrderBy(c => c.Name).Select(c => c.Name).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(SubCatagoryAndCatagoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var DoesSubcatagoryExist = _context.SubCatagories.Where(x => x.Name == model.subCatagory.Name).Count();

                var DoesSubcatagoryAndCatagoryExist = _context.SubCatagories.Where(x => x.Name == model.subCatagory.Name && x.CatagoryId == model.subCatagory.CatagoryId).Count();

                if (DoesSubcatagoryExist > 0 && model.IsNew)
                {
                    //Eror

                    statusMessage = "Error : Sub Catagory Name is Already exist";
                }
                else
                {
                    if (DoesSubcatagoryExist == 0 && !model.IsNew)
                    {
                        //error

                        statusMessage = "Error : Sub Catagory Name doesn't exist";
                    }

                    else
                    {
                        if (DoesSubcatagoryAndCatagoryExist > 0)
                        {
                            //Error


                            statusMessage = "Error : Sub Catagory and Catagory Combination already exist";
                        }
                        else
                        {
                            _context.SubCatagories.Add(model.subCatagory);
                            await _context.SaveChangesAsync();

                            return RedirectToAction(nameof(Index));
                        }
                    }
                }
            }

            SubCatagoryAndCatagoryViewModel model1 = new SubCatagoryAndCatagoryViewModel()
            {
                catagoriesList = _context.Catagories.ToList(),
                subCatagory = model.subCatagory,
                SubCatagoryList = _context.SubCatagories.OrderBy(c => c.Name).Select(c => c.Name).ToList(),
                statusMessage = statusMessage
            };

            return View(model1);

        }


        //Action/ SubCatagory Edit /id

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            //var claimId =(ClaimsIdentity)this.User.Identity;

            //var claim = claimId.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

            //var use=_context.ApplicationUsers.Where(c=>c.Id!=claim.Value).ToList();


            if (id == null)
            {
                return NotFound();
            }
            var SubcatagoryList = await _context.SubCatagories.FirstOrDefaultAsync(i => i.Id == id);
            if (SubcatagoryList == null)
            {
                return NotFound();
            }

            SubCatagoryAndCatagoryViewModel model = new SubCatagoryAndCatagoryViewModel
            {
                catagoriesList = _context.Catagories.ToList(),
                subCatagory = SubcatagoryList,
                SubCatagoryList = _context.SubCatagories.Select(p => p.Name).Distinct().ToList()
            };


            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, SubCatagoryAndCatagoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCatagoryExist = _context.SubCatagories.Where(x => x.Name == model.subCatagory.Name).Count();

                var doesSubCataAndCatagoryExist = _context.SubCatagories.Where(x => x.Name == model.subCatagory.Name && x.CatagoryId == model.subCatagory.CatagoryId).Count();

                if (doesSubCatagoryExist > 0)
                {
                    statusMessage = "Error :Subcatagory does not exist, You can not add new subCaagory here";

                }
                else
                {
                    if (doesSubCataAndCatagoryExist > 0)
                    {
                        statusMessage = "Error :SubCatagory and Catagory Combination have already exist";
                    }
                    else
                    {
                        var subcat = _context.SubCatagories.Find(id);

                        subcat.Name = model.subCatagory
                            .Name;
                        subcat.CatagoryId = model.subCatagory.CatagoryId;

                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            SubCatagoryAndCatagoryViewModel obj = new SubCatagoryAndCatagoryViewModel
            {
                catagoriesList = _context.Catagories.ToList(),
                subCatagory = model.subCatagory,
                SubCatagoryList = _context.SubCatagories.Select(p => p.Name).Distinct().ToList(),
                statusMessage = statusMessage
            };


            return View(obj);
        }


        //Detail SubCatagory Action/id
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCatagory = await _context
                .SubCatagories.Include(c => c.Catagory).FirstOrDefaultAsync(c => c.Id == id);

            if (subCatagory == null)
            {
                return NotFound();
            }

            return View(subCatagory);
        }

        //Delete Method of SubCatagoryController/id

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCatagory = await _context
                .SubCatagories.Include(c => c.Catagory).FirstOrDefaultAsync(c => c.Id == id);

            if (subCatagory == null)
            {
                return NotFound();
            }

            return View(subCatagory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSubCatagory(int? id)
        {
            var subCatagory = await _context.SubCatagories.SingleOrDefaultAsync(x => x.Id == id);

            _context.SubCatagories.Remove(subCatagory);

            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Index));

        }
    }
}