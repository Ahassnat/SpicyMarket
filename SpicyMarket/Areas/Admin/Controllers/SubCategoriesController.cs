using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SpicyMarket.Data;
using SpicyMarket.Models;
using SpicyMarket.Models.ViewModel;

namespace SpicyMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task< IActionResult> Index()
        {
            var categories = await _context.SubCategories.Include(x => x.Category).ToListAsync();
            return View(categories);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CategoryAndSubCategoryViewModel
            {
                CategoriesList = await _context.Categories.ToListAsync(),
                SubCategory = new Models.SubCategory(),
             //   SubCategories = await _context.SubCategories.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()
            };
            return View(model);
        }
    
        [HttpPost]
        public async Task<IActionResult> Create(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doseExist = await _context.SubCategories.Include(x => x.Category)
                                        .Where(x => x.Category.Id == model.SubCategory.CategoryId &&
                                                    x.Name == model.SubCategory.Name).ToListAsync();
                if (doseExist.Count() > 0)
                {
                    StatusMessage = "Error : this is Sub Category Exist Under " + doseExist.FirstOrDefault().Category.Name;
                }
                else
                {
                      _context.SubCategories.Add(model.SubCategory);
                      await _context.SaveChangesAsync();
                      return RedirectToAction(nameof(Index));
                }
               
            }
            var modelVM = new CategoryAndSubCategoryViewModel
            {
                CategoriesList = await _context.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
               // SubCategories = await _context.SubCategories.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage

            };
            return View(modelVM);
        }
        [HttpGet]
        public async Task<IActionResult> GetSupCategoriesJson(int id)
        {
            // used List cuz we used Json Return Value 
            var subCategories = new List<SubCategory>();
            subCategories = await _context.SubCategories.Where(x => x.CategoryId == id).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        [HttpGet]
        public async Task<IActionResult> Edit (int ? id)
        {
            if (id == null) return NotFound();

            var subCategories = await _context.SubCategories.FindAsync(id);

            if (subCategories == null) return NotFound();

            var model = new CategoryAndSubCategoryViewModel
            {
                CategoriesList = await _context.Categories.ToListAsync(),
                SubCategory = subCategories,
                //   SubCategories = await _context.SubCategories.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryAndSubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doseExist = await _context.SubCategories.Include(x => x.Category)
                                        .Where(x => x.Category.Id == model.SubCategory.CategoryId &&
                                                    x.Name == model.SubCategory.Name &&
                                                    x.Id != model.SubCategory.Id).ToListAsync();
                if (doseExist.Count() > 0)
                {
                    StatusMessage = "Error : this is Sub Category Exist Under " + doseExist.FirstOrDefault().Category.Name;
                }
                else
                {
                    _context.SubCategories.Update(model.SubCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

            }
            var modelVM = new CategoryAndSubCategoryViewModel
            {
                CategoriesList = await _context.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                // SubCategories = await _context.SubCategories.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage

            };
            return View(modelVM);
        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var subCategories = _context.SubCategories.Include(x => x.Category).Where(x => x.Id == id).SingleOrDefault();

            if (subCategories == null) return NotFound();

           
            return View(subCategories);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SubCategory subCategory)
        {
            _context.SubCategories.Remove(subCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();

            var subCategories = _context.SubCategories.Include(x => x.Category).Where(x => x.Id == id).SingleOrDefault();

            if (subCategories == null) return NotFound();


            return View(subCategories);
        }
    }
}
