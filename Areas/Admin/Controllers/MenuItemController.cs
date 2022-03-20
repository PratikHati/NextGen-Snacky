using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Data;
using NextGen_Snacky.Models;
using NextGen_Snacky.Models.ViewModels;
using NextGen_Snacky.Utility;
using System;
using System.Collections.Generic;
using System.IO;
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
            _MenuItemViewModel = new MenuItemViewModel()
            {
                Category = _adb.Category,
                MenuItem = new Models.MenuItem()
            };
        }
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var menu = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).ToListAsync();
                return View(menu);
            }

            return NoContent();            
        }
        //GET- Create
        public IActionResult Create()
        {
            return View(_MenuItemViewModel);
        }

        //POST- Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()           // MenuItemViewModel already binded in constructor so , not need to pass as parameter
        {
            if(User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            _MenuItemViewModel.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());      //"SubCategoryId" will be collcted from cshtml Form

            if (!ModelState.IsValid)
            {
                return View(_MenuItemViewModel);
            }

            _adb.MenuItem.Add(_MenuItemViewModel.MenuItem);
            await _adb.SaveChangesAsync();

            //image saving logic
            string rootpath = _hosting.WebRootPath;
            var files = HttpContext.Request.Form.Files;             //retreive the file from Create.cshtml uploaded by user

            var menuItemFromDb = await _adb.MenuItem.FindAsync(_MenuItemViewModel.MenuItem.Id);         //retrive the MenuItem ID for the picture

            if (files.Count > 0)
            {
                //file already uploaded
                var upload = Path.Combine(rootpath,@"images\");       //fixed
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(upload + _MenuItemViewModel.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);        //copy file to server(upload)
                }
                menuItemFromDb.Image = @"\images\" +_MenuItemViewModel.MenuItem.Id + extension;
            }
            else
            {
                //files not uploaded, use default 
                var upload = Path.Combine(rootpath,@"\images\"+SD.DefaultFoodImage);        //use default png file

                System.IO.File.Copy(upload,rootpath+@"\images\"+_MenuItemViewModel.MenuItem.Id+".png");

                menuItemFromDb.Image = @"\images\" + _MenuItemViewModel.MenuItem.Id + ".png";      //also update menuItemFromDb's Image
            }

            await _adb.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET- Edit
        public async Task<IActionResult> Edit(int ? id)
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            if (id == null)
            {
                return NotFound();
            }

            _MenuItemViewModel.MenuItem = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x=>x.Id==id);

            _MenuItemViewModel.SubCaregory = await _adb.SubCategory.Where(y=>y.CategoryId==_MenuItemViewModel.MenuItem.CategoryId).ToListAsync();

            if (_MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(_MenuItemViewModel);
        }

        //POST- Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int ? id)      // MenuItemViewModel already binded in constructor so , not need to pass as parameter
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            if (id == null)
            {
                return NotFound();
            }
            _MenuItemViewModel.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());      //"SubCategoryId" will be collcted from cshtml Form

            if (!ModelState.IsValid)
            {
                _MenuItemViewModel.SubCaregory = await _adb.SubCategory.Where(x=>x.CategoryId == _MenuItemViewModel.MenuItem.CategoryId).ToListAsync();
                return View(_MenuItemViewModel);
            }


            //image saving logic
            string rootpath = _hosting.WebRootPath;
            var files = HttpContext.Request.Form.Files;             //retreive the file from Create.cshtml uploaded by user

            var menuItemFromDb = await _adb.MenuItem.FindAsync(_MenuItemViewModel.MenuItem.Id);         //retrive the MenuItem ID for the picture
                
            if (files.Count > 0)
            {
                    //file already uploaded
                var upload = Path.Combine(rootpath, @"images\");    //Review if not updating image url
                var extension = Path.GetExtension(files[0].FileName);

                //Delete origonal file
                var path = Path.Combine(rootpath, menuItemFromDb.Image.TrimStart('\\'));       //trim that out

                if (System.IO.File.Exists(path))
                {
                    //Delete the file/image
                    System.IO.File.Delete(path);
                }

                //upload new file
                using (var filestream = new FileStream(Path.Combine(upload + _MenuItemViewModel.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);        //copy file to server(upload)
                }
                menuItemFromDb.Image = @"\images\" + _MenuItemViewModel.MenuItem.Id + extension;
            }

            //also update any change in property of MenuItemViewModel
            menuItemFromDb.Name = _MenuItemViewModel.MenuItem.Name;
            menuItemFromDb.Price = _MenuItemViewModel.MenuItem.Price;
            menuItemFromDb.Spicyness = _MenuItemViewModel.MenuItem.Spicyness;
            menuItemFromDb.Description = _MenuItemViewModel.MenuItem.Description;
            menuItemFromDb.CategoryId = _MenuItemViewModel.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = _MenuItemViewModel.MenuItem.SubCategoryId;


            await _adb.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //GET- Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _MenuItemViewModel.MenuItem = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x => x.Id == id);

            _MenuItemViewModel.SubCaregory = await _adb.SubCategory.Where(y => y.CategoryId == _MenuItemViewModel.MenuItem.CategoryId).ToListAsync();

            if (_MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(_MenuItemViewModel);
        }

        //GET- Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            if (id == null)
            {
                return NotFound();
            }

            _MenuItemViewModel.MenuItem = await _adb.MenuItem.Include(x => x.Category).Include(x => x.SubCategory).SingleOrDefaultAsync(x => x.Id == id);

            //_MenuItemViewModel.SubCaregory = await _adb.SubCategory.Where(y => y.CategoryId == _MenuItemViewModel.MenuItem.CategoryId).ToListAsync();

            if (_MenuItemViewModel.MenuItem == null)
            {
                return NotFound();
            }
            return View(_MenuItemViewModel);
        }

        //POST- Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int  id)           // MenuItemViewModel already binded in constructor so , not need to pass as parameter
        {
            if (User.IsInRole(SD.CustomerUser) || User.IsInRole(SD.FrontDeskUser))
            {
                return NoContent();
            }

            var rootpath = _hosting.WebRootPath;
            MenuItem menuitem = await _adb.MenuItem.FindAsync(id);

            if(menuitem != null)
            {
                //first delete image path
                var imagepath = Path.Combine(rootpath,menuitem.Image.TrimStart('\\'));


                if (System.IO.File.Exists(imagepath))
                {
                    //delete File , not path
                    System.IO.File.Delete(imagepath);   
                }
                //then delete menuitem
                _adb.MenuItem.Remove(menuitem);

                //save db
                await _adb.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
