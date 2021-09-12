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
    }
}
