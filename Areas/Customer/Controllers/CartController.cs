using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models.ViewModels;
using NextGen_Snacky.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NextGen_Snacky.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _adb;

        [BindProperty]
        public OrderDetailsCart ViewmodelCart { get; set; }
        public async Task<IActionResult> Index()
        {
            ViewmodelCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            ViewmodelCart.OrderHeader.OrderTotal = 0;  //Starting price is 0

            //retrive user info from session
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            //get corressponding cart info
            var cart = _adb.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value);

            if(cart != null)
            {
                ViewmodelCart.ListCart = cart.ToList();
            }
            
            //go each cart info and add price and count and description
            foreach(var v in ViewmodelCart.ListCart)
            {
                v.MenuItem = await _adb.MenuItem.FirstOrDefaultAsync(x => x.Id == v.MenuItemId);

                ViewmodelCart.OrderHeader.OrderTotal += (v.MenuItem.Price * v.Count);     //Error , debuging

                v.MenuItem.Description = SD.ConvertToRawHtml(v.MenuItem.Description);   //for html
            }

            ViewmodelCart.OrderHeader.OrderTotalOriginal = ViewmodelCart.OrderHeader.OrderTotal;        //coupon logic I willbe added later
            
            return View(ViewmodelCart);
        }

        public CartController(ApplicationDbContext adb)
        {
            _adb = adb;
        }


    }
}
