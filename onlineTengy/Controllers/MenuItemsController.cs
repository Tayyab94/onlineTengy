using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using onlineTengy.Data;
using onlineTengy.Models;
using onlineTengy.Models.MenuItemsViewModels;
using onlineTengy.Utility;

namespace onlineTengy.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IHostingEnvironment _hostingEnvirenment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }

        public MenuItemsController(ApplicationDbContext db, IHostingEnvironment hostingEnvironemnt)
        {
            _context = db;
            _hostingEnvirenment = hostingEnvironemnt;
            MenuItemVM = new MenuItemViewModel
            {
                Catagory = _context.Catagories.ToList(),
                MenuItem = new MenuItem()
            };
        }

        //Get MenuItems
        public async Task<IActionResult> Index()
        {
            var MenuItemList = _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory);
            return View(await MenuItemList.ToListAsync());
        }


        public IActionResult Create()
        {
            return View(MenuItemVM);
        }




        //This Method returns the CasaCadind Dropdown list of SubCatagory
        public JsonResult GETSubCatagoriesList(int catagoryID)
        {
            List<SubCatagory> SubcatagoriesList = new List<SubCatagory>();
            SubcatagoriesList = (from c in _context.SubCatagories
                                 where c.CatagoryId == catagoryID
                                 select c).ToList();

            return Json(new SelectList(SubcatagoriesList, "Id", "Name"));
        }

        //Post form MenuItem Method

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreatePost()
        {
            MenuItemVM.MenuItem.SubCatagoryId = Convert.ToInt32(Request.Form["SubCatagoryId"].ToString());

            if (!ModelState.IsValid)
            {
                //return RedirectToAction("Create", MenuItemVM);

                return View(MenuItemVM);
            }

            _context.MenuItems.Add(MenuItemVM.MenuItem);

            await _context.SaveChangesAsync();

            //Image Binding Save


            string webRootPath = _hostingEnvirenment.WebRootPath;
            //this line of code gives you all the file to tht the user have been uploaded
            var files = HttpContext.Request.Form.Files;

            var MenuItemFromDb = _context.MenuItems.Find(MenuItemVM.MenuItem.Id);

            if (files[0] != null && files[0].Length > 0)
            {
                var upload = Path.Combine(webRootPath, "MenuPostImages");

                //This line of code gives us the Exact extension of the file line png, jpeg etc...

                var extenstionfile = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));


                //Copy the File to our Folder in the Project 
                using (var filestream = new FileStream(Path.Combine(upload, MenuItemVM.MenuItem.Id + extenstionfile), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                MenuItemFromDb.ImageUrl = @"\MenuPostImages\" + MenuItemVM.MenuItem.Id + extenstionfile;

            }
            else
            {
                //When the User Does not Upload the files

                var upload = Path.Combine(webRootPath, @"MenuPostImages\" + SD.DefaultImage);

                System.IO.File.Copy(upload, webRootPath + @"\MenuPostImages\" + MenuItemVM.MenuItem.Id + ".png");
                MenuItemVM.MenuItem.ImageUrl = @"\MenuPostImages\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // MenuItems Edit Action Result
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
          
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory).SingleOrDefaultAsync(x => x.Id == id);

            MenuItemVM.GetSubCatagory = await _context.SubCatagories.Where(c => c.CatagoryId == MenuItemVM.MenuItem.CatagoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {

            if (id != MenuItemVM.MenuItem.Id)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem.SubCatagoryId = Convert.ToInt32(Request.Form["SubCatagoryId"].ToString());
            if (ModelState.IsValid)
            {


                try
                {
                    string webRootPath = _hostingEnvirenment.WebRootPath;

                    var files = HttpContext.Request.Form.Files;


                    var MenuItemsformDB = await _context.MenuItems.Include(c => c.Catagory).Include(c => c.SubCatagory).FirstOrDefaultAsync(x=>x.Id==id);


                    if (files[0].Length > 0 && files[0] != null)
                    {
                        //if the USer Upload the new image

                        var uploads = Path.Combine(webRootPath, "MenuPostImages");


                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                        var extension_Old = MenuItemsformDB.ImageUrl.Substring(MenuItemsformDB.ImageUrl.LastIndexOf("."), MenuItemsformDB.ImageUrl.Length - MenuItemsformDB.ImageUrl.LastIndexOf("."));

                        //Check the file exist in the folder or not 
                        if (System.IO.File.Exists(Path.Combine(uploads, MenuItemsformDB.ImageUrl + extension_Old)))
                        {
                            System.IO.File.Decrypt(Path.Combine(uploads, MenuItemsformDB.ImageUrl + extension_Old));
                        }


                        //Copy the File to our Folder in the Project 
                        using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_New), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }

                        MenuItemsformDB.ImageUrl = @"\MenuPostImages\" + MenuItemVM.MenuItem.Id + extension_New;
                    }
                    else
                    {
                        
                            MenuItemsformDB.ImageUrl = MenuItemVM.MenuItem.ImageUrl;

                    }
                    //if (MenuItemVM.MenuItem.ImageUrl != null)
                    //{
                    //    MenuItemsformDB.ImageUrl = MenuItemVM.MenuItem.ImageUrl;
                    //}
                    MenuItemsformDB.Name = MenuItemVM.MenuItem.Name;
                    MenuItemsformDB.Price = MenuItemVM.MenuItem.Price;
                    MenuItemsformDB.Description = MenuItemVM.MenuItem.Description;
                    MenuItemsformDB.CatagoryId = MenuItemVM.MenuItem.CatagoryId;
                    MenuItemsformDB.Spicyness = MenuItemVM.MenuItem.Spicyness;
                    MenuItemsformDB.SubCatagoryId = MenuItemVM.MenuItem.SubCatagoryId;

                    await _context.SaveChangesAsync();

                   
                }
                catch (Exception e)
                {


                }

                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.GetSubCatagory = _context.SubCatagories.Where(c => c.CatagoryId == MenuItemVM.MenuItem.CatagoryId).ToList();

            return View(MenuItemVM);
        }

        public async Task<IActionResult> Details(int ?id)
        {

            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory).SingleOrDefaultAsync(x => x.Id == id);

            MenuItemVM.GetSubCatagory = await _context.SubCatagories.Where(c => c.CatagoryId == MenuItemVM.MenuItem.CatagoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);

        }


        public async Task<IActionResult> Delete(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _context.MenuItems.Include(x => x.Catagory).Include(x => x.SubCatagory).SingleOrDefaultAsync(x => x.Id == id);

            MenuItemVM.GetSubCatagory = await _context.SubCatagories.Where(c => c.CatagoryId == MenuItemVM.MenuItem.CatagoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);

        }



        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>DeleteMenuItem(int id)
        {
            string webRootPath = _hostingEnvirenment.WebRootPath;

            MenuItem menuItem = _context.MenuItems.Find(id);

            if(menuItem!=null)
            {
                var upload = Path.Combine(webRootPath, "MenuPostImages");

                var extension = menuItem.ImageUrl.Substring(menuItem.ImageUrl.LastIndexOf("."), menuItem.ImageUrl.Length - menuItem.ImageUrl.LastIndexOf("."));


                var imageDirectory = Path.Combine(upload, menuItem.Id + extension);
                if(System.IO.File.Exists(imageDirectory))
                {
                    System.IO.File.Delete(imageDirectory);
                }

                _context.MenuItems.Remove(menuItem);

                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }


    }
}