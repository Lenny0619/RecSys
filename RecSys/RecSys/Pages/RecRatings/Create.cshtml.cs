using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecSys.Models;

namespace RecSys.Pages_RecRatings
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
        ViewData["HistoryId"] = new SelectList(_context.RecHistories, "HistoryId", "HistoryId");
            return Page();
        }

        [BindProperty]
        public RecRating RecRating { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.RecRatings == null || RecRating == null)
            {
                return Page();
            }

            _context.RecRatings.Add(RecRating);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
