using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RecSys.Models;

namespace RecSys.Pages_PastStudentsChoices
{
    public class IndexModel : PageModel
    {
        private readonly RecSysContext _context;

        public IndexModel(RecSysContext context)
        {
            _context = context;
        }

        public IList<PastStudentsChoice> PastStudentsChoice { get; set; }
        public IList<Lecturer> Lecturers { get; set; }

        public async Task OnGetAsync()
        {
            PastStudentsChoice = await _context.PastStudentsChoices.ToListAsync();
            Lecturers = await _context.Lecturers.ToListAsync();
        }
    }
}