using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_CourseLecturers
{
    public class EditModel : PageModel
    {
        private readonly RecSysContext _context;

        public EditModel(RecSysContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CourseLecturer CourseLecturer { get; set; }

        [BindProperty]
        public int OriginalSvId { get; set; }

        [BindProperty]
        public int OriginalCourseId { get; set; }

        [BindProperty]
        public int OriginalYear { get; set; }

        [BindProperty]
        public int OriginalSem { get; set; }

        public async Task<IActionResult> OnGetAsync(int svId, int courseId, int year, int sem)
        {
            CourseLecturer = await _context.CourseLecturers
                .Include(cl => cl.Sv)
                .Include(cl => cl.Course)
                .FirstOrDefaultAsync(m => m.SvId == svId
                                      && m.CourseId == courseId
                                      && m.Year == year
                                      && m.Sem == sem);

            if (CourseLecturer == null)
            {
                return NotFound();
            }

            // Store original values
            OriginalSvId = svId;
            OriginalCourseId = courseId;
            OriginalYear = year;
            OriginalSem = sem;

            ViewData["CourseName"] = CourseLecturer.Course?.CourseName;
            ViewData["LecturerName"] = CourseLecturer.Sv?.SvName;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // First delete the original record
            var originalRecord = await _context.CourseLecturers
                .FirstOrDefaultAsync(m => m.SvId == OriginalSvId
                                       && m.CourseId == OriginalCourseId
                                       && m.Year == OriginalYear
                                       && m.Sem == OriginalSem);

            if (originalRecord == null)
            {
                return NotFound();
            }

            _context.CourseLecturers.Remove(originalRecord);
            await _context.SaveChangesAsync();

            // Now create a new record with the updated values
            var newRecord = new CourseLecturer
            {
                SvId = OriginalSvId,
                CourseId = OriginalCourseId,
                Year = CourseLecturer.Year,
                Sem = CourseLecturer.Sem
            };

            _context.CourseLecturers.Add(newRecord);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists, " +
                    "see your system administrator.");
                return Page();
            }
        }

        private bool CourseLecturerExists(int svId, int courseId, int year, int sem)
        {
            return _context.CourseLecturers.Any(e => e.SvId == svId
                                                 && e.CourseId == courseId
                                                 && e.Year == year
                                                 && e.Sem == sem);
        }
    }
}