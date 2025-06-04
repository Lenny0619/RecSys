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
    public class DetailsModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public DetailsModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

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
    }
}
