﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Revature_Project2.Data;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Revature_Project2.Models;

namespace Revature_Project2.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IHttpClientFactory clientFactory)
        {
            _signInManager = signInManager;
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string CustomerEmail { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {

                returnUrl = returnUrl ?? Url.Content("~/");

                Customer cust = new Customer();
                cust.CustomerEmail = Input.CustomerEmail;
                cust.Password = Input.Password;
                var httpClient = _clientFactory.CreateClient("API");
                var response = await httpClient.PostAsJsonAsync("https://localhost:44376/api/token", cust);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    Dictionary<string, string> tokenDictionary =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                    var access_token = tokenDictionary["access_token"];
                    var Email = tokenDictionary["email"];
                    var firstName = tokenDictionary["firstName"];
                    var lastName = tokenDictionary["lastName"];

                   
                    var claims = new List<Claim>
                    {  
                        new Claim(ClaimTypes.Email, Email ),
                        new Claim("firstName", firstName),
                        new Claim("lastName", lastName)
                        // add other claimsas you want ...
                    };
                    var iden = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(iden);

                    await _signInManager
                    var result = await _signInManager.PasswordSignInAsync(Input.CustomerEmail, Input.Password, Input.RememberMe, lockoutOnFailure: false);



                    return LocalRedirect(returnUrl);

                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid password");
                    return Page();
                }

                //return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();






            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Input.CustomerEmail, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
