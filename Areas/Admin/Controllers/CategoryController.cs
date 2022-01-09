using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
        }
    }
}
