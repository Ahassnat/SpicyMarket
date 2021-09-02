using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models;
using SpicyMarket.Models.ViewModel;

namespace SpicyMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var indexViewModel = new IndexViewModel()
            {
                Categories = await _context.Categories.ToListAsync(),
                Coupons = await _context.Coupons.Where(x=>x.IsActive == true) // to show the active coupon Only 
                                                .ToListAsync(), 
                MenuItems = await _context.MenuItems.Include(x => x.Category)
                                                    .Include(x => x.SubCategory)
                                                    .ToListAsync()
            };
            return View(indexViewModel);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItem = await _context.MenuItems.Include(x => x.Category)
                                                   .Include(x => x.SubCategory)
                                                   .Where(x => x.Id == id)
                                                   .FirstOrDefaultAsync();

            var shoppinCart = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };

            return View(shoppinCart);
        }

       
    }
}
