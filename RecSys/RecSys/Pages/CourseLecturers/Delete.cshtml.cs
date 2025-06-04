using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_CourseLecturers
{
    public class DeleteModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public DeleteModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CourseLecturer CourseLecturer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? svId, int? courseId, int? year, int? sem)
        {
            if (svId == null || courseId == null || year == null || sem == null)
            {
                return NotFound();
            }

            CourseLecturer = await _context.CourseLecturers
                .Include(cl => cl.Course)
                .Include(cl => cl.Sv)
                .FirstOrDefaultAsync(m => m.SvId == svId
                                      && m.CourseId == courseId
                                      && m.Year == year
                                      && m.Sem == sem);

            if (CourseLecturer == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? svId, int? courseId, int? year, int? sem)
        {
            if (svId == null || courseId == null || year == null || sem == null)
            {
                return NotFound();
            }

            CourseLecturer = await _context.CourseLecturers
                .FirstOrDefaultAsync(m => m.SvId == svId
                                       && m.CourseId == courseId
                                       && m.Year == year
                                       && m.Sem == sem);

            if (CourseLecturer != null)
            {
                _context.CourseLecturers.Remove(CourseLecturer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}