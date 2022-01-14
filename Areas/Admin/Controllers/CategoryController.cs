using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _adb;
        public CategoryController(ApplicationDbContext db)
        {
            _adb = db;
        }

        //GET Category list
        public async Task<IActionResult> Index()
        {
            return View(await _adb.Category.ToListAsync());
        }

        //GET create and it will not return any retrived element 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _adb.Category.Add(category);
                await _adb.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        //GET - Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var category = await _adb.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _adb.Category.Update(category);
                await _adb.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }
    }
}
