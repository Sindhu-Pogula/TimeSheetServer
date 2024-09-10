using Microsoft.AspNetCore.Mvc;
using TimeSheet.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

            foreach (var entry in entries)
            {
                Console.WriteLine($"Saving entry: {entry.Project}, {entry.Monday}, {entry.Tuesday}, {entry.Wednesday}, {entry.Thursday}, {entry.Friday}, {entry.Saturday}, {entry.Sunday}");
                // Assuming 'Date' and 'Project' together form a unique identifier for a Timesheet entry
                var existingEntry = await _context.Timesheets
                                                  .FirstOrDefaultAsync(e => e.FromDate == entry.FromDate && e.ToDate ==entry.ToDate&& e.Project == entry.Project);
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

        [HttpPost]
        public async Task<IActionResult> DeleteTimesheet(int id)
        {
            var entry = await _context.Timesheets.FindAsync(id);
            if (entry != null)
            {
                _context.Timesheets.Remove(entry);
                await _context.SaveChangesAsync();
            }
            return Json(new { success = true });
        }
        public async Task<IActionResult> History()
        {
            var timesheetHistory = await _context.Timesheets.ToListAsync();
            return View(timesheetHistory);
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

