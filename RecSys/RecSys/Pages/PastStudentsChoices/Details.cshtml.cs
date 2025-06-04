using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_PastStudentsChoices
{
    public class DetailsModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public DetailsModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

      public PastStudentsChoice PastStudentsChoice { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.PastStudentsChoices == null)
            {
                return NotFound();
            }

            var paststudentschoice = await _context.PastStudentsChoices.FirstOrDefaultAsync(m => m.StudentId == id);
            if (paststudentschoice == null)
            {
                return NotFound();
            }
            else 
            {
                PastStudentsChoice = paststudentschoice;
            }
            return Page();
        }
    }
}
