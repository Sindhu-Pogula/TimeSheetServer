using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSheet.Models;

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
    public async Task<IActionResult> Logout()
    {
        //await SignInManager.SignOutAsync();  // Ensure the user is logged out
        return RedirectToAction("Login", "Account");  // Redirect to the Index action of HomeController
    }

    // POST: Account/Signup
    [HttpPost]
    public async Task<ActionResult> Signup(User user)
    {
        if (ModelState.IsValid)
        {
            // Check if a user with the same username already exists
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                // Add a model error to indicate that the user already exists
                ModelState.AddModelError("Username", "User already exists.");
                return View(user);
            }

            // Add user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            /// / Set a flag in session to indicate successful registration
            HttpContext.Session.SetString("RegistrationSuccess", "true");

            // Redirect to the Login page after successful signup
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
        // Find the user by username
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

        if (user != null)
        {
            // Check if the password is correct
            if (user.Password == password)
            {
                // Store user in session
                HttpContext.Session.SetString("Username", user.Username);

                // Redirect to the UserDashboard if the login is successful
                return RedirectToAction("UserDashboard");
            }
            else
            {
                // Add an error for incorrect password
                ModelState.AddModelError("Password", "Invalid password.");
            }
        }
        else
        {
            // Optionally, show an error for username not found
            ModelState.AddModelError("Username", "Username not found.");
        }

        // Return the view with errors
        return View();
    }
    // GET: Account/UserDashboard
    public ActionResult UserDashboard()
    {
        // Retrieve the logged-in user's username from session
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            // If no user is logged in, redirect to Login page
            return RedirectToAction("Login");
        }

        // Find the user based on the username stored in session
        var user = _context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            // If user is not found, redirect to Login page
            return RedirectToAction("Login");
        }

        // Pass the user model to the UserDashboard view
        return View(user);
    }
}