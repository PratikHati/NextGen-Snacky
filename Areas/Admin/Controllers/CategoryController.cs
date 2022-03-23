using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using NextGen_Snacky.Utility;
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
            if(User.Identity.IsAuthenticated)                       //Only authorized user allowed
                return View(await _adb.Category.ToListAsync());

            return NoContent();
        }

        //GET create and it will not return any retrived element 
        public IActionResult Create()                               //write is not allowd to customer user
        {
            if (User.IsInRole(SD.CustomerUser))
                return NoContent();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            if (ModelState.IsValid)
            {
                _adb.Category.Add(category);
                await _adb.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        //GET - Edit
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            if (id==null)
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
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            if (ModelState.IsValid)
            {
                _adb.Category.Update(category);
                await _adb.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            if (id == null)
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

        //ActionName("Delete") is used to deferentiate it from above method i.e "public async Task<IActionResult> Delete(int? id)"
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }
            var category = await _adb.Category.FindAsync(id);

            if (category == null)
            {
                return View();
            }

            _adb.Category.Remove(category);
            await _adb.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        //GET -Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
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
    }
}
