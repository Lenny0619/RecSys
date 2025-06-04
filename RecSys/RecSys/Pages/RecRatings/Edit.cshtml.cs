using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_RecRatings
{
    public class EditModel : PageModel
    {
        private readonly RecSysContext _context;

        public EditModel(RecSysContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RecRating RecRating { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.RecRatings == null)
            {
                return NotFound();
            }

            var recrating = await _context.RecRatings
                .Include(r => r.RecommendedLecturer)
                .FirstOrDefaultAsync(m => m.RatingId == id);

            if (recrating == null)
            {
                return NotFound();
            }

            RecRating = recrating;
            LecturerName = recrating.RecommendedLecturer?.SvName ?? "Unknown Lecturer";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Get the existing record from database
            var existingRating = await _context.RecRatings.FindAsync(RecRating.RatingId);

            if (existingRating == null)
            {
                return NotFound();
            }

            // Only update the fields we want to allow editing
            existingRating.SubmittedRating = RecRating.SubmittedRating;
            existingRating.SubmittedAt = DateTime.Now;

            try
            {
                // Explicitly mark only these fields as modified
                _context.Entry(existingRating).Property(x => x.SubmittedRating).IsModified = true;
                _context.Entry(existingRating).Property(x => x.SubmittedAt).IsModified = true;

                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecRatingExists(RecRating.RatingId))
                {
                    return NotFound();
                }
                throw;
            }
        }

        private bool RecRatingExists(int id)
        {
            return _context.RecRatings.Any(e => e.RatingId == id);
        }
    }
}