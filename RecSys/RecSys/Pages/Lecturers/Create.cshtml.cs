using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecSys.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace RecSys.Pages_Lecturers
{
    public class CreateModel : PageModel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly RecSys.Models.RecSysContext _context;

        public CreateModel(RecSys.Models.RecSysContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Lecturer Lecturer { get; set; }

        [BindProperty]
        public IFormFile ImageUpload { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Ensure SvId is not being set manually
            Lecturer.SvId = 0; // Or remove any code that sets SvId

            if (ImageUpload != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageUpload.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageUpload.CopyToAsync(fileStream);
                }

                Lecturer.ImagePath = "/images/" + uniqueFileName;
            }

            _context.Lecturers.Add(Lecturer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}