using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models;

namespace SpicyMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CouponsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task< IActionResult > Index()
        {
            var coupons = await _context.Coupons.ToListAsync();
            return View(coupons);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; // store update file
                if(files.Count> 0)
                {
                    byte[] picture = null;
                    var fileStream = files[0].OpenReadStream();
                    var memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    picture = memoryStream.ToArray();
                    coupon.Picture = picture;
                }
                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }



        [HttpGet]
        public async Task< IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null) return NotFound();

            return View(coupon);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files; // store update file
                if (files.Count > 0)
                {
                    byte[] picture = null;
                    var fileStream = files[0].OpenReadStream();
                    var memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    picture = memoryStream.ToArray();
                    coupon.Picture = picture;
                }
                _context.Coupons.Update(coupon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coupon);
        }
    }
}
