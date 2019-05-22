using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewTechParentPortalV3.Models;

namespace NewTechParentPortalV3.Areas.Identity.Pages.Account
{
    //public class IdentitySeed
    //{
    //    private readonly NewTechParentPortalV31Context _context;
    //    private readonly UserManager<IdentityUser> _userManager;
    //    private readonly RoleManager<IdentityRole> _rolesManager;
    //    private readonly ILogger _logger;

    //    public IdentitySeed(
    //        NewTechParentPortalV31Context context,
    //        UserManager<IdentityUser> userManager,
    //        RoleManager<IdentityRole> roleManager,
    //         ILoggerFactory loggerFactory)
    //    {
    //        _context = context;
    //        _userManager = userManager;
    //        _rolesManager = roleManager;
    //        _logger = loggerFactory.CreateLogger<IdentitySeed>();
    //    }

        //public async Task CreateRoles()
        //{
        //    if (await _context.Roles.AnyAsync())
        //    {// not waste time
        //        _logger.LogInformation("Exists Roles.");
        //        return;
        //    }
        //    var adminRole = "Admin";
        //    var roleNames = new String[] { adminRole, "ParentRole", "Instructor", "Guest" };

        //    foreach (var roleName in roleNames)
        //    {
        //        var role = await _rolesManager.RoleExistsAsync(roleName);
        //        if (!role)
        //        {
        //            var result = await _rolesManager.CreateAsync(new IdentityRole { Name = roleName });

        //            // var result1 = await _rolesManager.CreateAsync(new IdentityRole("Parent"));
        //            _logger.LogInformation("Create {0}: {1}", roleName, result.Succeeded);
        //        }
        //    }
        //    // administrator
        //    var user = new IdentityUser
        //    {
        //        UserName = "Administrator",
        //        Email = "admin@gmail.com",
        //        EmailConfirmed = true
        //    };
        //    var i = await _userManager.FindByEmailAsync(user.Email);
        //    if (i == null)
        //    {
        //        var adminUser = await _userManager.CreateAsync(user, "Admin_123");
        //        if (adminUser.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, adminRole);
        //            //
        //            _logger.LogInformation("Create {0}", user.UserName);
        //        }
        //    }
        //}
    //}

    ///////////////////
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _rolesManager;

        //var role = new IdentityRole();
        //role.Name = "Manager";
        //    await _roleManager.CreateAsync(role);

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,//)
            RoleManager<IdentityRole> rolesManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _rolesManager = rolesManager;
        }

        
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            //[Required]
            //[StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
            //[DataType(DataType.Text)]
            //[Display(Name = "First Name")]
            //public string FirstName { get; set; }

            //[Required]
            //[StringLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
            //[DataType(DataType.Text)]
            //[Display(Name = "Last Name")]
            //public string LastName { get; set; }

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
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //public async Task CreateRoles()
            //{
            if (await _rolesManager.RoleExistsAsync("Admin"))
            {// not waste time
                _logger.LogInformation("Exists Roles.");
                // return Page();
            }
            else
            {
                var adminRole = "Admin";
                var roleNames = new String[] { adminRole, "Parent", "Instructor", "Guest" };

                foreach (var roleName in roleNames)
                {
                    var role = await _rolesManager.RoleExistsAsync(roleName);
                    if (!role)
                    {
                        var result = await _rolesManager.CreateAsync(new IdentityRole { Name = roleName });

                        // var result1 = await _rolesManager.CreateAsync(new IdentityRole("Parent"));
                        _logger.LogInformation("Create {0}: {1}", roleName, result.Succeeded);
                    }
                }
                // administrator
                var user1 = new IdentityUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };
                var i = await _userManager.FindByEmailAsync(user1.Email);
                if (i == null)
                {
                    var adminUser = await _userManager.CreateAsync(user1, "Admin_123");
                    if (adminUser.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user1, adminRole);
                        //
                        _logger.LogInformation("Create {0}", user1.UserName);
                    }
                }
            }
            //}
            /////////////
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new IdentityUser {
                    UserName = Input.Email,
                    Email = Input.Email,
                    //FirstName = Input.FirstName,
                    //LastName= Input.LastName 
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                //var result1 = await _rolesManager.CreateAsync(new IdentityRole("Parent"));
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Parent");////////////

                    //var hp = await _userManager.IsInRoleAsync(user,"Parent");

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
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
