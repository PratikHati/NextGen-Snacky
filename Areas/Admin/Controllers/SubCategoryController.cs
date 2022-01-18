using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _adb;
        
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _adb = db;
        }
       
        //GET - Index
        public async Task<IActionResult> Index()
        {
            var subcategory = await _adb.SubCategory.Include(x=>x.Category).ToListAsync();      //"Include(x=>x.Category)" to map the foreign key from Category table
            return View(subcategory);
        }

        //GET- Create
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _adb.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _adb.SubCategory.OrderBy(x=>x.Name).Select(x => x.Name).Distinct().ToListAsync(),
            };

            return View(model);
        }

        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel sub)
        {
            if (ModelState.IsValid)
            {
                var subcategory = _adb.SubCategory.Include(x => x.Category).Where(x => x.Name == sub.SubCategory.Name && x.Category.Id == sub.SubCategory.CategoryId);
                if(subcategory.Count() > 0)
                {
                    //Error  "Error" will auto identified by ASP.NET Identity. Visit "_StatuMessage.cshtml" to see logic
                    StatusMessage = "Error : Sub-Category already exists in " + subcategory.First().Category.Name + " Category. Please select different Sub-Category";
                }
                else
                {
                    //valid
                    _adb.SubCategory.Add(sub.SubCategory);
                    await _adb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            SubCategoryAndCategoryViewModel obj = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _adb.Category.ToListAsync(),
                SubCategory = sub.SubCategory,
                SubCategoryList = await _adb.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).ToListAsync(),
                StatusMessage=StatusMessage
            };

            return View(obj);
        }
    }
}
