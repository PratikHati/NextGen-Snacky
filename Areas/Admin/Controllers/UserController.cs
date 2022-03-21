using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Utility;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if(User.Identity.IsAuthenticated)
            {
                var identity = (ClaimsIdentity)this.User.Identity;      //this tecnique used to get current logged in user role (Look StackOverflow)

                var claim = identity.FindFirst(ClaimTypes.NameIdentifier);      //null if user not logged in

                if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.KitchenUser) || User.IsInRole(SD.FrontDeskUser))                  //(2nd level security)Customer user should not able to see Index() of this UserController (SECURITY AND PRIVACY)
                {
                    return NoContent();                     //don't use UnAuthorized() as it will behave as broken link. Rather use NoContent()
                }

                //retreive all role except current role 
                return View(await _adb.ApplicationUser.Where(x => x.Id != claim.Value).ToListAsync());
            }
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _adb.ApplicationUser.Where(x => x.Id == id).FirstOrDefaultAsync();

            if(user == null)
            {
                return NotFound();
            }

            //if found lock it
            user.LockoutEnd = DateTime.Now.AddYears(100);       //Lockout will end in 100 years

            await _adb.SaveChangesAsync();      //also save in db

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _adb.ApplicationUser.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            //if found unlock it
            user.LockoutEnd = DateTime.Now;       //Lockout will end now

            await _adb.SaveChangesAsync();      //also save in db

            return RedirectToAction("Index");
        }
    }
}
