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
using SpicyMarket.Utility;

namespace SpicyMarket.Areas.Customer.Controllers
{
    [Area(SD.Customer)]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var orderDetalisVM = new OrderDetailsViewModel()
            {
                OrderHeader = await _context.OrderHeaders.Include(x => x.ApplicationUser)
                                                        .FirstOrDefaultAsync(x => x.UserId == claim.Value &&
                                                                                  x.Id ==id),
                OrderDetails =await _context.OrderDetails.Where(m=>m.OrderId == id).ToListAsync()
            };


            return View(orderDetalisVM);
        }

        [Authorize]
        public async Task<IActionResult> OrderHistory()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var orderDetailsVMList = new List<OrderDetailsViewModel>();

            var orderHeadersList = await _context.OrderHeaders.Include(x => x.ApplicationUser)
                                                          .Where(m => m.UserId == claim.Value)
                                                          .ToListAsync();
            foreach (var orderHeader in orderHeadersList)
            {
                var orderDetailsVM = new OrderDetailsViewModel()
                {
                    OrderHeader = orderHeader,
                    OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == orderHeader.Id)
                                                              .ToListAsync()
                };
                orderDetailsVMList.Add(orderDetailsVM);
            }
           
            return View(orderDetailsVMList);
        }
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var orderDetailsVM = new OrderDetailsViewModel()
            {
                OrderHeader = await _context.OrderHeaders.Include(x => x.ApplicationUser)
                                                      .FirstOrDefaultAsync(x => x.Id == id),
                OrderDetails = await _context.OrderDetails.Where(x => x.OrderId == id).ToListAsync()
            };
            return PartialView("_IndividualOrderDetalis", orderDetailsVM);
        }
    }
}
