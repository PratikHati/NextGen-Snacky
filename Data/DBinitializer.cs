using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NextGen_Snacky.Models;
using NextGen_Snacky.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextGen_Snacky.Data
{
    //--------------------------------------------FOR AZURE DEPLOYMENT-----------------------------------------------
    public class DBinitializer : IDBinitializer
    {
        private readonly ApplicationDbContext _adb;
        private readonly UserManager<IdentityUser> _manager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public DBinitializer(ApplicationDbContext adb, UserManager<IdentityUser> manage, RoleManager<IdentityRole> rolemamage)
        {
            _adb = adb;
            _manager = manage;
            _rolemanager = rolemamage;
        }
        public async void Initialize()
        {
            try
            {
                if(_adb.Database.GetPendingMigrations().Count() > 0)    //if not migrated
                {
                    _adb.Database.Migrate();
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }

            if(_adb.Roles.Any(x=>x.Name == SD.ManageUser))  //if manager exists
            {
                return;
            }
            else                                //no manager no other roles
            {
                //create all role(SEEDING NEW DB if no role is there)

                _rolemanager.CreateAsync(new IdentityRole(SD.ManageUser)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.FrontDeskUser)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.KitchenUser)).GetAwaiter().GetResult();
                _rolemanager.CreateAsync(new IdentityRole(SD.CustomerUser)).GetAwaiter().GetResult();


                //only once executed (SEEDING Database to add admin/manager account)
                _manager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Name = "Pratik",
                    EmailConfirmed = true,
                    PhoneNumber = "9438350383"
                }, "admin123").GetAwaiter().GetResult();


                IdentityUser usr = await _adb.Users.FirstOrDefaultAsync(x=>x.Email == "pratikbablu267@gmail.com");

                await _manager.AddToRoleAsync(usr,SD.ManageUser );
            }
        }
    }
}
