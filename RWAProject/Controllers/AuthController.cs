using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RWAProject.Models;
using RWAProject.ViewModels;
using RWAProject.Middleware;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace RWAProject.Controllers
{
    public class AuthController : Controller
    {
        private readonly RwaMoviesContext _context;

        public AuthController(RwaMoviesContext context)
        {
            _context = context;
        }

        [TypeFilter(typeof(AuthFilter))]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth"); 
        }

        // GET: Auth
        public async Task<IActionResult> Login(UserVM userVM)
        {
            ModelState.Clear();
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == userVM.Username);

            if (user != null)
            {
                string enteredHash = HashPassword(userVM.Password, user.PwdSalt);

                if (enteredHash == user.PwdHash)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Username), 
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return Redirect("/Videos");
                }
                else
                {
                    ModelState.AddModelError("", "Password or username are incorrect!");
                }
            }
            return View();
        }


        private string HashPassword(string password, string salt)
        {
            string combined = password + salt;
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }

        // GET: Auth/Create
        public IActionResult Register()
        {
            ViewData["CountryOfResidenceId"] = new SelectList(_context.Countries, "Id", "Name");
            return View();
        }

        // POST: Auth/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserVM userVM)
        {
            ModelState.Clear();

            if (_context.Users.Any(u => u.Username == userVM.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
            }

            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            string saltString = Convert.ToBase64String(salt);

            using (var sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(userVM.Password + saltString);
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);
                string hashedPassword = Convert.ToBase64String(hashBytes);
                string securityToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

                User user = new User
                {
                    CreatedAt = DateTime.Now,
                    DeletedAt = null,
                    Username = userVM.Username,
                    FirstName = userVM.FirstName,
                    LastName = userVM.LastName,
                    Email = userVM.Email,
                    PwdHash = hashedPassword,
                    PwdSalt = saltString,
                    Phone = userVM.Phone,
                    IsConfirmed = true,
                    SecurityToken = securityToken,
                    CountryOfResidenceId = userVM.CountryOfResidenceId
                };

                if (ModelState.IsValid)
                {
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Login));
                }
                ViewData["CountryOfResidenceId"] = new SelectList(_context.Countries, "Id", "Id", user.CountryOfResidenceId);
                return View(userVM);
            }
        }
    }
}
