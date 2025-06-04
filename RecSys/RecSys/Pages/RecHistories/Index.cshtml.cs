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
    public class IndexModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public IndexModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        public IList<RecHistory> RecHistory { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.RecHistories != null)
            {
                RecHistory = await _context.RecHistories.ToListAsync();
            }
        }
    }
}
