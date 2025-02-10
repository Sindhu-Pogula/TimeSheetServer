using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
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

        // GET: Account/Signup
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Signup(User user)
        {
            if (ModelState.IsValid)
            {
                // Check if a user with the same username already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                {
                    ModelState.AddModelError("Username", "User already exists.");
                    return View(user);
                }

                // Ensure role is assigned correctly (default to "User" if not provided)
                user.Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;

                // Add user to the database
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("RegistrationSuccess", "true");
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user != null)
            {
                if (user.Password == password)
                {
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else if (user.Role == "User")
                    {  
                        return RedirectToAction("UserDashboard");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid password.");
                }
            }
            else
            {
                ModelState.AddModelError("Username", "Username not found.");
            }

            return View();
        }

        // GET: Account/AdminDashboard
        public ActionResult AdminDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Admin")
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        // GET: Account/UserDashboard
        public ActionResult UserDashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "User")
            {
                return RedirectToAction("Login");
            }

            var username = HttpContext.Session.GetString("Username");
            var user = _context.Users.SingleOrDefault(u => u.Username == username);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Logout()
        {
            //await SignInManager.SignOutAsync();  // Ensure the user is logged out
            return Task.FromResult<IActionResult>(RedirectToAction("Login", "Account"));  // Redirect to the Index action of HomeController
        }
    }
}
