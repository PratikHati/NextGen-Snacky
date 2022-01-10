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
    }
}
