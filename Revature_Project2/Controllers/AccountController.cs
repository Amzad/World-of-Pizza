﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace Revature_Project2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        [TempData]
        public string ErrorMessage { get; set; }

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateCreditCard()
        {
            int custID = int.Parse(User.FindFirst("customerID").Value);
            var httpClient = _clientFactory.CreateClient("API");

            var request = new HttpRequestMessage(HttpMethod.Get,
                Program.API + "customers/" + custID);
            request.Headers.Add("authorization", "Bearer " + User.FindFirstValue("access_token"));
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Customer cust = await response.Content.ReadAsAsync<Customer>();
                return View("UpdateCreditCard", cust);
            }
            return View("CreditCard");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateProfile()
        {
            int custID = int.Parse(User.FindFirst("customerID").Value);
            var httpClient = _clientFactory.CreateClient("API");

            var request = new HttpRequestMessage(HttpMethod.Get,
                Program.API + "customers/" + custID);
            request.Headers.Add("authorization", "Bearer " + User.FindFirstValue("access_token"));
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Customer cust = await response.Content.ReadAsAsync<Customer>();
                return View("UpdateProfile", cust);
            }
            return View("");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreditCard(string cardnumber, string expmonth, string securitycode, string cardtype)
        {
            int custID = int.Parse(User.FindFirst("customerID").Value);
            var httpClient = _clientFactory.CreateClient("API");

            var request = new HttpRequestMessage(HttpMethod.Get,
                Program.API + "customers/" + custID);
            request.Headers.Add("authorization", "Bearer " + User.FindFirstValue("access_token"));
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Customer cust = await response.Content.ReadAsAsync<Customer>();
                cust.CreditCardNumber = cardnumber;
                cust.ExpMonth = expmonth;
                cust.SecurityCode = securitycode;
                cust.ExpYear = cardtype;

                // Update Customer Object
                httpClient = _clientFactory.CreateClient("API");
                response = await httpClient.PutAsJsonAsync(Program.API + "Customers/" + custID, cust);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Checkout", "CheckOut");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to update credit card details");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to update credit card details");
                return View();
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateCreditCard(string cardnumber, string expmonth, string securitycode, string cardtype)
        {
            int custID = int.Parse(User.FindFirst("customerID").Value);
            var httpClient = _clientFactory.CreateClient("API");

            var request = new HttpRequestMessage(HttpMethod.Get,
                Program.API + "customers/" + custID);
            request.Headers.Add("authorization", "Bearer " + User.FindFirstValue("access_token"));
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Customer cust = await response.Content.ReadAsAsync<Customer>();
                cust.CreditCardNumber = cardnumber;
                cust.ExpMonth = expmonth;
                cust.SecurityCode = securitycode;
                cust.ExpYear = cardtype;

                // Update Customer Object
                httpClient = _clientFactory.CreateClient("API");
                response = await httpClient.PutAsJsonAsync(Program.API + "Customers/" + custID, cust);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to update credit card details");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to update credit card details");
                return View();
            }

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string firstname, string lastname, string address, string state, string zipcode)
        {
            int custID = int.Parse(User.FindFirst("customerID").Value);
            var httpClient = _clientFactory.CreateClient("API");

            var request = new HttpRequestMessage(HttpMethod.Get,
                Program.API + "customers/" + custID);
            request.Headers.Add("authorization", "Bearer " + User.FindFirstValue("access_token"));
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                Customer cust = await response.Content.ReadAsAsync<Customer>();
                cust.CustomerFirstName = firstname;
                cust.CustomerLastName = lastname;
                cust.CustomerAddress = address;
                cust.State = state;
                cust.ZipCode = zipcode;

                // Update Customer Object
                httpClient = _clientFactory.CreateClient("API");
                response = await httpClient.PutAsJsonAsync(Program.API + "Customers/" + custID, cust);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Unable to update credit card details");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Unable to update credit card details");
                return View();
            }

        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {

            if (ModelState.IsValid)
            {

                //returnUrl = returnUrl ?? Url.Content("~/");

                Customer cust = new Customer()
                {
                    CustomerEmail = model.CustomerEmail,
                    Password = model.Password
                };
                var httpClient = _clientFactory.CreateClient("API");
                var response = await httpClient.PostAsJsonAsync(Program.API + "token", cust);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    Dictionary<string, string> tokenDictionary =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                    var access_token = tokenDictionary["access_token"];
                    var Email = tokenDictionary["email"];
                    var firstName = tokenDictionary["firstName"];
                    var lastName = tokenDictionary["lastName"];
                    var customerID = tokenDictionary["customerID"];


                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, Email ),
                        new Claim("firstName", firstName),
                        new Claim("lastName", lastName),
                        new Claim("access_token", access_token),
                        new Claim("customerID", customerID)
                    };
                    var iden = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        // Refreshing the authentication session should be allowed.

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(29),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        IsPersistent = true

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };
                    var principal = new ClaimsPrincipal(iden);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);


                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid password");
                    return View();
                }

                //return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterInputModel Input)
        {
            //returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                Customer user = new Customer { Username = Input.Email, CustomerEmail = Input.Email, CustomerFirstName = Input.FirstName, CustomerLastName = Input.LastName, CustomerPhoneNumber = Input.PhoneNumber, CustomerAddress = Input.Address, State = Input.State, ZipCode = Input.ZipCode ,Password = Input.Password };

                var httpClient = _clientFactory.CreateClient("API");
                var response = await httpClient.PostAsJsonAsync(Program.API + "create", user);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    Dictionary<string, string> tokenDictionary =
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

                    var access_token = tokenDictionary["access_token"];
                    var Email = tokenDictionary["email"];
                    var firstName = tokenDictionary["firstName"];
                    var lastName = tokenDictionary["lastName"];
                    var customerID = tokenDictionary["customerID"];


                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, Email ),
                        new Claim("firstName", firstName),
                        new Claim("lastName", lastName),
                        new Claim("customerID", customerID),
                        new Claim("access_token", access_token)
                    };
                    var iden = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,

                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(29),

                        IsPersistent = true

                    };
                    var principal = new ClaimsPrincipal(iden);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.ReasonPhrase);
                    return View();
                }
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
            /*}
            else
            {
                return Page();
            }*/
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}