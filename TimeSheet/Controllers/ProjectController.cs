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

        if (file != null && file.Length > 0)
        {
            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var project = new Project
                    {
                        ProjectName = csv.GetField("Project Name"),
                        Description = csv.GetField("Description")
                    };
                    uploadedProjects.Add(project);
                }
            }

            _context.Projects.AddRange(uploadedProjects);
            await _context.SaveChangesAsync();
        }

        return View("UploadCSV", uploadedProjects); // Correctly returning the same view
    }


    // GET: Project/Index
    public IActionResult Index()
    {
        var projects = _context.Projects.ToList();
        return View(projects);
    }
}
