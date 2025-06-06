using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_PastStudentsChoices
{
    public class EditModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public EditModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PastStudentsChoice PastStudentsChoice { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.PastStudentsChoices == null)
            {
                return NotFound();
            }

            var paststudentschoice =  await _context.PastStudentsChoices.FirstOrDefaultAsync(m => m.StudentId == id);
            if (paststudentschoice == null)
            {
                return NotFound();
            }
            PastStudentsChoice = paststudentschoice;
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

            _context.Attach(PastStudentsChoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PastStudentsChoiceExists(PastStudentsChoice.StudentId))
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

        private bool PastStudentsChoiceExists(int id)
        {
          return (_context.PastStudentsChoices?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
