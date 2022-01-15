using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
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
    }
}
