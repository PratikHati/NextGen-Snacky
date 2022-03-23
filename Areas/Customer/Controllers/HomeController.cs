using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
using System.Security.Claims;
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

            //after user again log in, if he did not buy the prev cart, then show it again
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)  //if user logged in
            {
                //retrive count of cart of this user
                var cnt =  _adb.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value).ToList().Count;     //fix - don't use await

                //assign to current session
                HttpContext.Session.SetInt32("ssCartCount", cnt);
            }
            return View(IndexVM);
        }

     
        public async Task<IActionResult> Details(int id)
        {
            //retreive menuitem info from db to display at Details()
            if (!User.Identity.IsAuthenticated)
            {
                return NoContent();
            }
            var MenuItemFromDB = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).Where(x => x.Id == id).FirstOrDefaultAsync();
            ShoppingCart cart = new ShoppingCart()
            {
                MenuItemId = MenuItemFromDB.Id,
                MenuItem = MenuItemFromDB
            };

            return View(cart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart shoppingcart)     //Fixed , menuItemid is going 0 
        {
            shoppingcart.Id = 0;
            if (ModelState.IsValid)
            {
                var claims = (ClaimsIdentity)this.User.Identity;                //get claims
                var claim = claims.FindFirst(ClaimTypes.NameIdentifier);        //get claim name
                shoppingcart.ApplicationUserId = claim.Value;                   //assign ApplicationUserId

                ShoppingCart cartfromdb = await _adb.ShoppingCart.Where         //cart from db
                    (x => x.ApplicationUserId == shoppingcart.ApplicationUserId && x.MenuItemId == shoppingcart.MenuItemId).FirstOrDefaultAsync();

                if (cartfromdb == null)
                {
                    await _adb.ShoppingCart.AddAsync(shoppingcart);
                }

                else
                {
                    cartfromdb.Count += shoppingcart.Count;     //add no of cart counts
                }

                await _adb.SaveChangesAsync();

                //again retrive total count of that user
                var count = _adb.ShoppingCart.Where(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count();

                //assign that count to session
                HttpContext.Session.SetInt32("ssCartCount", count);     //error

                return RedirectToAction("Index");
            }

            else
            {
                var MenuItemFromDB = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).Where(x => x.Id == shoppingcart.MenuItemId).FirstOrDefaultAsync();
                ShoppingCart cart = new ShoppingCart()
                {
                    Id = MenuItemFromDB.Id,
                    MenuItem = MenuItemFromDB
                };

                return View(cart);
            }
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
