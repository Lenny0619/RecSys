using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_CourseLecturers
{
    public class CreateModel : PageModel
    {
        private readonly RecSysContext _context;

        public CreateModel(RecSysContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
            ViewData["SvId"] = new SelectList(_context.Lecturers, "SvId", "SvName");
            return Page();
        }

        [BindProperty]
        public CourseLecturer CourseLecturer { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"Attempting to save: SvId={CourseLecturer.SvId}, CourseId={CourseLecturer.CourseId}, Year={CourseLecturer.Year}, Sem={CourseLecturer.Sem}");

            // Verify the received values
            if (CourseLecturer.SvId == 0 || CourseLecturer.CourseId == 0)
            {
                Console.WriteLine("Validation failed - missing required fields");
                ModelState.AddModelError("", "Both Lecturer and Course must be selected.");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model state invalid");
                ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
                ViewData["SvId"] = new SelectList(_context.Lecturers, "SvId", "SvName");
                return Page();
            }

            // Check if the referenced entities exist
            var lecturerExists = await _context.Lecturers.AnyAsync(l => l.SvId == CourseLecturer.SvId);
            var courseExists = await _context.Courses.AnyAsync(c => c.CourseId == CourseLecturer.CourseId);

            if (!lecturerExists || !courseExists)
            {
                Console.WriteLine($"Referenced entities not found: Lecturer exists={lecturerExists}, Course exists={courseExists}");
                ModelState.AddModelError("", "The selected lecturer or course doesn't exist.");
                ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
                ViewData["SvId"] = new SelectList(_context.Lecturers, "SvId", "SvName");
                return Page();
            }

            try
            {
                // Create new mapping without setting navigation properties
                var newMapping = new CourseLecturer
                {
                    SvId = CourseLecturer.SvId,
                    CourseId = CourseLecturer.CourseId,
                    Year = CourseLecturer.Year,
                    Sem = CourseLecturer.Sem
                };

                _context.CourseLecturers.Add(newMapping);
                await _context.SaveChangesAsync();
                Console.WriteLine("Successfully saved to database");
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database error: {ex.InnerException?.Message ?? ex.Message}");
                ModelState.AddModelError("", $"Database error: {ex.InnerException?.Message ?? ex.Message}");
                ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
                ViewData["SvId"] = new SelectList(_context.Lecturers, "SvId", "SvName");
                return Page();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                ViewData["CourseId"] = new SelectList(_context.Courses, "CourseId", "CourseName");
                ViewData["SvId"] = new SelectList(_context.Lecturers, "SvId", "SvName");
                return Page();
            }
        }
    }
}