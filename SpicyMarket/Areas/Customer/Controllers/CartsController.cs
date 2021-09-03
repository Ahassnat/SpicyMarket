using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models.ViewModel;

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
                orderDetailsCartVM.OrderHeader.OrderTotal =
                                    orderDetailsCartVM.OrderHeader.OrderTotal + (item.MenuItem.Price * item.Count);
            }
            orderDetailsCartVM.OrderHeader.OrderTotalOrginal = orderDetailsCartVM.OrderHeader.OrderTotal;
            

            return View(orderDetailsCartVM);
        }
    }
}
