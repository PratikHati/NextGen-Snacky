using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        //GET- Create
        public IActionResult Create()
        {
            return View();
        }

        //POST- Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;     //retrive file from from
                
                if(files.Count > 0)                             //file uploaded successfully
                {
                    //create a byte array
                    byte[] p1 = null;

                    using (var fs1 = files[0].OpenReadStream())   //read first files object
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            //copy from fs1 to ms1
                            fs1.CopyTo(ms1);

                            //assign ms1 to byte[]
                            p1 = ms1.ToArray();
                        }
                    }
                    //then store image in coupon
                    coupon.Picture = p1;
                }

                //store other attributes
                _adb.Coupon.Add(coupon);
                await _adb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            else
            {
                return View(coupon);
            }
        }
    }
}
