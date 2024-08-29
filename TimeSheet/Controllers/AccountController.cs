using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Signup                             
        public IActionResult Signup()
        {
            return View();
        }

        // POST: /Account/Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Signup(User model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Users.Add(model);
                    _context.SaveChanges();

                    // Redirect to the Login action after successful signup
                    return RedirectToAction("Login", "Account");
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log the error)
                    Console.WriteLine($"Exception: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the data. Please try again.");
                }
            }
            else
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Authentication logic here (e.g., setting cookies or tokens)
                    await SignInUser(user);

                    // Redirect to the dashboard after successful login
                    return RedirectToAction("UserDashboard", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username)
        // Add other claims as needed
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                // Additional properties can be set here if needed
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }
        [Authorize]  // Ensures only authenticated users can access the dashboard
        public IActionResult Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));

            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var model = new UserDashboard
            {
                Username = user.Username,
                Email = user.Email,
                // Add other properties as needed
            };

            return View(model);
        }


    }
}
