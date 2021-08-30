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

        [HttpGet]
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
    }
}
