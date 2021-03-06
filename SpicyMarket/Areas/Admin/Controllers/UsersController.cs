using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Utility;

namespace SpicyMarket.Areas.Admin.Controllers
{
    [Authorize(Roles =SD.ManagerUser)]
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity; // User is existing proparity to check if user make login or not 
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;// return for userId how is make login 
            return View(await _context.ApplicationUsers.Where(x=>x.Id != userId).ToListAsync());
        }

        public async Task<IActionResult> LockUnLock( string? id)
        {
            if (id == null) return NotFound();
            var user = await _context.ApplicationUsers.FindAsync(id);
            if (user == null) return NotFound();

            if(user.LockoutEnd == null || user.LockoutEnd  < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        }
}
