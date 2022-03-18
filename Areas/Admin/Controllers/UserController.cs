using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NextGen_Snacky.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _adb;

        public UserController(ApplicationDbContext adb)
        {
            _adb = adb;
        }
        public async Task<IActionResult> Index()
        { 
            var identity = (ClaimsIdentity)this.User.Identity;      //this tecnique used to get current logged in user role (Look StackOverflow)

            var claim = identity.FindFirst(ClaimTypes.NameIdentifier);      //null if user not logged in

            //retreive all role except current role 
            return View(await _adb.ApplicationUser.Where(x =>x.Id != claim.Value).ToListAsync());
        }
    }
}
