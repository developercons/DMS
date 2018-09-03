using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DataLayer;
using BusinessLayer;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using DMS.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace DMS.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _context;
        public AuthController( DMSContext context )
        {
            _context = new AuthService(context);
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var _user = _context.CheckCredential(user);
                if (_user == null)
                {
                    TempData["error"] = "Credentials do not match.";
                    return RedirectToAction("Index","Auth");
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, _user[0].UserName.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Role, _user[0].UserRole));
                identity.AddClaim(new Claim(ClaimTypes.Email, _user[0].UserEmail));               
                HttpContext.Session.SetString("UserEmail", _user[0].UserEmail);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                if (_user[0].UserRole == "Admin")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "AdminOrUser"));
                    return RedirectToAction("Index", "Home");
                }
                else if (_user[0].UserRole == "User")
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "AdminOrUser"));
                    return RedirectToAction("Index", "Home");
                }
                
            }
            TempData["error"] = "Credentials do not match.";
            return RedirectToAction("Index", "Auth");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Auth");
        }
        public IActionResult Forbidden()
        {
            TempData["error"] = "Permissin Denied!";
            return RedirectToAction("Index", "Auth");
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}