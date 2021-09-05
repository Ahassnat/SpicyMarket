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
using SpicyMarket.Models.ViewModel;
using SpicyMarket.Utility;

namespace SpicyMarket.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public OrderDetailsCartViewModel  orderDetailsCartVM { get; set; }
        public IActionResult Index()
        {
            orderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };
            orderDetailsCartVM.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var shoppingCarts = _context.ShoppingCarts.
                                                        Where(x => x.ApplicationUserId == claim.Value);
                                                        
            if(shoppingCarts != null)
            {
                orderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach (var item in orderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);
                orderDetailsCartVM.OrderHeader.OrderTotal = orderDetailsCartVM.OrderHeader.OrderTotal + (item.MenuItem.Price * item.Count);

                orderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(orderDetailsCartVM.OrderHeader.OrderTotal, 2);
                if(item.MenuItem.Description.Length > 75)
                {
                    item.MenuItem.Description = item.MenuItem.Description.Substring(0, 74);
                }
            
            }
            orderDetailsCartVM.OrderHeader.OrderTotalOrginal = orderDetailsCartVM.OrderHeader.OrderTotal;
            var CouponCodeSessionValue = HttpContext.Session.GetString(SD.ssCouponCode);
            if (CouponCodeSessionValue != null)
            {
                orderDetailsCartVM.OrderHeader.CouponCode = CouponCodeSessionValue;
                var couponFromDB = _context.Coupons.Where(x => 
                                            x.Name.ToLower() == orderDetailsCartVM.OrderHeader.CouponCode.ToLower())
                                                   .FirstOrDefault();

                orderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDB, orderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }

            return View(orderDetailsCartVM);
        }

        public IActionResult ApplyCoupon() 
        {
            var couponCode = orderDetailsCartVM.OrderHeader.CouponCode;
            if(couponCode == null)
            {
                couponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, couponCode);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            //var couponCode = orderDetailsCartVM.OrderHeader.CouponCode;
            
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));
        }
    }
}
