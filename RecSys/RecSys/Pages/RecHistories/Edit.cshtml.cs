using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_RecHistories
{
    public class EditModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public EditModel(RecSys.Models.RecSysContext context)
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

            var rechistory =  await _context.RecHistories.FirstOrDefaultAsync(m => m.HistoryId == id);
            if (rechistory == null)
            {
                return NotFound();
            }
            RecHistory = rechistory;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(RecHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecHistoryExists(RecHistory.HistoryId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool RecHistoryExists(int id)
        {
          return (_context.RecHistories?.Any(e => e.HistoryId == id)).GetValueOrDefault();
        }
    }
}
