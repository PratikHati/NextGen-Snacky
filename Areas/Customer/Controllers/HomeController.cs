using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using NextGen_Snacky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _adb;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext adb)
        {
            _logger = logger;
            _adb = adb;         //dependency injection
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync(),
                Category = await _adb.Category.ToListAsync(),
                Coupon = await _adb.Coupon.Where(x => x.IsActive == true).ToListAsync()     //Fixed "isActive"
            };
            return View(IndexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
