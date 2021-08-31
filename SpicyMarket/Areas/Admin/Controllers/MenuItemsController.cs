using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models;
using SpicyMarket.Models.ViewModel;

namespace SpicyMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        [BindProperty]
        public  MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                MenuItem = new MenuItem(),
                CategoriesList = _context.Categories.ToList()
            };
        }
        public async Task< IActionResult> Index()
        {
            var menuItems = await _context.MenuItems.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync();
            return View(menuItems);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                string imagePath = @"/images/food-image.png/";
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string imageName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    var fileStream = new FileStream(Path.Combine(webRootPath, "images", imageName),FileMode.Create);
                    files[0].CopyTo(fileStream);
                    imagePath = @"/images/"+ imageName;
                }
                MenuItemVM.MenuItem.Image = imagePath;
                _context.MenuItems.Add(MenuItemVM.MenuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemVM);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var menuItem =  _context.MenuItems.Include(x => x.Category)
                                              .Include(x => x.SubCategory)
                                              .SingleOrDefault(x=>x.Id == id);

            if (menuItem == null) return NotFound();

            MenuItemVM.MenuItem = menuItem;

            MenuItemVM.SubCategoriesList = await _context.SubCategories
                                                .Where(x => x.CategoryId == MenuItemVM.MenuItem.Id)
                                                .ToListAsync();


            return View(MenuItemVM);
        }

        [HttpPost]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost()
        {
            if (ModelState.IsValid)
            {
           
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string imageName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    var fileStream = new FileStream(Path.Combine(webRootPath, "images", imageName), FileMode.Create);
                    files[0].CopyTo(fileStream);
                    string imagePath = @"/images/" + imageName;
                    MenuItemVM.MenuItem.Image = imagePath;
                }
               
                _context.MenuItems.Update(MenuItemVM.MenuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(MenuItemVM);
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = _context.MenuItems.Include(x => x.Category)
                                              .Include(x => x.SubCategory)
                                              .SingleOrDefault(x => x.Id == id);

            if (menuItem == null) return NotFound();

            MenuItemVM.MenuItem = menuItem;

            MenuItemVM.SubCategoriesList = await _context.SubCategories
                                                .Where(x => x.CategoryId == MenuItemVM.MenuItem.Id)
                                                .ToListAsync();


            return View(MenuItemVM);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var menuItem = _context.MenuItems.Include(x => x.Category)
                                              .Include(x => x.SubCategory)
                                              .SingleOrDefault(x => x.Id == id);

            if (menuItem == null) return NotFound();

            MenuItemVM.MenuItem = menuItem;

            MenuItemVM.SubCategoriesList = await _context.SubCategories
                                                .Where(x => x.CategoryId == MenuItemVM.MenuItem.Id)
                                                .ToListAsync();


            return View(MenuItemVM);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost()
        {
            /*if (ModelState.IsValid)
            {

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string imageName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(files[0].FileName);
                    var fileStream = new FileStream(Path.Combine(webRootPath, "images", imageName), FileMode.Create);
                    files[0].CopyTo(fileStream);
                    string imagePath = @"/images/" + imageName;
                    MenuItemVM.MenuItem.Image = imagePath;
                }*/

                _context.MenuItems.Remove(MenuItemVM.MenuItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
           
        }


    }
}
