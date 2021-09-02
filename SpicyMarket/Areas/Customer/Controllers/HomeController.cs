using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models;
using SpicyMarket.Models.ViewModel;
using SpicyMarket.Utility;

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
            //set session 
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim != null)
            {
                var shoppingCart = await _context.ShoppingCarts
                                                .Where(x => x.ApplicationUserId == claim.Value)
                                                .ToListAsync();
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, shoppingCart.Count);
            }


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
        public async Task<IActionResult> Details(int itemId)
        {
            var menuItem = await _context.MenuItems.Include(x => x.Category)
                                                   .Include(x => x.SubCategory)
                                                   .Where(x => x.Id == itemId)
                                                   .FirstOrDefaultAsync();

            var shoppinCart = new ShoppingCart()
            {
                MenuItem = menuItem,
                MenuItemId = menuItem.Id
            };

            return View(shoppinCart);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                // for number of menu items for detective user  
                var shoppingCartFromDb = await _context.ShoppingCarts
                                                        .Where(x =>
                                                           x.ApplicationUserId == shoppingCart.ApplicationUserId &&
                                                           x.MenuItemId == shoppingCart.MenuItemId)
                                                        .FirstOrDefaultAsync();

                if (shoppingCartFromDb == null) // means => add to count value
                {
                    _context.ShoppingCarts.Add(shoppingCart);
                }
                else // mean => update the count value 
                {
                    //shoppingCartFromDb.Count = shoppingCartFromDb.Count + shoppingCart.Count;
                    shoppingCartFromDb.Count += shoppingCart.Count;
                }

                await _context.SaveChangesAsync();
                // # of items in shopping cart for logedin  user 
                var count = _context.ShoppingCarts.Where(x =>
                                                        x.ApplicationUserId == shoppingCart.ApplicationUserId)
                                                  .ToList().Count;
                // save count for user in session
                HttpContext.Session.SetInt32(SD.ShoppingCartCount,count);

                return RedirectToAction(nameof(Index));

            }
            else
            {
                var menuItem = await _context.MenuItems.Include(x => x.Category)
                                                  .Include(x => x.SubCategory)
                                                  .Where(x => x.Id == shoppingCart.MenuItemId)
                                                  .FirstOrDefaultAsync();

                var shoppinCartObj = new ShoppingCart()
                {
                    MenuItem = menuItem,
                    MenuItemId = menuItem.Id
                };

                return View(shoppinCartObj);
            }
        }
    }
}
