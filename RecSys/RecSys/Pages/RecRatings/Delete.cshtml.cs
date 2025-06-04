using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_RecRatings
{
    public class DeleteModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public DeleteModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        [BindProperty]
      public RecRating RecRating { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.RecRatings == null)
            {
                return NotFound();
            }

            RecRating = await _context.RecRatings
                .Include(r => r.RecommendedLecturer)
                .Include(r => r.History)
                .FirstOrDefaultAsync(m => m.RatingId == id);

            var recrating = await _context.RecRatings.FirstOrDefaultAsync(m => m.RatingId == id);

            if (recrating == null)
            {
                return NotFound();
            }
            else 
            {
                RecRating = recrating;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.RecRatings == null)
            {
                return NotFound();
            }
            var recrating = await _context.RecRatings.FindAsync(id);

            if (recrating != null)
            {
                RecRating = recrating;
                _context.RecRatings.Remove(RecRating);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
