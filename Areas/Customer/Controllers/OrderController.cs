using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NextGen_Snacky.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _adb;
        public OrderController(ApplicationDbContext adb)
        {
            _adb = adb;
        }

        [Authorize] //only if user is authorized
        public async Task<IActionResult> Confirm(int id)
        {
            //claimsidentity
            var claims = (ClaimsIdentity)this.User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);    //claim will contain current user info including id as in query string if I change order id 2 to 3, then unauthorized user may see other user cart

            OrderDetailsListViewModel orderdetailsvm = new OrderDetailsListViewModel()
            {
                OrderHeader = await _adb.OrderHeader.Include(x => x.ApplicationUser).FirstOrDefaultAsync(x=>x.Id == id && x.UserId == claim.Value),
                OrderDetailsList = await _adb.OrderDetails.Where(x=>x.OrderId == id).ToListAsync()
            };

            return View(orderdetailsvm);
        }
        public IActionResult Index()
        {
            return View();
        }
    }   
}
