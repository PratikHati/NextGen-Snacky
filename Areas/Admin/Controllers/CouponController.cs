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

        //GET - Edit
        public async Task<IActionResult> Edit(int ? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = await _adb.Coupon.SingleOrDefaultAsync(x => x.Id == id);

            if(coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        //POST - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Coupon coupon)
        {
            if (coupon == null)
            {
                return NotFound();
            }

            var couponDB = await _adb.Coupon.SingleOrDefaultAsync(x => x.Id == coupon.Id);

            if (ModelState.IsValid)
            {
                var file = HttpContext.Request.Form.Files;      //retrive any file from FrontEnd .cshtml form

                byte[] f = null;

                //update file info in byte[]
                if(file.Count > 0)
                {
                    using (var x = file[0].OpenReadStream())
                    {
                        using (var y = new MemoryStream())
                        {
                            //copy x to y
                            x.CopyTo(y);

                            f = y.ToArray();
                        }
                    }
                    //save byte[] in db
                    couponDB.Picture = f;
                }

                //update other properties
                couponDB.Name = coupon.Name;
                couponDB.MinimumAmount = coupon.MinimumAmount;
                couponDB.IsActive = coupon.IsActive;
                couponDB.Discount = coupon.Discount;
                couponDB.CouponType = coupon.CouponType;

                //savechanges
                await _adb.SaveChangesAsync();


                //return view
                return RedirectToAction("Index");
            }

            return View(coupon);
        }

        //GET - Details
        public async Task<IActionResult> Details(int ? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var coupon = await _adb.Coupon.SingleOrDefaultAsync(x => x.Id == id);

            if(coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }
    }
}
