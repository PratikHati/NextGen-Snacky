using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using NextGen_Snacky.Models;
using NextGen_Snacky.Utility;

namespace NextGen_Snacky.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;        //used inbuild "RoleManager" of ASP.NET CORE (Microsoft.AspNetCore.Identity)

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PinCode { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }   

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)       //After user clicking "Register", all backend logic algorithms should be here
        {
            string role = Request.Form["common"].ToString();

            returnUrl = returnUrl ?? Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = Input.Email, 
                    Email = Input.Email,
                    Name = Input.Name,
                    PhoneNumber = Input.Phone,
                    Address = Input.Address,
                    City = Input.City,
                    State = Input.State,
                    PinCode = Input.PinCode
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                
                if (result.Succeeded)
                {
                    if(!await _roleManager.RoleExistsAsync(SD.CustomerUser))        //if Customer already not exists
                    {
                        //create a Customer account
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.ManageUser))        //if Manager already not exists
                    {
                        //create a Manager account
                        await _roleManager.CreateAsync(new IdentityRole(SD.ManageUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.KitchenUser))        //if Chef already not exists
                    { 
                        //create a Chef account
                        await _roleManager.CreateAsync(new IdentityRole(SD.KitchenUser));
                    }

                    if (!await _roleManager.RoleExistsAsync(SD.FrontDeskUser))        //if Cashier already not exists
                    {
                        //create a Cashier account
                        await _roleManager.CreateAsync(new IdentityRole(SD.FrontDeskUser));
                    }

                    //assign current role

                    if(role == SD.FrontDeskUser)
                    {
                        await _userManager.AddToRoleAsync(user, SD.FrontDeskUser);
                    }
                    else
                    {
                        if(role == SD.KitchenUser)
                        {
                            await _userManager.AddToRoleAsync(user, SD.KitchenUser);
                        }
                        else
                        {
                            if(role == SD.ManageUser)
                            {
                                await _userManager.AddToRoleAsync(user, SD.ManageUser);
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, SD.CustomerUser);
                                await _signInManager.SignInAsync(user, isPersistent: false);        //only customer
                                return LocalRedirect(returnUrl);            //customer  view will not show admin privilages
                            }
                        }
                    }
                    return RedirectToAction("Index","User",new { area = "Admin"});      //rest role can see "User" tab

                    _logger.LogInformation("User created a new account with password.");

                    /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
*/

                    /*if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }*/


                }

                foreach (var error in result.Errors)
                { 
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }   

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
