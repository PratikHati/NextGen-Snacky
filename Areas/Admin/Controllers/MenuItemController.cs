using Microsoft.AspNetCore.Hosting;
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
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _adb;
        private readonly IHostingEnvironment _hosting;

        [BindProperty]
        public MenuItemViewModel _MenuItemViewModel { get; set; }

        public MenuItemController(ApplicationDbContext adb, IHostingEnvironment hosting)
        {
            _adb = adb;
            _hosting = hosting;
        }
        public async Task<IActionResult> Index()
        {
            var menu = await _adb.MenuItem.Include(x=>x.Category).Include(x=>x.SubCategory).ToListAsync();
            return View(menu);
        }

    }
}
