using BusinessRegistrationSystem.Models;
using BusinessRegistrationSystem.Services;
using BusinessRegistrationSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BusinessRegistrationSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _dbContext;

        public AuthController(IAuthService authService, AppDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _authService.LoginAsync(username, password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("LoginSuccess");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoginSuccess()
        {
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return RedirectToAction("Login");
            }

            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return RedirectToAction("Login");

            List<BusinessRegistration> registrations;
            if (user.Role == UserRole.Admin)
            {
                registrations = await _dbContext.BusinessRegistrations
                    .OrderByDescending(r => r.SubmittedAt)
                    .ToListAsync();
            }
            else
            {
                registrations = await _dbContext.BusinessRegistrations
                    .Where(r => r.OwnerId == userId)
                    .OrderByDescending(r => r.SubmittedAt)
                    .ToListAsync();
            }

            var viewModel = new DashboardViewModel
            {
                User = user,
                Registrations = registrations
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            if (await _authService.RegisterAsync(username, password, UserRole.User))
            {
                ViewBag.Message = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Username already exists.";
            return View();
        }
    }
}