using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecSys.Models;

namespace RecSys.Pages_RecHistories
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
        public RecHistory RecHistory { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.RecHistories == null || RecHistory == null)
            {
                return Page();
            }

            _context.RecHistories.Add(RecHistory);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
