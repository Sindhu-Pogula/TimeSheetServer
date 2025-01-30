using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assignment/Assign
        public async Task<IActionResult> Assign()
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "Username");
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "ProjectName");
            return View();
        }

        // POST: Assignment/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(UserProjectAssignment assignment)
        {
            if (ModelState.IsValid)
            {
                _context.UserProjectAssignments.Add(assignment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User assigned to project successfully!";
                return RedirectToAction("Assign");
            }

            // Reload dropdowns if validation fails
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "Username");
            ViewBag.Projects = new SelectList(await _context.Projects.ToListAsync(), "Id", "ProjectName");
            return View(assignment);
        }
    }
}
