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
    [Authorize]
    public class CartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public OrderDetailsCartViewModel orderDetailsCartVM { get; set; }
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

            if (shoppingCarts != null)
            {
                orderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach (var item in orderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);
                orderDetailsCartVM.OrderHeader.OrderTotal = orderDetailsCartVM.OrderHeader.OrderTotal + (item.MenuItem.Price * item.Count);

                orderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(orderDetailsCartVM.OrderHeader.OrderTotal, 2);
                if (item.MenuItem.Description.Length > 75)
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
            if (couponCode == null)
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


        public async Task<IActionResult> Plus(int cartId)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(cartId);
            shoppingCart.Count += 1;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(cartId);
            if (shoppingCart.Count > 1)
            {
                shoppingCart.Count -= 1;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(cartId);

            _context.ShoppingCarts.Remove(shoppingCart);
            await _context.SaveChangesAsync();

            var count = _context.ShoppingCarts.Where(x => x.ApplicationUserId == shoppingCart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);

            return RedirectToAction(nameof(Index));
        }


        #region lec 10 
        public IActionResult Summary()
        {
            orderDetailsCartVM = new OrderDetailsCartViewModel()
            {
                OrderHeader = new Models.OrderHeader()
            };
            orderDetailsCartVM.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var appUser = _context.ApplicationUsers.Find(claim.Value);
            orderDetailsCartVM.OrderHeader.PickUpName = appUser.Name;
            orderDetailsCartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
            orderDetailsCartVM.OrderHeader.PickUpTime = DateTime.Now;


            var shoppingCarts = _context.ShoppingCarts.
                                                        Where(x => x.ApplicationUserId == claim.Value);

            if (shoppingCarts != null)
            {
                orderDetailsCartVM.ShoppingCartsList = shoppingCarts.ToList();
            }

            foreach (var item in orderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);
                orderDetailsCartVM.OrderHeader.OrderTotalOrginal  += (item.MenuItem.Price * item.Count);

                orderDetailsCartVM.OrderHeader.OrderTotal = Math.Round(orderDetailsCartVM.OrderHeader.OrderTotalOrginal, 2);

            }
            orderDetailsCartVM.OrderHeader.OrderTotalOrginal = orderDetailsCartVM.OrderHeader.OrderTotal;
             HttpContext.Session.GetString(SD.ssCouponCode);
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                orderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDB = _context.Coupons.Where(x =>
                                            x.Name.ToLower() == orderDetailsCartVM.OrderHeader.CouponCode.ToLower())
                                                   .FirstOrDefault();

                orderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDB, orderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }

            return View(orderDetailsCartVM);
        }
        #endregion

        #region lec 11 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost()
        {
           
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            orderDetailsCartVM.ShoppingCartsList =await _context.ShoppingCarts.
                                                        Where(x => x.ApplicationUserId == claim.Value).ToListAsync();
            //set Order Header proparety
            orderDetailsCartVM.OrderHeader.PaymentStatus = SD.PaymetStatuPending;
            orderDetailsCartVM.OrderHeader.OrderDate = DateTime.Now;
            orderDetailsCartVM.OrderHeader.UserId = claim.Value;
            orderDetailsCartVM.OrderHeader.Status = SD.PaymetStatuPending;
            orderDetailsCartVM.OrderHeader.PickUpTime = Convert.ToDateTime
                                        (orderDetailsCartVM.OrderHeader.PickUpDate.ToShortDateString()
                                        + " " + 
                                        orderDetailsCartVM.OrderHeader.PickUpTime.ToShortTimeString());
            orderDetailsCartVM.OrderHeader.OrderTotalOrginal = 0;
            //
            _context.OrderHeaders.Add(orderDetailsCartVM.OrderHeader);
            await _context.SaveChangesAsync();


            foreach (var item in orderDetailsCartVM.ShoppingCartsList)
            {
                item.MenuItem = _context.MenuItems.FirstOrDefault(x => x.Id == item.MenuItemId);
                var orderDetails = new OrderDetail()
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderDetailsCartVM.OrderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count

                };
                orderDetailsCartVM.OrderHeader.OrderTotalOrginal  += (item.MenuItem.Price * item.Count);
                _context.OrderDetails.Add(orderDetails);
                await _context.SaveChangesAsync();

            }
            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                orderDetailsCartVM.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDB = _context.Coupons.Where(x =>
                                            x.Name.ToLower() == orderDetailsCartVM.OrderHeader.CouponCode.ToLower())
                                                   .FirstOrDefault();

                orderDetailsCartVM.OrderHeader.OrderTotal = SD.DiscountPrice(couponFromDB, orderDetailsCartVM.OrderHeader.OrderTotalOrginal);
            }
            else
            {
                orderDetailsCartVM.OrderHeader.OrderTotal =Math.Round(orderDetailsCartVM.OrderHeader.OrderTotalOrginal, 2);
            }
            orderDetailsCartVM.OrderHeader.CouponCodeDiscount =
                   orderDetailsCartVM.OrderHeader.OrderTotalOrginal - orderDetailsCartVM.OrderHeader.OrderTotal;

            // After all process of Pay the cart list item we must empty the shopping cart
             _context.ShoppingCarts.RemoveRange(orderDetailsCartVM.ShoppingCartsList);
            HttpContext.Session.SetInt32(SD.ShoppingCartCount, 0);
            await _context.SaveChangesAsync();



            return RedirectToAction("Index", "Home");
        }
        #endregion

    }
}
