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
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _adb;

        public CouponController(ApplicationDbContext adb)
        {
            _adb = adb; 
        }
        public async Task<IActionResult> Index()
        {
            return View(await _adb.Coupon.ToListAsync());
        }

    }
}
