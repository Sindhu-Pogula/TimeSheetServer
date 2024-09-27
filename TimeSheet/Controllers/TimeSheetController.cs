using Microsoft.AspNetCore.Mvc;
using TimeSheet.Models;
using System.Linq;
using System.Threading.Tasks;
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync();

            // Redirect to the home page or login page after logout
            return RedirectToAction("", "");
        }

        [HttpPost]
        public async Task<IActionResult> SaveTimesheet([FromBody] Timesheet[] entries)
        {
            if (entries == null || !entries.Any())
                return BadRequest("No entries provided.");

            foreach (var entry in entries)
            {
                Console.WriteLine($"Saving entry: {entry.Project}, {entry.Monday}, {entry.Tuesday}, {entry.Wednesday}, {entry.Thursday}, {entry.Friday}, {entry.Saturday}, {entry.Sunday}");
                // Assuming 'Date' and 'Project' together form a unique identifier for a Timesheet entry
                var existingEntry = await _context.Timesheets
                                                  .FirstOrDefaultAsync(e => e.FromDate == entry.FromDate && e.ToDate == entry.ToDate && e.Project == entry.Project);
                if (existingEntry != null)
                {
                    // Update existing entry
                    //existingEntry.FromDate = entry.FromDate;
                    //existingEntry.ToDate = entry.ToDate;
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

        //[HttpPost]
        //public async Task<IActionResult> DeleteTimesheet(int id)
        //{
        //    var entry = await _context.Timesheets.FindAsync(id);
        //    if (entry != null)
        //    {
        //        _context.Timesheets.Remove(entry);
        //        await _context.SaveChangesAsync();
        //    }
        //    return Json(new { success = true });
        //}
        ////public async Task<IActionResult> History(string searchQuery)
        //{
        //    var timesheets = _context.Timesheets.AsQueryable();

        //    if (!string.IsNullOrEmpty(searchQuery))
        //    {
        //        DateTime fromDate;
        //        DateTime toDate;

        //        if (DateTime.TryParse(searchQuery, out fromDate))
        //        {
        //            timesheets = timesheets.Where(t => t.FromDate.Date == fromDate.Date || t.ToDate.Date == fromDate.Date);
        //        }
        //        else if (DateTime.TryParse(searchQuery, out toDate))
        //        {
        //            timesheets = timesheets.Where(t => t.ToDate.Date == toDate.Date);
        //        }
        //        else
        //        {
        //            timesheets = timesheets.Where(t => t.Project.Contains(searchQuery));
        //        }
        //    }

        //    var filteredTimesheets = await timesheets.ToListAsync();
        //    return View(filteredTimesheets);
        //}

        // GET: Timesheet/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var timesheet = await _context.Timesheets.FindAsync(id);
            if (timesheet == null)
            {
                return NotFound();
            }
            return View(timesheet); // Pass the timesheet to the Edit view
        }

        // POST: Timesheet/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(Timesheet model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing timesheet entry by its ID
                var existingTimesheet = await _context.Timesheets.FindAsync(model.Id);

                if (existingTimesheet != null)
                {
                    // Update the existing timesheet fields
                    existingTimesheet.FromDate = model.FromDate;
                    existingTimesheet.ToDate = model.ToDate;
                    existingTimesheet.Project = model.Project;
                    existingTimesheet.Monday = model.Monday;
                    existingTimesheet.Tuesday = model.Tuesday;
                    existingTimesheet.Wednesday = model.Wednesday;
                    existingTimesheet.Thursday = model.Thursday;
                    existingTimesheet.Friday = model.Friday;
                    existingTimesheet.Saturday = model.Saturday;
                    existingTimesheet.Sunday = model.Sunday;
                    existingTimesheet.TotalHours = model.TotalHours;

                    // Save the updated timesheet entry to the database
                    _context.Update(existingTimesheet);
                    await _context.SaveChangesAsync();
                }

                // Redirect to the Timesheet History page after saving
                return RedirectToAction("History");
            }

            // Return the view with model errors if validation fails
            return View(model);
        }

        public async Task<IActionResult> History(string searchQuery)
        {
            var timesheetHistory = await _context.Timesheets
                .Where(t => string.IsNullOrEmpty(searchQuery) ||
                            t.FromDate.ToString().Contains(searchQuery) ||
                            t.ToDate.ToString().Contains(searchQuery) ||
                            t.Project.Contains(searchQuery))
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

