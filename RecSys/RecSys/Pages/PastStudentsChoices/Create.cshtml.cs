using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecSys.Models;

namespace RecSys.Pages_PastStudentsChoices
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
        public PastStudentsChoice PastStudentsChoice { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.PastStudentsChoices == null || PastStudentsChoice == null)
            {
                return Page();
            }

            _context.PastStudentsChoices.Add(PastStudentsChoice);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
