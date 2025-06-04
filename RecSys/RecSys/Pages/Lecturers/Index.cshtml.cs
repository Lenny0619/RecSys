using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_Lecturers
{
    public class IndexModel : PageModel
    {
        private readonly RecSys.Models.RecSysContext _context;

        public IndexModel(RecSys.Models.RecSysContext context)
        {
            _context = context;
        }

        public IList<Lecturer> Lecturer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Lecturers != null)
            {
                Lecturer = await _context.Lecturers.ToListAsync();
            }
        }
    }
}
