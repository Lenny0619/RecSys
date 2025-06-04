using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_RecHistories
{
    public class DeleteModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public DeleteModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        [BindProperty]
      public RecHistory RecHistory { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.RecHistories == null)
            {
                return NotFound();
            }

            var rechistory = await _context.RecHistories.FirstOrDefaultAsync(m => m.HistoryId == id);

            if (rechistory == null)
            {
                return NotFound();
            }
            else 
            {
                RecHistory = rechistory;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.RecHistories == null)
            {
                return NotFound();
            }
            var rechistory = await _context.RecHistories.FindAsync(id);

            if (rechistory != null)
            {
                RecHistory = rechistory;
                _context.RecHistories.Remove(RecHistory);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
