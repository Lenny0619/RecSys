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
    public class DetailsModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public DetailsModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

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

            if (RecRating == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
