using Microsoft.AspNetCore.Mvc;
using TimeSheet.Models;
using System.Linq;
using System.Threading.Tasks;

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

            // Save or update entries
            foreach (var entry in entries)
            {
                var existingEntry = _context.Timesheets
                                             .FirstOrDefault(e => e.Date == entry.Date && e.Project == entry.Project);
                if (existingEntry != null)
                {
                    // Update existing entry
                    existingEntry.Date = entry.Date;
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
    }
}
