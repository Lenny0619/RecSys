using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using RecSys.Data;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecSys.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ISupervisorRecommendationService _recommendationService;
        private readonly RecSysContext _context;

        public IndexModel(ISupervisorRecommendationService recommendationService, RecSysContext context)
        {
            _recommendationService = recommendationService;
            _context = context;
        }

        public List<SelectListItem> CourseOptions { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Course 1 is required")]
        public string Course1 { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Course 2 is required")]
        public string Course2 { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Course 3 is required")]
        public string Course3 { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Project focus is required")]
        public string ProjectFocus { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Reload course options in case of validation error
            CourseOptions = await _context.Courses
                .OrderBy(c => c.CourseCode)
                .Select(c => new SelectListItem
                {
                    Value = c.CourseName,
                    Text = c.CourseCode + " - " + c.CourseName
                })
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var history = new RecHistory
            {
                AnonymousId = Guid.NewGuid(),
                PreferredCourses = $"{Course1},{Course2},{Course3}",
                ProjectFocus = ProjectFocus,
                SubmittedAt = DateTime.Now
            };

            _context.RecHistories.Add(history);
            await _context.SaveChangesAsync();

            return RedirectToPage("ResultPage", new
            {
                courseNames = $"{Course1},{Course2},{Course3}",
                projectFocus = ProjectFocus,
                historyId = history.HistoryId
            });
        }
        public async Task OnGetAsync()
        {
            HttpContext.Session.Clear();

            CourseOptions = await _context.Courses
                .OrderBy(c => c.CourseCode)
                .Select(c => new SelectListItem
                {
                    Value = c.CourseName,
                    Text = c.CourseCode + " - " + c.CourseName
                })
                .ToListAsync();
        }
    }
}