using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecSys.Models;

namespace RecSys.Pages_Courses
{
    public class CreateModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public CreateModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Courses == null || Course == null)
            {
                return Page();
            }

            // Explicitly ensure CourseId is not set
            Course.CourseId = 0;

            _context.Courses.Add(Course);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}