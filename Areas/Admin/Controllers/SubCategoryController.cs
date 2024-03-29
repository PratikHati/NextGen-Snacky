﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using NextGen_Snacky.Models.ViewModels;
using NextGen_Snacky.Utility;
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
            //anybody can view subcategory
            var subcategory = await _adb.SubCategory.Include(x => x.Category).ToListAsync();      //"Include(x=>x.Category)" to map the foreign key from Category table
            return View(subcategory);
        }

        //GET- Create
        public async Task<IActionResult> Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _adb.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _adb.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
            };

            return View(model);
        }

        //POST - Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel sub)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            if (ModelState.IsValid)
            {
                var subcategory = _adb.SubCategory.Include(x => x.Category).Where(x => x.Name == sub.SubCategory.Name && x.Category.Id == sub.SubCategory.CategoryId);
                if (subcategory.Count() > 0)
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
                StatusMessage = StatusMessage
            };

            return View(obj);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from SubCategory in _adb.SubCategory
                                   where SubCategory.CategoryId == id
                                   select SubCategory).ToListAsync();

            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        //GET- Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _adb.SubCategory.SingleOrDefaultAsync(x => x.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _adb.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _adb.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
            };

            return View(model);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubCategoryAndCategoryViewModel sub)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            if (ModelState.IsValid)
            {
                var subcategory = _adb.SubCategory.Include(x => x.Category).Where(x => x.Name == sub.SubCategory.Name && x.Category.Id == sub.SubCategory.CategoryId);
                if (subcategory.Count() > 0)
                {
                    //Error  "Error" will auto identified by ASP.NET Identity. Visit "_StatuMessage.cshtml" to see logic
                    StatusMessage = "Error : Sub-Category already exists in " + subcategory.First().Category.Name + " Category. Please select different Sub-Category";
                }
                else
                {
                    var subcat = await _adb.SubCategory.FindAsync(id);
                    subcat.Name = sub.SubCategory.Name;
                    await _adb.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            SubCategoryAndCategoryViewModel obj = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _adb.Category.ToListAsync(),
                SubCategory = sub.SubCategory,
                SubCategoryList = await _adb.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).ToListAsync(),
                StatusMessage = StatusMessage
            };

            //---------------------IMPORTANT--------------------------//

            //FIXED  Reason- after error message again reassign its subcategory id else next step(Edit()) will affect
            //as "id" will 0 if prev step was error 
            obj.SubCategory.Id = id;

            return View(obj);
        }

        //GET- Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _adb.SubCategory.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }



            return View(subCategory);
        }

        //GET- Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = await _adb.SubCategory.Include(x => x.Category).SingleOrDefaultAsync(x => x.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //POST - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            var subCategory = await _adb.SubCategory.SingleOrDefaultAsync(x => x.Id == id);

            if (subCategory == null)
            {
                return View();
            }

            _adb.SubCategory.Remove(subCategory);
            await _adb.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
