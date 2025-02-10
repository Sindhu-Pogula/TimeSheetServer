using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TimeSheet.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin Dashboard
    public IActionResult AdminDashboard()
    {
        var projects = _context.Projects.ToList();
        return View(projects);
    }

    // GET: Upload CSV
    public IActionResult UploadCsv()
    {
        return View();
    }

    // POST: Upload CSV
    [HttpPost]
    public IActionResult UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "Please upload a valid CSV file.");
            return View();
        }

        try
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                var projects = new List<Project>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (values.Length == 2) // Ensure each row has exactly two columns
                    {
                        var project = new Project
                        {
                            ProjectName = values[0].Trim(),
                            Description = values[1].Trim()
                        };

                        // Add project only if it doesn't already exist
                        if (!_context.Projects.Any(p => p.ProjectName == project.ProjectName && p.Description == project.Description))
                        {
                            projects.Add(project);
                        }
                    }
                }

                if (projects.Any())
                {
                    _context.Projects.AddRange(projects);
                    _context.SaveChanges();
                    TempData["Message"] = $"{projects.Count} projects uploaded successfully!";
                }
                else
                {
                    TempData["Message"] = "No new projects were added (duplicates or invalid data).";
                }
            }
        }
        catch
        {
            ModelState.AddModelError("", "An error occurred while processing the file.");
            return View();
        }

        return RedirectToAction("AdminDashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Logout()
    {
        //await SignInManager.SignOutAsync();  // Ensure the user is logged out
        return Task.FromResult<IActionResult>(RedirectToAction("Login", "Account"));  // Redirect to the Index action of HomeController
    }
}
