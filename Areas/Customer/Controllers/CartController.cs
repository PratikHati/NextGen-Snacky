using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using NextGen_Snacky.Models.ViewModels;
using NextGen_Snacky.Utility;
using Stripe;
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

        [HttpGet]
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

            if (cart != null)
            {
                ViewmodelCart.ListCart = cart.ToList();
            }

            //go each cart info and add price and count and description
            foreach (var v in ViewmodelCart.ListCart)
            {
                v.MenuItem = await _adb.MenuItem.FirstOrDefaultAsync(x => x.Id == v.MenuItemId);

                ViewmodelCart.OrderHeader.OrderTotal += (v.MenuItem.Price * v.Count);     //Error , debuging

                v.MenuItem.Description = SD.ConvertToRawHtml(v.MenuItem.Description);   //for html
            }

            ViewmodelCart.OrderHeader.OrderTotalOriginal = ViewmodelCart.OrderHeader.OrderTotal;        //coupon logic I willbe added later

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ViewmodelCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);   //coupon code assigned to currnet user

                var couponfromDB = await _adb.Coupon.Where(x => x.Name.ToLower() == ViewmodelCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();

                ViewmodelCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponfromDB, ViewmodelCart.OrderHeader.OrderTotalOriginal);
            }

            return View(ViewmodelCart);
        }
        public async Task<IActionResult> Summary()
        {
            ViewmodelCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            ViewmodelCart.OrderHeader.OrderTotal = 0;  //Starting price is 0


            //retrive user info from session
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser appuser = await _adb.ApplicationUser.Where(x => x.Id == claim.Value).FirstOrDefaultAsync();


            //get corressponding cart info
            var cart = _adb.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                ViewmodelCart.ListCart = cart.ToList();
            }

            //go each cart info and add price and count and description
            foreach (var v in ViewmodelCart.ListCart)
            {
                v.MenuItem = await _adb.MenuItem.FirstOrDefaultAsync(x => x.Id == v.MenuItemId);

                ViewmodelCart.OrderHeader.OrderTotal += (v.MenuItem.Price * v.Count);     //Error , debuging
            }

            ViewmodelCart.OrderHeader.OrderTotalOriginal = ViewmodelCart.OrderHeader.OrderTotal;        //coupon logic I willbe added later
            ViewmodelCart.OrderHeader.PickUpName = appuser.Name;
            ViewmodelCart.OrderHeader.PhoneNumber = appuser.PhoneNumber;
            ViewmodelCart.OrderHeader.PickUpTime = DateTime.Now;


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ViewmodelCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);   //coupon code assigned to currnet user

                var couponfromDB = await _adb.Coupon.Where(x => x.Name.ToLower() == ViewmodelCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();

                ViewmodelCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponfromDB, ViewmodelCart.OrderHeader.OrderTotalOriginal);
            }

            return View(ViewmodelCart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost(string stripetoken)              //Not tested, pls review later  
        {
            //retrive user info from session
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);

            //assign all property value
            ViewmodelCart.ListCart = await _adb.ShoppingCart.Where(x => x.ApplicationUserId == claim.Value).ToListAsync();
            ViewmodelCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ViewmodelCart.OrderHeader.OrderDate = DateTime.Now;
            ViewmodelCart.OrderHeader.UserId = claim.Value;
            ViewmodelCart.OrderHeader.Status = SD.PaymentStatusPending;
            ViewmodelCart.OrderHeader.PickUpTime = Convert.ToDateTime(ViewmodelCart.OrderHeader.PickUpDate.ToShortDateString() + " " + ViewmodelCart.OrderHeader.PickUpTime.ToShortDateString());


            List<OrderDetails> orderdetailslist = new List<OrderDetails>();
            _adb.OrderHeader.Add(ViewmodelCart.OrderHeader);        //save it in DB bcz OrderDetailsCart needs OrderHeader id to FK mapping

            await _adb.SaveChangesAsync();

            ViewmodelCart.OrderHeader.OrderTotalOriginal = 0;

            //go each cart info and add price and count and description
            foreach (var v in ViewmodelCart.ListCart)
            {
                v.MenuItem = await _adb.MenuItem.FirstOrDefaultAsync(x => x.Id == v.MenuItemId);

                OrderDetails orderdetails = new OrderDetails
                {
                    MenuId = v.MenuItemId,
                    OrderId = ViewmodelCart.OrderHeader.Id,
                    Description = v.MenuItem.Description,
                    Name = v.MenuItem.Name,
                    Price = v.MenuItem.Price,
                    Count = v.Count
                };

                ViewmodelCart.OrderHeader.OrderTotalOriginal += orderdetails.Count * orderdetails.Price;
                _adb.OrderDetails.Add(orderdetails);

                //await _adb.SaveChangesAsync();        don't use here, coupon may be used
            }


            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                ViewmodelCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);   //coupon code assigned to currnet user

                var couponfromDB = await _adb.Coupon.Where(x => x.Name.ToLower() == ViewmodelCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();

                ViewmodelCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponfromDB, ViewmodelCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                ViewmodelCart.OrderHeader.OrderTotal = ViewmodelCart.OrderHeader.OrderTotalOriginal;
            }

            ViewmodelCart.OrderHeader.CouponCodeDiscount = ViewmodelCart.OrderHeader.OrderTotalOriginal - ViewmodelCart.OrderHeader.OrderTotal;
            
            await _adb.SaveChangesAsync();

            //Note- current cart info should be nullified, as next cart should be empty and current cart is being paid
            _adb.ShoppingCart.RemoveRange(ViewmodelCart.ListCart);
            HttpContext.Session.SetInt32(SD.ssCartCount,0);


            await _adb.SaveChangesAsync();

            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(ViewmodelCart.OrderHeader.OrderTotal * 100),     //STRIPE calculate price in cents not on dolars
                Currency = "usd",
                Description = "Order ID: "+ ViewmodelCart.OrderHeader.Id,
                Source = stripetoken
            };

            var service = new ChargeService();              //Charge from STRIPE package

            //---------------------IMPORTANT------------------------

            Charge charge = service.Create(options);        //Actual transaction logic

            if(charge.BalanceTransactionId == null)
            {
                //failed transaction
                ViewmodelCart.OrderHeader.PaymentStatus = SD.PaymentStatusCancelled;
            }
            else
            {
                //successful transactionID but still may have error in actual transaction operation like "ROLEBACK" 
                ViewmodelCart.OrderHeader.TransactionID = charge.BalanceTransactionId;  
            }

            if(charge.Status.ToLower() == "succeeded")
            {
                ViewmodelCart.OrderHeader.PaymentStatus = SD.PaymentStatusCompleted;
                ViewmodelCart.OrderHeader.Status = SD.StatusSubmitted;
            }
            else
            {
                ViewmodelCart.OrderHeader.PaymentStatus = SD.PaymentStatusCancelled;
            }

            await _adb.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }
        public CartController(ApplicationDbContext adb)
        {
            _adb = adb;
        }

        public IActionResult AddCoupon()
        {
            if (ViewmodelCart.OrderHeader.CouponCode == null)
            {
                ViewmodelCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, ViewmodelCart.OrderHeader.CouponCode);       //assign coupon  from DB to server code i.e. "ViewmodelCart.OrderHeader.CouponCode"

            return RedirectToAction("Index");
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);       //unassign coupon  from DB to server code i.e. "ViewmodelCart.OrderHeader.CouponCode"

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Plus(int cartid)
        {
            var cart = await _adb.ShoppingCart.Where(x => x.Id == cartid).FirstOrDefaultAsync();

            
            cart.Count += 1;

            await _adb.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Minus(int cartid)
        {
            var cart = await _adb.ShoppingCart.Where(x => x.Id == cartid).FirstOrDefaultAsync();

            if(cart.Count == 1)
            {
                _adb.ShoppingCart.Remove(cart);
                await _adb.SaveChangesAsync();

                //also remove from current session

                //get new count of that same user from db
                var cnt = _adb.ShoppingCart.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssCartCount,cnt);
            }
            else
            {
                cart.Count -= 1;
                await _adb.SaveChangesAsync();
            }
            
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Remove(int cartid)
        {
            var cart = await _adb.ShoppingCart.Where(x => x.Id == cartid).FirstOrDefaultAsync();

            _adb.ShoppingCart.Remove(cart);
            await _adb.SaveChangesAsync();

            var cnt = _adb.ShoppingCart.Where(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssCartCount, cnt);

            return RedirectToAction("Index");
        }

    }
}
