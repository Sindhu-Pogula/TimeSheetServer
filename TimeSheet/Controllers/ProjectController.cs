using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TimeSheet.Models;
using CsvHelper; // Install CsvHelper NuGet package

public class ProjectController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Project/UploadCSV
    public IActionResult UploadCSV()
    {
        return View();
    }

    // POST: Project/UploadCSV
    [HttpPost]
    public async Task<IActionResult> UploadCSV(IFormFile file)
    {
        List<Project> uploadedProjects = new List<Project>();
        List<string> duplicateProjects = new List<string>();

        if (file != null && file.Length > 0)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    string projectName = csv.GetField("Project Name");
                    string description = csv.GetField("Description");

                    // Check if the project already exists
                    bool exists = await _context.Projects.AnyAsync(p => p.ProjectName == projectName && p.Description == description);

                    if (exists)
                    {
                        duplicateProjects.Add(projectName);
                    }
                    else
                    {
                        var project = new Project
                        {
                            ProjectName = projectName,
                            Description = description
                        };
                        uploadedProjects.Add(project);
                    }
                }
            }

            if (uploadedProjects.Any())
            {
                _context.Projects.AddRange(uploadedProjects);
                await _context.SaveChangesAsync();
            }
        }

        if (duplicateProjects.Any())
        {
            TempData["DuplicateMessage"] = $"The following projects already exist: {string.Join(", ", duplicateProjects)}";
        }

        TempData["SuccessMessage"] = uploadedProjects.Any() ? "Projects uploaded successfully!" : "No new projects uploaded.";
        return View("UploadCSV", uploadedProjects);
    }



    // GET: Project/Index
    public IActionResult Index()
    {
        var projects = _context.Projects.ToList();
        return View(projects);
    }
}