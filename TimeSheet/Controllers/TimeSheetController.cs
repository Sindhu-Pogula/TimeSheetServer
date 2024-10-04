using Microsoft.AspNetCore.Mvc;
using TimeSheet.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace TimeSheet.Controllers
{
    public class TimesheetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimesheetController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> SaveTimesheet([FromBody] Timesheet[] entries)
        {
            if (entries == null || !entries.Any())
                return BadRequest("No entries provided.");

            // Retrieve the logged-in user's username from the session
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return Unauthorized(); // Ensure user is logged in

            // Find the user by the username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return Unauthorized();

            foreach (var entry in entries)
            {
                entry.UserId = user.Id; // Link timesheet to the logged-in user

                var existingEntry = await _context.Timesheets
                                                  .FirstOrDefaultAsync(e => e.FromDate == entry.FromDate
                                                                         && e.ToDate == entry.ToDate
                                                                         && e.Project == entry.Project
                                                                         && e.UserId == user.Id); // Ensure timesheet is unique to the user

                if (existingEntry != null)
                {
                    // Update existing entry
                    existingEntry.Monday = entry.Monday;
                    existingEntry.Tuesday = entry.Tuesday;
                    existingEntry.Wednesday = entry.Wednesday;
                    existingEntry.Thursday = entry.Thursday;
                    existingEntry.Friday = entry.Friday;
                    existingEntry.Saturday = entry.Saturday;
                    existingEntry.Sunday = entry.Sunday;
                    existingEntry.TotalHours = entry.TotalHours;
                    _context.Timesheets.Update(existingEntry);
                }
                else
                {
                    // Create new entry
                    _context.Timesheets.Add(entry);
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }


        public async Task<IActionResult> History(string searchQuery)
        {
            // Retrieve the logged-in user's username from session
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Account");

            // Find the user by the username
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return RedirectToAction("Login", "Account");

            // Fetch only the timesheets belonging to the logged-in user
            var timesheetHistory = await _context.Timesheets
                .Where(t => t.UserId == user.Id &&
                            (string.IsNullOrEmpty(searchQuery) ||
                             t.FromDate.ToString().Contains(searchQuery) ||
                             t.ToDate.ToString().Contains(searchQuery) ||
                             t.Project.Contains(searchQuery)))
                .ToListAsync();

            ViewData["SearchQuery"] = searchQuery;

            return View(timesheetHistory);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }

            _context.Timesheets.Remove(timesheet);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Deleted successfully!";
            return RedirectToAction("History");
        }

        public async Task<IActionResult> DownloadTimesheet()
        {
            // Fetch timesheet data
            var timesheets = await _context.Timesheets.ToListAsync();

            // Create CSV content
            var csv = new StringBuilder();
            csv.AppendLine("FromDate,ToDate,Project,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday,TotalHours");

            foreach (var entry in timesheets)
            {
                csv.AppendLine($"{entry.FromDate},{entry.ToDate},{entry.Project},{entry.Monday},{entry.Tuesday},{entry.Wednesday},{entry.Thursday},{entry.Friday},{entry.Saturday},{entry.Sunday},{entry.TotalHours}");
            }

            // Convert CSV content to byte array
            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            // Return CSV file for download
            return File(bytes, "text/csv", "TimesheetData.csv");
        }
    }
}

