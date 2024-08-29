using Microsoft.AspNetCore.Mvc;
using TimeSheet.Models;

public class AccountController : Controller
{
    private static List<User> users = new List<User>();

    // GET: Account/Signup
    public ActionResult Signup()
    {
        return View();
    }

    // POST: Account/Signup
    [HttpPost]
    public ActionResult Signup(User user)
    {
        if (ModelState.IsValid)
        {
            // Add user to the database (in this case, a simple list for demonstration)
            users.Add(user);

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
    public ActionResult Login(string username, string password)
    {
        // Validate the user (simple check for demonstration)
        var user = users.SingleOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            // Store user in session (simplified example)
            HttpContext.Session.SetString("Username", user.Username);

            // Redirect to the UserDashboard if the login is successful
            return RedirectToAction("UserDashboard");
        }

        // Invalid login attempt
        ModelState.AddModelError("", "Invalid username or password.");
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
        var user = users.SingleOrDefault(u => u.Username == username);

        if (user == null)
        {
            // If user is not found, redirect to Login page
            return RedirectToAction("Login");
        }

        // Pass the user model to the UserDashboard view
        return View(user);
    }
}
