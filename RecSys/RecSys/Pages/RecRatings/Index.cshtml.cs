using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_RecRatings
{
    public class IndexModel : PageModel
    {
        private readonly RecSysContext _context;

        public IndexModel(RecSysContext context)
        {
            _context = context;
        }

        public IList<RecRating> RecRating { get; set; }
        public IList<Lecturer> Lecturers { get; set; }

        public async Task OnGetAsync()
        {
            RecRating = await _context.RecRatings
                .Include(r => r.History)
                .ToListAsync();

            Lecturers = await _context.Lecturers.ToListAsync();
        }
    }
}